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
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage2 : Page
    {
        public Frame frame;
        public BlankPage2()
        {
            this.InitializeComponent();
            if(Globals.LastWebPage != "")
            {
                WebView.Source = new Uri(Globals.LastWebPage);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WebView.GoForward();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WebView.Source = new Uri("https://www.google.com/");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (WebView.Source != new Uri("edge://downloads/all"))
            {
                WebView.Source = new Uri("edge://downloads/all");
            }
            else
            {
                WebView.GoBack();
            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (WebView.Source != new Uri("edge://history/all"))
            {
                WebView.Source = new Uri("edge://history/all");
            }
            else
            {
                WebView.GoBack();
            }
        }

        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (AddressBar.Text.Length > 3 && AddressBar.Text.Remove(0, 1).Contains("://"))
                {
                    WebView.Source = new Uri(AddressBar.Text);
                }
                else if (AddressBar.Text.Length > 3 && AddressBar.Text.Remove(0, 1).Contains(".") && AddressBar.Text.Remove(AddressBar.Text.Length - 2, 2).Contains(".") && AddressBar.Text.Contains(" ") == false)
                {
                    WebView.Source = new Uri("https://" + AddressBar.Text);
                }
                else
                {
                    string search = AddressBar.Text;
                    search.Replace(" ", "+");
                    WebView.Source = new Uri("https://www.google.com/search?q=" + AddressBar.Text.Replace(" ", "+"));
                }
                WebView.Focus(FocusState.Programmatic);
            }

        }
        private bool fullScreen = false;
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                fullScreen = value;
                if (fullScreen == true)
                {
                    ControlsRow.Height = new GridLength(0, GridUnitType.Pixel);
                    frame.Margin = new Thickness(0, 0, 0, 0);

                }
                else
                {
                    ControlsRow.Height = new GridLength(90, GridUnitType.Pixel);
                    frame.Margin = new Thickness(0, 45, 0, 0);
                }
            }
        }
        private void WebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            frame = this.Parent as Frame;
            WebView.CoreWebView2.ContainsFullScreenElementChanged += (obj, args) =>
            {
                this.FullScreen = WebView.CoreWebView2.ContainsFullScreenElement;
            };
            try
            {
                AddressBar.Text = WebView.Source.ToString();
                Globals.LastWebPage = WebView.Source.ToString();
            }
            catch { }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            WebView.Close();
            try
            {
                frame.Margin = new Thickness(0, 45, 0, 0);
            }
            catch
            {

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            var environmentOptions = new CoreWebView2EnvironmentOptions();
            environmentOptions.AreBrowserExtensionsEnabled = true;
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync("", userDataFolder: null, environmentOptions);
            await WebView.EnsureCoreWebView2Async(environment);
            WebView.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.57.2_0")); 
            WebView.Source = new Uri("https://www.google.com/");
        }
    }
}
