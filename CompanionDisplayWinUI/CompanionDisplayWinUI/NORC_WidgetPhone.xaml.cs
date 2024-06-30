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
using System.Threading;
using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetPhone : Page
    {
        int i = 0;
        string LastID;
        double Brightness1, Brightness2;
        public NORC_WidgetPhone()
        {
            this.InitializeComponent();
        }
        private void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Remove Widget", Name = senderElement.Name + "Flyout" };
            MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "Refresh", Name = senderElement.Name + "Edit" };
            firstItem.Click += MenuFlyoutItem_Click;
            secondItem.Click += MenuFlyoutEdit_Click;
            myFlyout.Items.Add(firstItem);
            myFlyout.Items.Add(secondItem);
            myFlyout.ShowAt(senderElement, new Point(0, 0));
        }
        private async void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Clear();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.IsEnabled = false;
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }

        public void UpdateUI()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Process process = new();
                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                };
                process.StartInfo.Arguments = "/C runtimes\\adb.exe devices";
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd().Replace("List of devices attached", "").Replace("\tdevice", "");
                foreach (string line in output.Split('\n'))
                {
                    try
                    {
                        string fix = line.Replace("\r", "");
                        if (fix.Length != 0)
                        {
                            LastID = fix;
                            Frame frame = new()
                            {
                                Name = fix,
                                Width = 240,
                            };
                            BasicGridView.Items.Add(frame);
                            frame.Navigate(typeof(WidgetPhoneIndividual));
                        }
                    }
                    catch { }
                }
                if(BasicGridView.Items.Count == 0)
                {
                    NoDevices.Visibility = Visibility.Visible;
                }
                else
                {
                    if(BasicGridView.Items.Count == 1)
                    {
                        var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(LastID);
                        childControl.Width = 486;
                    }
                    NoDevices.Visibility = Visibility.Collapsed;
                }
            });
        }
    }
}