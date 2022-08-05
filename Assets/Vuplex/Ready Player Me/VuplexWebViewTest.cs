#if VUPLEX_CCU
using UnityEngine;
using ReadyPlayerMe;
using Vuplex.WebView;
public class VuplexWebViewTest : MonoBehaviour
{
    private GameObject avatar;
    private AvatarLoader avatarLoader;
    private VuplexWebView vuplexWebView;

    [SerializeField] private GameObject loading;
    [SerializeField] private BaseWebViewPrefab canvasWebView;

    private void Start()
    {
        vuplexWebView = new VuplexWebView();
        vuplexWebView.Initialize(canvasWebView);
        vuplexWebView.OnInitialized = () => Debug.Log("WebView Initialized");
        vuplexWebView.OnAvatarUrlReceived = OnAvatarUrlReceived;
    }

    // WebView callback for retrieving avatar url
    private void OnAvatarUrlReceived(string avatarUrl)
    {
        loading.SetActive(true);
        avatarLoader = new AvatarLoader();
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
