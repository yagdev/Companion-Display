using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpotifyPlayer : Page
    {
        public SpotifyPlayer()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private bool FTU = true;
        private async void SpotifyBuiltin_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var environmentOptions = new CoreWebView2EnvironmentOptions();
                environmentOptions.AreBrowserExtensionsEnabled = true;
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync("", "", environmentOptions);
                await SpotifyBuiltin.EnsureCoreWebView2Async(environment);
                await SpotifyBuiltin.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.59.0_0"));
                SpotifyBuiltin.Source = new Uri("https://open.spotify.com/");
                FTU = false;
            }
        }
    }
}
