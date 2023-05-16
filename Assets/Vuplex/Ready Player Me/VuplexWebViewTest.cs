using UnityEngine;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.WebView;
using Vuplex.WebView;

public class VuplexWebViewTest : MonoBehaviour
{
    private const string TAG = nameof(VuplexWebViewTest);
    
    private GameObject avatar;
    private AvatarObjectLoader avatarLoader;
    private VuplexWebView vuplexWebView;

    [SerializeField] private GameObject loading;
    [SerializeField] private BaseWebViewPrefab canvasWebView;
    [SerializeField]
    private bool initializeOnStart = true;
    [SerializeField] private UrlConfig urlConfig;
    
    private CanvasGroup canvasGroup;


    
    private void Start()
    {
        if (initializeOnStart)
        {
            SetupVuplexWebView();
        }
        
        canvasGroup = canvasWebView.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    
    public void SetupVuplexWebView()
    {
        vuplexWebView = new VuplexWebView();
        vuplexWebView.Initialize(canvasWebView, urlConfig);
        vuplexWebView.OnPageLoadFinished = OnPageLoadFinished;
        vuplexWebView.OnPageLoadStarted = OnPageLoadStarted;
        vuplexWebView.OnAvatarExport = HandleAvatarExported;
        vuplexWebView.OnUserAuthorized = HandleUserAuthorized;
        vuplexWebView.OnUserSet = HandleUserSet;
        vuplexWebView.OnAssetUnlock = HandleAssetUnlock;
    }
    
    /// <summary>
    /// This can be used to reload the webview with a new login token (useful for auto-login)
    /// </summary>
    /// <param name="loginToken">An auth token used to automatically login to the WebView with your RPM account</param>
    public void ReloadVuplexWebView(string loginToken = "")
    {
        vuplexWebView.ReloadWithUrl(urlConfig, loginToken);
    }
    
    private void OnPageLoadStarted()
    {
        loading.SetActive(true);
    }
    
    private void OnPageLoadFinished()
    {
        loading.SetActive(false);
        canvasGroup.alpha = 1;
    }
    
    // Called when the v1.avatar.exported event is received from the webview (After you click Next)
    private void HandleAvatarExported(string avatarUrl)
    {
        SDKLogger.Log(TAG, $"Avatar Exported: avatarUrl = {avatarUrl}");
        loading.SetActive(true);
        avatarLoader = new AvatarObjectLoader();
        avatarLoader.OnCompleted += HandleAvatarLoadCompleted;
        avatarLoader.LoadAvatar(avatarUrl);
    }
    
    // Called when the v1.asset.unlock event is received from the webview
    private void HandleAssetUnlock(AssetRecord asset)
    {
        SDKLogger.Log(TAG, $"Asset Unlocked: assetId = {asset.AssetId} userId = {asset.UserId}");
    }

    // Called when the v1.user.set event is received from the webview
    private void HandleUserSet(string userId)
    {
        SDKLogger.Log(TAG, $"User Set: userId = {userId}");
    }

    // Called when the v1.user.authorized event is received from the webview
    private void HandleUserAuthorized(string userId)
    {
        SDKLogger.Log(TAG, $"User Authorized: userId = {userId}");
    }

    /// <summary>
    /// Called when the <seealso cref="AvatarObjectLoader"/> has finished loading the avatar
    /// </summary>
    private void HandleAvatarLoadCompleted(object sender, CompletionEventArgs args)
    {
        // cleanup previous avatar
        if (avatar != null)
        {
            Destroy(avatar);
        }
        avatar = args.Avatar;
        avatar.transform.Rotate(Vector3.up, 180);
        avatar.transform.position = new Vector3(0.75f, 0, 0.75f);
        avatar.gameObject.SetActive(true);

        loading.SetActive(false);
        SetWebViewVisibility(false);
        SDKLogger.Log(TAG, "Avatar Load Completed");
    }

    // Show or hide the webview
    public void SetWebViewVisibility(bool visible)
    {
        canvasWebView.gameObject.SetActive(visible);
    }
}
