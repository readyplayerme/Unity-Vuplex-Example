#if VUPLEX_CCU
using UnityEngine;
using Vuplex.WebView;
using Vuplex.WebView.Demos;

public class VuplexKeyboard : MonoBehaviour
{
        [SerializeField] private CanvasWebViewPrefab canvasWebViewPrefab;
        private HardwareKeyboardListener hardwareKeyboardListener;

        async void Start() {
            if (canvasWebViewPrefab == null)
            {
                canvasWebViewPrefab = FindObjectOfType<CanvasWebViewPrefab>();
            }
            
            SetUpHardwareKeyboard();

            // Wait for the CanvasWebViewPrefab to initialize, because the CanvasWebViewPrefab.WebView property
            // is null until the prefab has initialized.
            await canvasWebViewPrefab.WaitUntilInitialized();

            // The CanvasWebViewPrefab has initialized, so now we can use the IWebView APIs
            // using its CanvasWebViewPrefab.WebView property.
            // https://developer.vuplex.com/webview/IWebView
            canvasWebViewPrefab.WebView.UrlChanged += (sender, eventArgs) => {
                Debug.Log("[CanvasWebViewDemo] URL changed: " + eventArgs.Url);
            };
        }

        private void SetUpHardwareKeyboard() {

            // Send keys from the hardware (USB or Bluetooth) keyboard to the webview.
            // Use separate KeyDown() and KeyUp() methods if the webview supports
            // it, otherwise just use IWebView.SendKey().
            // https://developer.vuplex.com/webview/IWithKeyDownAndUp
            hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
            hardwareKeyboardListener.KeyDownReceived += (sender, eventArgs) => {
                var webViewWithKeyDown = canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
                if (webViewWithKeyDown != null) {
                    webViewWithKeyDown.KeyDown(eventArgs.Value, eventArgs.Modifiers);
                } else {
                    canvasWebViewPrefab.WebView.SendKey(eventArgs.Value);
                }
            };
            hardwareKeyboardListener.KeyUpReceived += (sender, eventArgs) => {
                var webViewWithKeyUp = canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
                webViewWithKeyUp?.KeyUp(eventArgs.Value, eventArgs.Modifiers);
            };
        }
}
#endif
