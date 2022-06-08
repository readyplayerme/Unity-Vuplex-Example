using System;
using UnityEngine;
using Vuplex.WebView;
using Newtonsoft.Json;

public class VuplexWebView
{
    private CanvasWebViewPrefab webView;

    private const string DataUrlFieldName = "url";
    private const string AvatarExportEventName = "v1.avatar.exported";

    public Action OnInitialized;
    public Action<string> OnAvatarUrlReceived;

    public void Initialize(CanvasWebViewPrefab prefab)
    {
        webView = prefab;

        PartnerSO partner = Resources.Load<PartnerSO>("Partner");
        string url = partner.GetUrl();
        webView.InitialUrl = url;
        webView.DragMode = DragMode.DragWithinPage;

        webView.Initialized += (object sender, EventArgs args) =>
        {
            Debug.Log("--- INIT");

            webView.WebView.LoadProgressChanged -= OnLoadProgressChanged;
            webView.WebView.LoadProgressChanged += OnLoadProgressChanged;
        };
    }

    private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs args)
    {
        Debug.Log($"--- PROGRESS: {args.Progress} - {args.Type}");

        if (args.Type == ProgressChangeType.Finished)
        {
            webView.WebView.MessageEmitted -= OnMessageReceived;
            webView.WebView.MessageEmitted += OnMessageReceived;

            webView.WebView.ExecuteJavaScript(@"
                function ready() {
                    document.cookie = 'webview=true';

                    window.removeEventListener('message', subscribe)
                    window.addEventListener('message', subscribe)

                    window.postMessage(
                        JSON.stringify({
                            target: 'readyplayerme',
                            type: 'subscribe',
                            eventName: 'v1.**'
                        }),
                        '*'
                    );
                }

                function subscribe(event) {
                    // post message v1, this will be deprecated
                    if(event.data.endsWith('.glb')) {
                        window.vuplex.postMessage(event.data)
                    }
                    // post message v2
                    else {
                        const json = parse(event);
                        const source = json.source;
                    
                        if (source !== 'readyplayerme') {
                            return;
                        }

			            window.vuplex.postMessage(event.data)
                    }
		        }

                function parse(event) {
                    try {
                        return JSON.parse(event.data);
                    } catch (error) {
                        return null;
                    }
                }

                if (window.vuplex) {
                    ready();
                } else {
                    window.removeEventListener('vuplexready', ready);
                    window.addEventListener('vuplexready', ready);
                }
            ", OnJsExecuted);
        }
    }

    private void OnJsExecuted(string result)
    {
        Debug.Log($"--- JS EXECUTED: {result}");

        OnInitialized?.Invoke();
    }

    private void OnMessageReceived(object sender, EventArgs<string> args)
    {
        Debug.Log($"OnMessageReceived: { args.Value }");

        try
        {
            WebMessage webMessage = JsonConvert.DeserializeObject<WebMessage>(args.Value);

            if (webMessage.eventName == AvatarExportEventName)
            {
                if (webMessage.data.TryGetValue(DataUrlFieldName, out string avatarUrl))
                {
                    OnAvatarUrlReceived?.Invoke(avatarUrl);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"OnMessageReceived: { args.Value }\nError Message: { e.Message }");
        }
    }
}
