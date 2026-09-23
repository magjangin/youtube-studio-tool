using System;
using Avalonia.Controls;

namespace youtube_studio_custom
{
    public partial class MainWindow : Window
    {
        private static readonly Uri StudioUri = new("https://studio.youtube.com");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WebView_OnEnvironmentRequested(object? sender, WebViewEnvironmentRequestedEventArgs e) =>
            YoutubeStudioWebView.ConfigureEnvironment(e);

        private async void WebView_OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
                await YoutubeStudioWebView.HideSubscriberCountsAsync(WebView);
        }

        // The document-created script registration below is async, so the very first navigation
        // must wait for it to finish - otherwise that first document can be created before the
        // registration lands, defeating the whole point for the load that matters most.
        private async void WebView_OnAdapterCreated(object? sender, WebViewAdapterEventArgs e)
        {
            await YoutubeStudioWebView.RegisterDocumentCreatedHideScriptAsync(WebView);
            WebView.Source = StudioUri;
        }
    }
}
