using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupStep1 : Page
    {
        public SetupStep1()
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
            Globals.SP_DC = SPDC_Field.Text;
            Globals._clientId = SPCli_Field.Text;
            Globals._secretId = SPCliSecret_Field.Text;
            Globals._clientId2 = SPCli2_Field.Text;
            Globals._secretId2 = SPCliSecret2_Field.Text;
            Globals.DiscordID = AppID_Field.Text;
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep2), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Globals.MediaConfigFile))
            {
                string config = File.ReadAllText(Globals.MediaConfigFile);
                using StringReader readerconfig = new(config);
                {
                    Globals.SP_DC = readerconfig.ReadLine();
                    Globals._clientId = readerconfig.ReadLine();
                    Globals._secretId = readerconfig.ReadLine();
                    Globals._clientId2 = readerconfig.ReadLine();
                    Globals._secretId2 = readerconfig.ReadLine();
                    Globals.DiscordID = readerconfig.ReadLine();
                }
            }
            if (File.Exists(Globals.RefreshTokenPath))
            {
                File.Delete(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                File.Delete(Globals.RefreshToken2Path);
            }
            SPDC_Field.Text = Globals.SP_DC;
            SPCli_Field.Text = Globals._clientId;
            SPCliSecret_Field.Text = Globals._secretId;
            SPCli2_Field.Text = Globals._clientId2;
            SPCliSecret2_Field.Text = Globals._secretId2;
            AppID_Field.Text = Globals.DiscordID;
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep2), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/yagdev/Spotify-Lyrics-For-Discord/wiki/User-Guides#" + (sender as Button).Tag;
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}
