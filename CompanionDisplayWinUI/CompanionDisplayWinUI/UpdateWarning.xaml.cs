using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateWarning : Page
    {
        public UpdateWarning()
        {
            this.InitializeComponent();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private void UpdateUI()
        {
            WebClient client = new WebClient();
            string reply = client.DownloadString("https://www.dropbox.com/scl/fi/f8nu0pn6leiz646o1ebce/changelog.txt?rlkey=fd6srkx7xm3wexk1eevglgs8c&st=mtnphh0r&dl=1");
            string version = client.DownloadString(Globals.UpdateString);
            DispatcherQueue.TryEnqueue(() =>
            {
                VersionName.Text = "Update " + version;
                ChangelogBlock.Text = reply;
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                UpdateBtn.IsEnabled = false;
                UpdateBtn.Content = "Updating...";
                client.DownloadFile(Globals.UpdateZip, "release.zip");
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.Arguments = "/C mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move release.zip ../ & cd .. & tar -xf release.zip & del /f /q release.zip & taskkill /f /im CompanionDisplayWinUI.exe & timeout 1 & rmdir /s /q Update & CompanionDisplayWinUI.exe";
                cmd.Start();
            }
        }
    }
}
