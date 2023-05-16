using System;
using UnityEngine;
using Vuplex.WebView;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.WebView;

public class VuplexWebView
{
    private const string TAG = nameof(VuplexWebView);
    
    private BaseWebViewPrefab webView;

    public Action OnInitialized;
    
    public Action<string> OnAvatarExport;
    public Action<string> OnUserSet;
    public Action<string> OnUserAuthorized;
    public Action<AssetRecord> OnAssetUnlock;
    
    public Action OnPageLoadFinished;
    public Action OnPageLoadStarted;
    
    public void Initialize(BaseWebViewPrefab prefab, UrlConfig urlConfig, string loginToken = "")
    {
        Web.SetCameraAndMicrophoneEnabled(true);
        webView = prefab;
        
        webView.InitialUrl = urlConfig.BuildUrl(loginToken);
        webView.DragMode = DragMode.DragWithinPage;

        webView.Initialized += (sender, args) =>
        {
            webView.WebView.LoadProgressChanged -= OnLoadProgressChanged;
            webView.WebView.LoadProgressChanged += OnLoadProgressChanged;
        };
    }

    public void ReloadWithUrl(UrlConfig urlConfig, string loginToken = "")
    {
        webView.WebView.LoadUrl(urlConfig.BuildUrl(loginToken));
    }

    private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs args)
    {
        if (args.Type == ProgressChangeType.Started)
        {
            OnPageLoadStarted?.Invoke();
            return;
        }
        if (args.Type == ProgressChangeType.Finished)
        {
            OnPageLoadFinished?.Invoke();
            webView.WebView.MessageEmitted -= OnMessageReceived;
            webView.WebView.MessageEmitted += OnMessageReceived;
            webView.WebView.ExecuteJavaScript(@"
                function ready() {
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
        OnInitialized?.Invoke();
    }

    private void OnMessageReceived(object sender, EventArgs<string> args)
    {
        try
        {
            WebMessage webMessage = JsonConvert.DeserializeObject<WebMessage>(args.Value);

            HandleMessage(webMessage);
        }
        catch (Exception e)
        {
            SDKLogger.Log(TAG,$"OnMessageReceived: { args.Value }\nError Message: { e.Message }");
        }
    }

    private void HandleMessage(WebMessage webMessage)
    {
        switch (webMessage.eventName)
        {
            case WebViewEvents.AVATAR_EXPORT:
                Debug.Log(webMessage.eventName);
                OnAvatarExport?.Invoke(webMessage.GetAvatarUrl());
                break;
            case WebViewEvents.USER_SET:
                Debug.Log(webMessage.eventName);
                OnUserSet?.Invoke(webMessage.GetUserId());
                break;
            case WebViewEvents.USER_AUTHORIZED:
                Debug.Log(webMessage.eventName);
                OnUserAuthorized?.Invoke(webMessage.GetUserId());
                break;
            case WebViewEvents.ASSET_UNLOCK:
                OnAssetUnlock?.Invoke(webMessage.GetAssetRecord());
                break;
        }
    }
    
}
