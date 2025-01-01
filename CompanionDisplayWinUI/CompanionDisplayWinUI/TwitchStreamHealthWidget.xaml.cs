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
    public sealed partial class TwitchStreamHealthWidget : Page
    {
        public TwitchStreamHealthWidget()
        {
            this.InitializeComponent();
        }

        private bool FTU = true;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var environmentOptions = new CoreWebView2EnvironmentOptions();
                environmentOptions.AreBrowserExtensionsEnabled = true;
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync("", "", environmentOptions);
                await Player.EnsureCoreWebView2Async(environment);
                Player.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.59.0_0"));
                FTU = false;
            }
            Player.Source = new Uri("https://dashboard.twitch.tv/popout/stream-manager/stream-health");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopOutPlayer m_window = new PopOutPlayer(Player.Source);
            m_window.Activate();
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Player.CoreWebView2.Reload();
        }
    }
}
