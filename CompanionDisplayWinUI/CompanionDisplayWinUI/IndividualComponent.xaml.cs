using CompanionDisplayWinUI.ClassImplementations;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Motherboard;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

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
            this.NavigationCacheMode = NavigationCacheMode.Required;
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
                                Frame frame = new()
                                {
                                    Tag = sensor,
                                    Name = sensor.Name
                                };
                                AddSensors(sensor, frame);
                            }
                        }
                        if (ComponentSensorStack.Children.Count == 1)
                        {
                            NoSensorsWarning.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            NoSensorsWarning.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch { }
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        Frame frame = new()
                        {
                            Tag = sensor,
                            Name = sensor.Name
                        };
                        AddSensors(sensor, frame);
                    }
                    if(ComponentSensorStack.Children.Count == 1)
                    {
                        NoSensorsWarning.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        NoSensorsWarning.Visibility = Visibility.Collapsed;
                    }
                }
                catch { }
                LoadFinished = true;
            }
        }
        private void AddSensors(ISensor sensor, Frame frame)
        {
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
        private void ImageOptionalBlur_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.Backdrop <= 1)
            {
                (sender as Rectangle).Fill = null;
                if (ThemingAndColors.GetTheme() == ElementTheme.Dark)
                {
                    (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(255, 33, 33, 33));
                }
                else
                {
                    (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(255, 212, 212, 212));
                }
            }
            else
            {
                (sender as Rectangle).Fill = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
            }
        }
    }
}
