#if VUPLEX_CCU
using UnityEngine;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.WebView;
using Vuplex.WebView;
public class VuplexWebViewTest : MonoBehaviour
{
    private GameObject avatar;
    private AvatarObjectLoader avatarLoader;
    private VuplexWebView vuplexWebView;

    [SerializeField] private GameObject loading;
    [SerializeField] private BaseWebViewPrefab canvasWebView;
    [SerializeField] private UrlConfig urlConfig;
    
    private CanvasGroup canvasGroup;
    private void Start()
    {
        vuplexWebView = new VuplexWebView();
        vuplexWebView.Initialize(canvasWebView, urlConfig);
        vuplexWebView.OnInitialized = () => Debug.Log("WebView Initialized");
        vuplexWebView.OnAvatarUrlReceived = OnAvatarUrlReceived;
        vuplexWebView.OnPageLoadFinished = OnPageLoadFinished;
        vuplexWebView.OnPageLoadStarted = OnPageLoadStarted;
        canvasGroup = canvasWebView.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
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

    // WebView callback for retrieving avatar url
    private void OnAvatarUrlReceived(string avatarUrl)
    {
        loading.SetActive(true);
        avatarLoader = new AvatarObjectLoader();
        avatarLoader.OnCompleted += OnAvatarLoadCompleted;
        avatarLoader.LoadAvatar(avatarUrl);
    }

    private void OnAvatarLoadCompleted(object sender, CompletionEventArgs args)
    {
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
        Debug.Log("Avatar Load Completed");
    }

    public void SetWebViewVisibility(bool visible)
    {
        canvasWebView.gameObject.SetActive(visible);
    }

}
#endif
