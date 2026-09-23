using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using Microsoft.Web.WebView2.Core;

namespace youtube_studio_custom
{
    /// <summary>
    /// YouTube Studio specific WebView2 setup: persistent session storage and the
    /// subscriber-count hiding script, kept separate from window/UI wiring in <see cref="MainWindow"/>.
    /// </summary>
    internal static class YoutubeStudioWebView
    {
        private static readonly Lazy<string> HideSubscriberCountScript = new(LoadHideSubscriberCountScript);

        public static void ConfigureEnvironment(WebViewEnvironmentRequestedEventArgs e)
        {
            // Needed to right-click "검사"(Inspect)/F12 into DevTools and read YouTube Studio's
            // real markup, since its class/id names shift between redesigns.
            e.EnableDevTools = true;

            // Pin WebView2 to a dedicated, persistent profile folder so login/session
            // cookies survive across app restarts instead of using a throwaway profile.
            if (e is WindowsWebView2EnvironmentRequestedEventArgs webView2)
            {
                webView2.ProfileName = "YoutubeStudioCustom";
                webView2.UserDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "YoutubeStudioCustom", "WebView2");
            }
        }

        public static Task HideSubscriberCountsAsync(NativeWebView webView) =>
            webView.InvokeScript(HideSubscriberCountScript.Value);

        // InvokeScript (called from NavigationCompleted) runs only after the page has already
        // rendered, so the real subscriber count flashes on screen for a frame before the hiding
        // style lands. Registering the same script as a WebView2 "document created" script makes it
        // run before YouTube Studio's own content is parsed, so the CSS is already in place at first
        // paint. This needs the raw ICoreWebView2 COM pointer, which Avalonia's WebView control only
        // exposes via TryGetPlatformHandle() - not through its cross-platform API surface.
        public static async Task RegisterDocumentCreatedHideScriptAsync(NativeWebView webView)
        {
            if (webView.TryGetPlatformHandle() is not IWindowsWebView2PlatformHandle handle)
                return;

            var coreWebView2 = CoreWebView2.CreateFromComICoreWebView2(handle.CoreWebView2);
            await coreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(HideSubscriberCountScript.Value);
        }

        private static string LoadHideSubscriberCountScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .First(name => name.EndsWith("hide-subscribers.js", StringComparison.Ordinal));
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
