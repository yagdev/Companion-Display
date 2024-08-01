using CoreAudio;
using EmbedIO.Sessions;
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
    public sealed partial class AudioDevice : Page
    {
        public AudioDevice()
        {
            this.InitializeComponent();
        }
        private int LastCount = 0, IsVisible = 0;
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsVisible = 1;
            device = (this.Parent as Frame).Tag as MMDevice;
            if (FTU)
            {
                FTU = false;
                Thread thread = new(UpdateUI);
                thread.Start();
            }
            Thread thread0 = new(Count);
            thread0.Start();
        }
        private void UpdateUI()
        {
            try
            {
                using (device)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SessionController.Children.Clear();
                        Frame frame0 = new()
                        {
                            Tag = device,
                            Name = "GLOBAL"
                        };
                        frame0.Navigate(typeof(IndividualAudioControl));
                        SessionController.Children.Add(frame0);
                        LastCount = 0;
                        foreach (var session in device.AudioSessionManager2.Sessions)
                        {
                            Frame frame = new()
                            {
                                Tag = session
                            };
                            frame.Navigate(typeof(IndividualAudioControl));
                            SessionController.Children.Add(frame);
                            LastCount++;
                        }
                    });
                }
            }
            catch
            {
                LastCount = -1;
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsVisible = 0;
        }
        private MMDevice device;
        private void Count()
        {
            try
            {
                using (device)
                {
                    device.AudioSessionManager2.RefreshSessions();
                    if (device.AudioSessionManager2.Sessions.Count != LastCount || LastCount == -1)
                    {
                        Thread thread = new(UpdateUI);
                        thread.Start();
                    }
                }
            }
            catch
            {

            }
            if(IsVisible == 1)
            {
                Thread.Sleep(1000);
                Thread thread1 = new(Count);
                thread1.Start();
            }
            
        }
    }
}
