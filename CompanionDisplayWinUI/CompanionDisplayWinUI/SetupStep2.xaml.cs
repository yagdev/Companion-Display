using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WebUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupStep2 : Page
    {
        public SetupStep2()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.GoBack();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsIP = obsIP.Text;
            Globals.obsPass = obsPass.Text;
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep3), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            obsIP.Text = Globals.obsIP;
            obsPass.Text = Globals.obsPass;
        }


        private void TwitchBrowser_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            TwitchBrowser.CoreWebView2.IsMuted = true;
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep3), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
