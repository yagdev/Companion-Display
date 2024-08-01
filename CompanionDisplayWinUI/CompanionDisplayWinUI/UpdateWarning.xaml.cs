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
        private async void UpdateUI()
        {
            using(HttpClient client = new())
            {
                string version, reply;
                if (Globals.IsBetaProgram)
                {
                    reply = await client.GetStringAsync("https://www.dropbox.com/scl/fi/hr6tiqeazmkzllsz9y53z/changelog.txt?rlkey=s40umhmz174z7tok6guelylnu&st=a68s40wb&dl=1");
                    version = await client.GetStringAsync(Globals.UpdateStringBeta);
                }
                else
                {
                    reply = await client.GetStringAsync("https://www.dropbox.com/scl/fi/f8nu0pn6leiz646o1ebce/changelog.txt?rlkey=fd6srkx7xm3wexk1eevglgs8c&st=mtnphh0r&dl=1");
                    version = await client.GetStringAsync(Globals.UpdateString);
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    VersionName.Text = "Update " + version;
                    ChangelogBlock.Text = reply;
                });
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new())
            {
                UpdateBtn.IsEnabled = false;
                UpdateBtn.Content = "Updating...";
                if (Globals.IsBetaProgram)
                {
                    using var s = await client.GetStreamAsync(Globals.UpdateZipBeta);
                    using var fs = new FileStream("release.zip", FileMode.CreateNew);
                    await s.CopyToAsync(fs);
                }
                else
                {
                    using var s = await client.GetStreamAsync(Globals.UpdateZip);
                    using var fs = new FileStream("release.zip", FileMode.CreateNew);
                    await s.CopyToAsync(fs);
                }
                using(Process cmd = new())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.Arguments = "/C mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move release.zip ../ & cd .. & tar -xf release.zip & del /f /q release.zip & taskkill /f /im CompanionDisplayWinUI.exe & timeout 1 & rmdir /s /q Update & CompanionDisplayWinUI.exe";
                    cmd.Start();
                }
            }
            
        }
    }
}
