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
    private void OnAvatarUrlReceived(string url)
    {
        loading.SetActive(true);
        avatarLoader = new AvatarLoader();
        avatarLoader.LoadAvatar(url, OnAvatarImported, OnAvatarLoaded);
    }

    // AvatarLoader callback when import is done
    private void OnAvatarImported(GameObject avatar)
    {
        avatar.gameObject.SetActive(false);
        avatar.transform.Rotate(Vector3.up, 180);
        Debug.Log("Avatar Imported");
    }

    // AvatarLoader callback for retrieving loaded avatar game object
    private void OnAvatarLoaded(GameObject avatar, AvatarMetadata metaData)
    {
        Destroy(this.avatar);

        this.avatar = avatar;
        avatar.gameObject.SetActive(true);

        loading.SetActive(false);
        SetWebViewVisibility(false);
        Debug.Log("Avatar Loaded");
    }
    
    public void SetWebViewVisibility(bool visible)
    {
        canvasWebView.gameObject.SetActive(visible);
        canvasWebView.Visible = visible;
        Debug.Log("SetActive");
    }
}
