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
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CoreAudio;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetAudioControl : Page
    {
        public WidgetAudioControl()
        {
            this.InitializeComponent();
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                var device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                foreach (var endpoint in DevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE))
                {
                    MenuFlyoutItem item = new MenuFlyoutItem();
                    item.Text = endpoint.FriendlyName;
                    item.Tag = endpoint;
                    item.Click += MenuFlyoutItem_Click;
                    ListOfDevices.Items.Add(item);
                }
                CurrentDevice.Content = device.FriendlyName;
                DeviceView.Tag = device;
                DeviceView.Navigate(typeof(AudioDevice));
                FTU = false;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = sender as MenuFlyoutItem;
            CurrentDevice.Content = selecteditem.Text;
            DeviceView.Tag = selecteditem.Tag;
            DeviceView.Navigate(typeof(AudioDevice));
        }
    }
}
