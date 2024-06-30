using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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
    public sealed partial class WidgetSensors : Page
    {
        public WidgetSensors()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private void UpdateUI()
        {
            HardwareSensorsFunction hardwareSensorsFunction = new HardwareSensorsFunction();
            hardwareSensorsFunction.Init();
        start:
            if (hardwareSensorsFunction.computer.Hardware.Count != 0)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (IHardware hardware in hardwareSensorsFunction.computer.Hardware)
                    {
                        Frame frame = new();
                        frame.Tag = hardware;
                        frame.Name = hardware.Name;
                        FlipViewHW.Items.Add(frame);
                        frame.Navigate(typeof(IndividualComponent));
                    }
                });
            }
            else
            {
                Thread.Sleep(1000);
                goto start;
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                Globals.CurrentHW = (FlipViewHW.SelectedItem as Frame).Name;
            });
        }

        private void FlipViewHW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.CurrentHW = (FlipViewHW.SelectedItem as Frame).Name;
        }
    }
}
