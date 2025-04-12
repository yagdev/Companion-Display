using CoreAudio;
using CoreAudio.Interfaces;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

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
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
            device = (this.Parent as Frame).Tag as MMDevice;
            device.AudioSessionManager2.OnSessionCreated += NewSesh;
            if (FTU)
            {
                FTU = false;
                var t = Task.Run(() => UpdateUI());
                t.Wait();
            }
        }
        private void NewSesh(object sender, IAudioSessionControl2 newSession)
        {
            UpdateUI();
        }
        private void UpdateUI()
        {
            try
            {
                device.AudioSessionManager2.RefreshSessions();
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        SessionController.Children.Clear();
                        Frame frame0 = new()
                        {
                            Tag = device,
                            Name = "GLOBAL"
                        };
                        frame0.Navigate(typeof(IndividualAudioControl));
                        SessionController.Children.Add(frame0);
                        if (device.AudioSessionManager2.Sessions != null)
                        {
                            foreach (var session in device.AudioSessionManager2.Sessions)
                            {
                                Frame frame = new()
                                {
                                    Tag = session
                                };
                                frame.Navigate(typeof(IndividualAudioControl));
                                SessionController.Children.Add(frame);
                            }
                        }
                    }
                    catch { }
                });
            }
            catch
            {
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                device.AudioSessionManager2.OnSessionCreated -= NewSesh;
            }
            catch { }
        }
        private MMDevice device;
    }
}
