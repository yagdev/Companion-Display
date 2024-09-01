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
        private double LastValue = -1;
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var parent = this.Parent as Frame;
                if (parent != null)
                {
                    FTU = false;
                    SensorName.Text = parent.Name;
                    var sensor = parent.Tag as ISensor;
                    sensor1 = sensor;
                }
            }
            CleanUp = false;
            Thread thread = new(UpdateUI);
            thread.Start();

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
                    if (sensor1.Hardware.Parent != null)
                    {
                        query = sensor1.Hardware.Parent.Name;
                    }
                    if ((Globals.CurrentHW == sensor1.Hardware || Globals.CurrentHW == sensor1.Hardware.Parent) && Math.Round((float)sensor1.Value) != LastValue)
                    {
                        LastValue = Math.Round((float)sensor1.Value);
                        try
                        {
                            LoadValue2 = (Math.Round((float)sensor1.Value)) + " MHz";
                        }
                        catch { }
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            LoadPercent.Text = LoadValue2;
                        });
                    }
                }
                catch
                {

                }
            }
            if (CleanUp == false)
            {
                Thread.Sleep(3000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
    }
}
