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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BTBatteryIndividual : Page
    {
        public BTBatteryIndividual()
        {
            this.InitializeComponent();
        }
        private string Name;
        private bool FTU = true;
        private void UpdateUI()
        {
            string script = @"Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Bypass -Force;
$BTDeviceFriendlyName = """ + Name + @"""
$Shell = New-Object -ComObject ""WScript.Shell""
$BTHDevices = Get-PnpDevice -FriendlyName ""*$($BTDeviceFriendlyName)*""
$BatteryLevels = foreach ($Device in $BTHDevices) 
{
        $BatteryProperty = Get-PnpDeviceProperty -InstanceId $Device.InstanceId -KeyName '{104EA319-6EE2-4701-BD47-8DDBF425BBE5} 2' |
            Where-Object { $_.Type -ne 'Empty' } |
            Select-Object -ExpandProperty Data

        if ($BatteryProperty) {
            $BatteryProperty
        }
    }
$Message = ""$BatteryLevels%""
$Message
";
            try
            {
                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.AddScript(script);
                    Collection<PSObject> results = powerShell.Invoke();
                    foreach (var result in results)
                    {
                        int DevBattery = int.Parse(result.ToString().Replace("%", ""));
                        string DevBatteryIcon = "%";
                        switch (DevBattery)
                        {
                            case >= 0 and < 10:
                                DevBatteryIcon = "\ue850";
                                break;
                            case >= 10 and < 20:
                                DevBatteryIcon = "\ue851";
                                break;
                            case >= 20 and < 30:
                                DevBatteryIcon = "\ue852";
                                break;
                            case >= 30 and < 40:
                                DevBatteryIcon = "\ue853";
                                break;
                            case >= 40 and < 50:
                                DevBatteryIcon = "\ue854";
                                break;
                            case >= 50 and < 60:
                                DevBatteryIcon = "\ue855";
                                break;
                            case >= 60 and < 70:
                                DevBatteryIcon = "\ue856";
                                break;
                            case >= 70 and < 80:
                                DevBatteryIcon = "\ue857";
                                break;
                            case >= 80 and < 90:
                                DevBatteryIcon = "\ue858";
                                break;
                            case >= 90 and < 100:
                                DevBatteryIcon = "\ue859";
                                break;
                            case 100:
                                DevBatteryIcon = "\ue83f";
                                break;
                        }
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Percentage.Text = result.ToString();
                            BattIcon.Text = DevBatteryIcon;
                        });
                    }
                }
            }
            catch
            {
            }
            Thread.Sleep(10000);
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private void PercentageChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            string percentage = reader.ReadByte().ToString();
            int DevBattery = int.Parse(percentage);
            string DevBatteryIcon = "%";
            switch (DevBattery)
            {
                case >= 0 and < 10:
                    DevBatteryIcon = "\ue850";
                    break;
                case >= 10 and < 20:
                    DevBatteryIcon = "\ue851";
                    break;
                case >= 20 and < 30:
                    DevBatteryIcon = "\ue852";
                    break;
                case >= 30 and < 40:
                    DevBatteryIcon = "\ue853";
                    break;
                case >= 40 and < 50:
                    DevBatteryIcon = "\ue854";
                    break;
                case >= 50 and < 60:
                    DevBatteryIcon = "\ue855";
                    break;
                case >= 60 and < 70:
                    DevBatteryIcon = "\ue856";
                    break;
                case >= 70 and < 80:
                    DevBatteryIcon = "\ue857";
                    break;
                case >= 80 and < 90:
                    DevBatteryIcon = "\ue858";
                    break;
                case >= 90 and < 100:
                    DevBatteryIcon = "\ue859";
                    break;
                case 100:
                    DevBatteryIcon = "\ue83f";
                    break;
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                BattIcon.Text = DevBatteryIcon;
                Percentage.Text = percentage + "%";
            });
        }
        BluetoothLEDevice device;
        GattCharacteristic batteryLevelCharacteristic;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                Name = (this.Parent as Frame).Name;
                DevName.Text = Name;
                if ((this.Parent as Frame).Tag as BluetoothLEDevice != null)
                {
                    device = (this.Parent as Frame).Tag as BluetoothLEDevice;
                    DevName.Text = device.Name;
                    var batteryService = device.GetGattServicesForUuidAsync(GattServiceUuids.Battery).GetResults();
                    var service = batteryService.Services.FirstOrDefault();
                    // Get the Battery Level Characteristic
                    var characteristics = await service.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);
                    batteryLevelCharacteristic = characteristics.Characteristics.FirstOrDefault();
                    // Read the Battery Level
                    var result = await batteryLevelCharacteristic.ReadValueAsync();
                    batteryLevelCharacteristic.ValueChanged -= PercentageChanged;
                    batteryLevelCharacteristic.ValueChanged += PercentageChanged;
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var reader = DataReader.FromBuffer(result.Value);
                        string percentage = reader.ReadByte().ToString();
                        Percentage.Text = percentage + "%";
                        int DevBattery = int.Parse(percentage);
                        string DevBatteryIcon = "%";
                        switch (DevBattery)
                        {
                            case >= 0 and < 10:
                                DevBatteryIcon = "\ue850";
                                break;
                            case >= 10 and < 20:
                                DevBatteryIcon = "\ue851";
                                break;
                            case >= 20 and < 30:
                                DevBatteryIcon = "\ue852";
                                break;
                            case >= 30 and < 40:
                                DevBatteryIcon = "\ue853";
                                break;
                            case >= 40 and < 50:
                                DevBatteryIcon = "\ue854";
                                break;
                            case >= 50 and < 60:
                                DevBatteryIcon = "\ue855";
                                break;
                            case >= 60 and < 70:
                                DevBatteryIcon = "\ue856";
                                break;
                            case >= 70 and < 80:
                                DevBatteryIcon = "\ue857";
                                break;
                            case >= 80 and < 90:
                                DevBatteryIcon = "\ue858";
                                break;
                            case >= 90 and < 100:
                                DevBatteryIcon = "\ue859";
                                break;
                            case 100:
                                DevBatteryIcon = "\ue83f";
                                break;
                        }
                        BattIcon.Text = DevBatteryIcon;
                    }
                    else
                    {
                        ((this.Parent as Frame).Parent as StackPanel).Children.Remove((this.Parent) as Frame);
                    }
                }
                else
                {
                    Thread thread = new(UpdateUI);
                    thread.Start();
                }
                FTU = false;
            }
            
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                batteryLevelCharacteristic.ValueChanged -= PercentageChanged;
            }
            catch { }
        }
    }
}
