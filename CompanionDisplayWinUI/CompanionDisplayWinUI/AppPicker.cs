using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanionDisplayWinUI
{
    internal class AppPicker
    {
        public string SetIcon()
        {
            var window = (Microsoft.UI.Xaml.Application.Current as App)?.m_window as MainWindow;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                DereferenceLinks = false
            };
            openFileDialog1.InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
            string filename = openFileDialog1.FileName;
            return filename;
        }
        public string SetIcon2()
        {
            var window = (Microsoft.UI.Xaml.Application.Current as App)?.m_window as MainWindow;
            FolderBrowserDialog openFileDialog1 = new FolderBrowserDialog();
            openFileDialog1.InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
            string filename = openFileDialog1.SelectedPath;
            return filename;
        }
        public void LaunchApp(string ButtonName)
        {
            if (File.Exists(ButtonName + ".txt"))
            {
                string text = File.ReadAllText(ButtonName + ".txt");
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C \"" + text + "\"";
                process.StartInfo = startInfo;
                process.Start();
            }
        }
    }
}
