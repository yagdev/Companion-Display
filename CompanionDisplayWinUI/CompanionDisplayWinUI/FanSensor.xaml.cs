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
    public sealed partial class FanSensor : Page
    {
        public FanSensor()
        {
            this.InitializeComponent();
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            var parent = this.Parent as Frame;
            if (parent != null)
            {
                if (FTU)
                {
                    FTU = false;
                    SensorName.Text = parent.Name;
                    var sensor = parent.Tag as ISensor;
                    sensor1 = sensor;
                }
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        ISensor sensor1;
        public string LoadValue2;
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
        public bool CleanUp = false;
        private double LastValue = -1;
        private void UpdateUI()
        {
            if (sensor1 != null)
            {
                string query = "";
                try
                {
                    try
                    {
                        query = sensor1.Hardware.Parent.Name;
                    }
                    catch { }
                    if ((Globals.CurrentHW == sensor1.Hardware || Globals.CurrentHW == sensor1.Hardware.Parent) && Math.Round((float)sensor1.Value) != LastValue)
                    {
                        LastValue = Math.Round((float)sensor1.Value);
                        try
                        {
                            LoadValue2 = (Math.Round((float)sensor1.Value)) + " RPM";
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
