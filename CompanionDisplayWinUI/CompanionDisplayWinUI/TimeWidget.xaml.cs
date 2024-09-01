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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Radios;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Control;
using Windows.Networking.Connectivity;
using Windows.Networking.NetworkOperators;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using static CompanionDisplayWinUI.MediaPlayerWidget;
using CoreAudio;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeWidget : Page
    {
        public TimeWidget()
        {
            this.InitializeComponent();
        }
        public bool CleanUp = false;
        private string DateStr = "", TimeStr = "", SongTitleCache = "";
        private void UpdateUI()
        {
            try
            {
                if(DateStr != DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy"))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Date.Text = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                        FullDate.Text = DateOnly.FromDateTime(DateTime.Now).ToString("dddd, dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    });
                    DateStr = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                }
                if(TimeStr != TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm"))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Time.Text = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                        TimeLeft.Text = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                    });
                    TimeStr = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                }
                try
                {
                    if ((Globals.SongName != SongTitleCache && Globals.IsSpotify) || (!Globals.IsSpotify && Globals.songInfo.Title != SongTitleCache))
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            try
                            {
                                if (Globals.IsSpotify)
                                {
                                    SongTitle.Text = Globals.SongName;
                                    Album.Source = new BitmapImage(new Uri(Globals.SongBackground));
                                    Album2.Source = new BitmapImage(new Uri(Globals.SongBackground));
                                }
                                else
                                {
                                    SongTitle.Text = Globals.songInfo.Title;
                                    Album.Source = (ImageSource)Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                                    Album2.Source = Album.Source;
                                }
                                SongTitleCache = Globals.SongName;
                            }
                            catch
                            {
                                SongTitle.Text = "No media playing";
                                SongTitleCache = "";
                            }
                        });
                    }
                }
                catch
                {
                }
                try
                {
                    if (Globals.sessionManager.GetCurrentSession().GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PlayPause.Content = "\ue768";
                        });
                    }
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PlayPause.Content = "\ue769";
                        });
                    }
                }
                catch
                {
                }

            }
            catch
            {

            }
            if (CleanUp == false)
            {
                Thread.Sleep(1000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
        private bool FTU = true;
        public async Task<bool> IsBluetoothOn()
        {
            await Radio.RequestAccessAsync();
            var radios = await Radio.GetRadiosAsync();
            foreach (var radio in radios)
            {
                if (radio.Kind == RadioKind.Bluetooth && radio.State == RadioState.On)
                {
                    return true;
                }
            }
            return false;
        }
        private Radio radio1, radio2;
        private async void BTToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (BTToggle.IsChecked == true)
                {
                    await radio2.SetStateAsync(RadioState.On);
                }
                else
                {
                    await radio2.SetStateAsync(RadioState.Off);
                }
            }
            catch
            {

            }
            
        }

        private async void WiFiToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (WiFiToggle.IsChecked == true)
                {
                    await radio1.SetStateAsync(RadioState.On);
                }
                else
                {
                    await radio1.SetStateAsync(RadioState.Off);
                }
            }
            catch { }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CleanUp = false;
                string config = File.ReadAllText(Globals.TimeConfigFile);
                if (config != "")
                {
                    using StringReader readerconfig = new(config);
                    int RTime = int.Parse(readerconfig.ReadLine());
                    int GTime = int.Parse(readerconfig.ReadLine());
                    int BTime = int.Parse(readerconfig.ReadLine());
                    int RDate = int.Parse(readerconfig.ReadLine());
                    int GDate = int.Parse(readerconfig.ReadLine());
                    int BDate = int.Parse(readerconfig.ReadLine());
                    Time.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RTime, (byte)GTime, (byte)BTime));
                    Date.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RDate, (byte)GDate, (byte)BDate));
                }
            }
            catch
            {

            }
            try
            {
                if (FTU)
                {
                    FTU = false;
                    await Radio.RequestAccessAsync();
                    var radios = await Radio.GetRadiosAsync();
                    foreach (var radio in radios)
                    {
                        if (radio.Kind == RadioKind.WiFi)
                        {
                            WiFiToggle.IsEnabled = true;
                            WiFiToggle.IsHitTestVisible = true;
                            radio1 = radio;
                            radio1.StateChanged -= UpdateWiFi;
                            radio1.StateChanged += UpdateWiFi;
                            if (radio.State == RadioState.On)
                            {
                                WiFiToggle.IsChecked = true;
                            }
                        }
                        if (radio.Kind == RadioKind.Bluetooth)
                        {
                            BTToggle.IsEnabled = true;
                            BTToggle.IsHitTestVisible = true;
                            radio2 = radio;
                            radio2.StateChanged -= UpdateBT;
                            radio2.StateChanged += UpdateBT;
                            if (radio.State == RadioState.On)
                            {
                                BTToggle.IsChecked = true;
                            }
                        }
                        uiSettings = new Windows.UI.ViewManagement.UISettings();
                        var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
                        uiSettings.ColorValuesChanged += UpdateDarkMode;
                        if(color.R == 0)
                        {
                            DarkModeToggle.IsChecked = true;
                        }
                        MMDeviceEnumerator DevEnum = new();
                        mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                        mDevice.AudioEndpointVolume.OnVolumeNotification += ChangeVol;
                        MuteToggle.IsChecked = mDevice.AudioEndpointVolume.Mute;
                    }
                    using (Process process = new()
                    {
                        StartInfo = new()
                        {
                            FileName = "powershell.exe",
                            Arguments = $"-Command \"(Get-ItemProperty -Path \"HKLM:\\System\\CurrentControlSet\\Control\\RadioManagement\\SystemRadioState\").'(default)'",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                        }
                    })
                    {
                        process.Start();
                        process.WaitForExit();
                        if (process.StandardOutput.ReadToEnd().Contains('1'))
                        {
                             AirPlaneToggle.IsChecked = true;
                        }
                    }
                    UpdateBL();
                }
            }
            catch
            {

            }
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private async void UpdateBL()
        {
            try
            {
                bluetoothDevices.Clear();
                bluetoothDevicesLE.Clear();
                DispatcherQueue.TryEnqueue(() =>
                {
                    BTStackBattery.Children.Clear();
                });
                var selector = BluetoothDevice.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(selector);
                foreach (var device in devices)
                {
                    BluetoothDevice bleDevice0 = await BluetoothDevice.FromIdAsync(device.Id);
                    bluetoothDevices.Add(bleDevice0);
                    bleDevice0.ConnectionStatusChanged += BLRefresh;
                    if (bleDevice0.ConnectionStatus == BluetoothConnectionStatus.Connected)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Frame frame = new()
                            {
                                Name = device.Name,
                            };
                            frame.Navigate(typeof(BTBatteryIndividual));
                            BTStackBattery.Children.Add(frame);
                        });
                    }
                }
                var selector0 = BluetoothLEDevice.GetDeviceSelector();
                var devices0 = await DeviceInformation.FindAllAsync(selector0);
                foreach (var device in devices0)
                {
                    BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
                    bleDevice.ConnectionStatusChanged += BLERefresh;
                    bluetoothDevicesLE.Add(bleDevice);
                    if (bleDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Frame frame = new()
                            {
                                Name = device.Name,
                                Tag = bleDevice
                            };
                            frame.Navigate(typeof(BTBatteryIndividual));
                            BTStackBattery.Children.Add(frame);
                        });
                    }
                }
            }
            catch
            {

            }
        }

        private void BLRefresh(BluetoothDevice sender, object args)
        {
            foreach(BluetoothDevice bluetoothDevice in bluetoothDevices)
            {
                bluetoothDevice.ConnectionStatusChanged -= BLRefresh;
            }
            foreach (BluetoothLEDevice bluetoothDevice in bluetoothDevicesLE)
            {
                bluetoothDevice.ConnectionStatusChanged -= BLERefresh;
            }
            sender.ConnectionStatusChanged -= BLRefresh;
            UpdateBL();
        }
        private List<BluetoothDevice> bluetoothDevices = new List<BluetoothDevice>();
        private List<BluetoothLEDevice> bluetoothDevicesLE = new List<BluetoothLEDevice>();
        private void BLERefresh(BluetoothLEDevice sender, object args)
        {
            sender.ConnectionStatusChanged -= BLERefresh;
            UpdateBL();
        }

        private DeviceWatcher deviceWatcher;
        private void ChangeVol(AudioVolumeNotificationData data)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                MuteToggle.IsChecked = data.Muted;
            });
        }

        private MMDevice mDevice;
        private UISettings uiSettings;

        private void DarkModeToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(DarkModeToggle.IsChecked == true)
            {
                using (Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"New-ItemProperty -Path HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize -Name AppsUseLightTheme -Value 0 -Type Dword -Force",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                })
                {
                    process.Start();
                }
            }
            else
            {
                using(Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"Remove-ItemProperty -Path HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize -Name AppsUseLightTheme\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                })
                {
                    process.Start();
                }
            }
        }

        private void MuteToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(MuteToggle.IsChecked == true)
            {
                mDevice.AudioEndpointVolume.Mute = true;
            }
            else
            {
                mDevice.AudioEndpointVolume.Mute = false;
            }
        }

        private async void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await Globals.sessionManager.GetCurrentSession().TrySkipNextAsync();
            }
            catch
            {
            }
        }

        private async void PlayPause_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await Globals.sessionManager.GetCurrentSession().TryTogglePlayPauseAsync();
            }
            catch
            {
            }
        }

        private void AirPlaneToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void shutdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = (sender as Button).Name;
            dialog.PrimaryButtonText = (sender as Button).Name;
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "Do you want to " + (sender as Button).Name + " your PC?";
            dialog.Tag = (sender as Button).Tag;
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                using (Process cmd = new())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.Arguments = "/C " + (sender as Button).Tag;
                    cmd.Start();
                }
            }
        }

        private void UpdateDarkMode(UISettings sender, object args)
        {
            if(sender.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).R == 0)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    DarkModeToggle.IsChecked = true;
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    DarkModeToggle.IsChecked = false;
                });
            }
        }

        private void UpdateBT(Radio sender, object args)
        {
            if (sender.State == RadioState.On)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BTToggle.IsChecked = true;
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BTToggle.IsChecked = false;
                });
            }
            using (Process process = new()
            {
                StartInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"(Get-ItemProperty -Path \"HKLM:\\System\\CurrentControlSet\\Control\\RadioManagement\\SystemRadioState\").'(default)'",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            })
            {
                process.Start();
                process.WaitForExit();
                if (process.StandardOutput.ReadToEnd().Contains('1'))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AirPlaneToggle.IsChecked = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AirPlaneToggle.IsChecked = false;
                    });
                }
            }
        }
        private void UpdateWiFi(Radio sender, object args)
        {
            if(sender.State == RadioState.On)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    WiFiToggle.IsChecked = true;
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    WiFiToggle.IsChecked = false;
                });
            }
            using (Process process = new()
            {
                StartInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"(Get-ItemProperty -Path \"HKLM:\\System\\CurrentControlSet\\Control\\RadioManagement\\SystemRadioState\").'(default)'",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            })
            {
                process.Start();
                process.WaitForExit();
                if (process.StandardOutput.ReadToEnd().Contains('1'))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AirPlaneToggle.IsChecked = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AirPlaneToggle.IsChecked = false;
                    });
                }
            }
        }
    }
}
