using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
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
    public sealed partial class MediaPlayerWidgetWidgetSettings : Page
    {
        public MediaPlayerWidgetWidgetSettings()
        {
            this.InitializeComponent();
            SPDC.Text = Globals.SP_DC;
            CID.Text = Globals._clientId;
            CS.Text = Globals._secretId;
            CID2.Text = Globals._clientId2;
            CS2.Text = Globals._secretId2;
            DiscordAppID.Text = Globals.DiscordID;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(MediaPlayerWidget));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.MediaConfigFile));
            File.Delete(Globals.MediaConfigFile);
            File.AppendAllText(Globals.MediaConfigFile, SPDC.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CID.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CS.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CID2.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CS2.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, DiscordAppID.Text);
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(MediaPlayerWidget));
        }
    }
}
