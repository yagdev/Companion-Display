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
    public sealed partial class ClockSensor : Page
    {
        public ClockSensor()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            var parent = this.Parent as Frame;
            if (parent != null)
            {
                SensorName.Text = parent.Name;
                var sensor = parent.Tag as ISensor;
                sensor1 = sensor;
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        ISensor sensor1;
        public double LoadValue, LoadMin, LoadMax;
        public bool CleanUp = false;
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }

        public string LoadValue2;
        private void UpdateUI()
        {
            if (sensor1 != null)
            {
                string query = "";
                try
                {
                    query = sensor1.Hardware.Parent.Name;
                }
                catch
                {

                }
                if (Globals.CurrentHW == sensor1.Hardware.Name || Globals.CurrentHW == query)
                {
                    try
                    {
                        LoadValue = (double)sensor1.Value;
                        LoadValue2 = (Math.Round((float)sensor1.Value)) + " MHz";
                        LoadMin = (double)sensor1.Min;
                        LoadMax = (double)sensor1.Max;
                    }
                    catch { }
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        LoadPercent.Text = LoadValue2;
                    });
                }
                Thread.Sleep(3000);
            }
            if (CleanUp == false)
            {
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
    }
}
