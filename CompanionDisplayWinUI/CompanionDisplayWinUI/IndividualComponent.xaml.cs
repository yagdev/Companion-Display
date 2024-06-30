using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Motherboard;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using static CompanionDisplayWinUI.WidgetSensors;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualComponent : Page
    {
        public IndividualComponent()
        {
            this.InitializeComponent();
        }
        public bool LoadFinished = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!LoadFinished)
            {
                try
                {
                    var parent = this.Parent as Frame;
                    ComponentType.Text = parent.Name;
                    Hardware hardware = parent.Tag as Hardware;
                    try
                    {
                        Motherboard motherboard = parent.Tag as Motherboard;
                        foreach (IHardware subhardware in motherboard.SubHardware)
                        {
                            foreach (ISensor sensor in subhardware.Sensors)
                            {
                                Frame frame = new();
                                frame.Tag = sensor;
                                frame.Name = sensor.Name;
                                switch (sensor.SensorType)
                                {
                                    case SensorType.Load:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(LoadSensors));
                                        break;
                                    case SensorType.Clock:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(ClockSensor));
                                        break;
                                    case SensorType.Power:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(PowerSensor));
                                        break;
                                    case SensorType.SmallData:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(SmallDataSensor));
                                        break;
                                    case SensorType.Factor:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(FactorSensor));
                                        break;
                                    case SensorType.Fan:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(FanSensor));
                                        break;
                                    case SensorType.Temperature:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(TemperatureSensor));
                                        break;
                                    case SensorType.Voltage:
                                        ComponentSensorStack.Children.Add(frame);
                                        frame.Navigate(typeof(VoltageSensor));
                                        break;

                                }
                            }
                        }
                    }
                    catch { }
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        Frame frame = new();
                        frame.Tag = sensor;
                        frame.Name = sensor.Name;
                        switch (sensor.SensorType)
                        {
                            case SensorType.Load:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(LoadSensors));
                                break;
                            case SensorType.Clock:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(ClockSensor));
                                break;
                            case SensorType.Power:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(PowerSensor));
                                break;
                            case SensorType.SmallData:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(SmallDataSensor));
                                break;
                            case SensorType.Factor:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(FactorSensor));
                                break;
                            case SensorType.Fan:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(FanSensor));
                                break;
                            case SensorType.Temperature:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(TemperatureSensor));
                                break;
                            case SensorType.Voltage:
                                ComponentSensorStack.Children.Add(frame);
                                frame.Navigate(typeof(VoltageSensor));
                                break;

                        }
                    }
                }
                catch { }
                LoadFinished = true;
            }
        }
    }
}
