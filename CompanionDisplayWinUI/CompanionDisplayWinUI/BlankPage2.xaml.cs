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
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.WebUI;

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
            this.NavigationCacheMode = NavigationCacheMode.Required;
            if (this.Tag as string == "Unload")
            {
                WebView.Close();
            }
            if (Globals.LastWebPage != "")
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
                else if (AddressBar.Text.Length > 3 && AddressBar.Text.Remove(0, 1).Contains('.') && AddressBar.Text.Remove(AddressBar.Text.Length - 2, 2).Contains('.') && AddressBar.Text.Contains(' ') == false)
                {
                    WebView.Source = new Uri("https://" + AddressBar.Text);
                }
                else
                {
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
                    WebView.Margin = new Thickness(0, -80, 0, 0);
                }
                else
                {
                    WebView.Margin = new Thickness(0, 0, 0, 0);
                }
            }
        }
        private void WebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            
            try
            {
                frame = this.Parent as Frame;
                (this.Parent as TabViewItem).Header = WebView.CoreWebView2.DocumentTitle.ToString();
                (this.Parent as TabViewItem).IconSource = new BitmapIconSource
                {
                    UriSource = new Uri(WebView.CoreWebView2.FaviconUri),
                    ShowAsMonochrome = false,
                };
            }
            catch { }
            WebView.CoreWebView2.ContainsFullScreenElementChanged += (obj, args) =>
            {
                this.FullScreen = WebView.CoreWebView2.ContainsFullScreenElement;
            };
            try
            {
                AddressBar.Text = WebView.Source.ToString();
                if (AddressBar.FocusState == FocusState.Pointer)
                {
                    AddressBar.SelectAll();
                }
                Globals.LastWebPage = WebView.Source.ToString();
            }
            catch { }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(this.Tag as string == "Unload")
            {
                WebView.Close();
            }
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
            AddressBar.Focus(FocusState.Programmatic);
        }
        private bool FTU = true;
        public void ClearBrowser()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WebView.Close();
            });
            
        }
        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var environmentOptions = new CoreWebView2EnvironmentOptions
                {
                    AreBrowserExtensionsEnabled = true
                };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync("", userDataFolder: null, environmentOptions);
                await WebView.EnsureCoreWebView2Async(environment);
                await WebView.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.57.2_0"));
                WebView.Source = new Uri("https://www.google.com/");
                WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                FTU = false;
            }
        }
        public CoreWebView2 ReturnNewCoreWebView2Tab()
        {
            //WebView.EnsureCoreWebView2Async();
            return WebView.CoreWebView2;
        }
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = WebView.CoreWebView2;
        }
        private void Page_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    WebView.Close();
                }
                catch { }
            });
        }

        private void AddressBar_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressBar.SelectAll();
        }

        private void WebView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
        }
    }
}
