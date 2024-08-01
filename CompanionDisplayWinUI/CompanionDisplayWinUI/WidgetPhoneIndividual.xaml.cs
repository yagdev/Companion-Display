using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhoneIndividual : Page
    {
        double Brightness1;
        string ID;
        int SliderInt = 0;
        public WidgetPhoneIndividual()
        {
            this.InitializeComponent();
        }
        public bool CleanUp = false;
        private double LastLvl = -1, LastBrightness = -1;
        private string LastName = "-";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                }
            })
            {
                process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " reboot";
                process.Start();
            }
        }
        private void UpdateUI()
        {
            string DevName = PhoneADB.GetDeviceName(ID);
            int DevBattery = PhoneADB.GetDeviceBattery(ID);
            double DevBrightness = PhoneADB.GetDeviceBrightness(ID);
            if (DevBattery != LastLvl)
            {
                string DevBatteryIcon = DevBattery.ToString() + "%";
                switch (DevBattery)
                {
                    case >= 0 and < 10:
                        DevBatteryIcon = "\ue85a";
                        break;
                    case >= 10 and < 20:
                        DevBatteryIcon = "\ue85b";
                        break;
                    case >= 20 and < 30:
                        DevBatteryIcon = "\ue85c";
                        break;
                    case >= 30 and < 40:
                        DevBatteryIcon = "\ue85d";
                        break;
                    case >= 40 and < 50:
                        DevBatteryIcon = "\ue85e";
                        break;
                    case >= 50 and < 60:
                        DevBatteryIcon = "\ue85f";
                        break;
                    case >= 60 and < 70:
                        DevBatteryIcon = "\ue860";
                        break;
                    case >= 70 and < 80:
                        DevBatteryIcon = "\ue861";
                        break;
                    case >= 80 and < 90:
                        DevBatteryIcon = "\ue862";
                        break;
                    case >= 90 and < 100:
                        DevBatteryIcon = "\ue83e";
                        break;
                    case 100:
                        DevBatteryIcon = "\uea93";
                        break;
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    BatteryIcon.Text = DevBatteryIcon;
                    Device1BatteryInt.Text = DevBattery.ToString() + "%";
                });
                LastLvl = DevBattery;
            }
            if(DevBrightness != LastBrightness)
            {
                LastBrightness = DevBrightness;
                DispatcherQueue.TryEnqueue(() =>
                {
                    Device1Brightness.Value = DevBrightness;
                });
            }            
            if(DevName != LastName)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Device1Name.Text = DevName;
                });
                LastName = DevName;
            }
            if (CleanUp == false)
            {
                Thread.Sleep(1000);
                Thread thread0 = new(UpdateUI);
                thread0.Start();
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                }
            })
            {
                process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell input keyevent 26";
                process.Start();
            }
        }
        private void Device1Brightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Brightness1 = Device1Brightness.Value;
            Thread thread = new(ChangeBrightness1);
            thread.Start();
        }
        private void ChangeBrightness1()
        {
        start:
            if (SliderInt == 0)
            {
                SliderInt = 1;
                using (Process process = new()
                {
                    StartInfo = new()
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "CMD.exe",
                        CreateNoWindow = true,
                        Arguments = "/C runtimes\\adb.exe -s " + ID + " shell settings put system screen_brightness " + Brightness1
                    }
                })
                {
                    process.Start();
                    process.WaitForExit();
                    SliderInt = 0;
                }
            }
            else
            {
                goto start;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Frame;
            ID = parent.Name;
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
    }
}
