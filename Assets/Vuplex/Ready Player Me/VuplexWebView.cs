using System;
using System.Text;
using UnityEngine;
using Vuplex.WebView;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.WebView;

public class VuplexWebView
{
    private const string DATA_URL_FIELD_NAME = "url";
    private const string AVATAR_EXPORT_EVENT_NAME = "v1.avatar.exported";
    private const string CLEAR_CACHE_PARAM = "clearCache";
    private const string FRAME_API_PARAM = "frameApi";
    private const string QUICK_START_PARAM = "quickStart";
    private const string SELECT_BODY_PARAM = "selectBodyType";
    private const string TAG = nameof(VuplexWebView);
    private BaseWebViewPrefab webView;

    public Action OnInitialized;
    public Action<string> OnAvatarUrlReceived;
    public Action OnPageLoadFinished;
    public Action OnPageLoadStarted;

    public void Initialize(BaseWebViewPrefab prefab, UrlConfig urlConfig)
    {
        Web.SetCameraAndMicrophoneEnabled(true);
        webView = prefab;

        // TODO this function will be added to WebView module in future update
        string url = GetUrlFromConfig(urlConfig);
        webView.InitialUrl = url;
        webView.DragMode = DragMode.DragWithinPage;

        webView.Initialized += (sender, args) =>
        {
            Debug.Log("--- INIT");

            webView.WebView.LoadProgressChanged -= OnLoadProgressChanged;
            webView.WebView.LoadProgressChanged += OnLoadProgressChanged;
        };
    }

    /// <summary>
    /// TODO this function will be added to WebView module in future update
    /// we can remove once update is released to reduce code duplication
    /// Builds RPM website URL for partner with given parameters.
    /// </summary>
    /// <returns>The Url to load in the WebView.</returns>
    private string GetUrlFromConfig(UrlConfig urlConfig)
    {
        var partnerSubdomain = CoreSettingsHandler.CoreSettings.Subdomain;
        var builder = new StringBuilder($"https://{partnerSubdomain}.readyplayer.me/");
        builder.Append(urlConfig.language != Language.Default ? $"{urlConfig.language.GetValue()}/" : string.Empty);
        builder.Append($"avatar?{FRAME_API_PARAM}");
        builder.Append(urlConfig.clearCache ? $"&{CLEAR_CACHE_PARAM}" : string.Empty);

        if (urlConfig.quickStart)
        {
            builder.Append(QUICK_START_PARAM);
        }
        else
        {
            builder.Append(urlConfig.gender != Gender.None ? $"&gender={urlConfig.gender.GetValue()}" : string.Empty);
            builder.Append(urlConfig.bodyType == BodyType.Selectable ? $"&{SELECT_BODY_PARAM}" : $"&bodyType={urlConfig.bodyType.GetValue()}");
        }

        var url = builder.ToString();
        SDKLogger.AvatarLoaderLogger.Log(TAG, "url");

        return url;
    }

    private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs args)
    {
        Debug.Log($"--- PROGRESS: {args.Progress} - {args.Type}");
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
        Debug.Log($"--- JS EXECUTED: {result}");

        OnInitialized?.Invoke();
    }

    private void OnMessageReceived(object sender, EventArgs<string> args)
    {
        Debug.Log($"OnMessageReceived: { args.Value }");

        try
        {
            WebMessage webMessage = JsonConvert.DeserializeObject<WebMessage>(args.Value);

            if (webMessage.eventName == AVATAR_EXPORT_EVENT_NAME)
            {
                if (webMessage.data.TryGetValue(DATA_URL_FIELD_NAME, out string avatarUrl))
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
