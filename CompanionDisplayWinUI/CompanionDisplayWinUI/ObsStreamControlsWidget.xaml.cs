using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class ObsStreamControlsWidget : Page
    {
        public ObsStreamControlsWidget()
        {
            this.InitializeComponent();
            Globals.obsControls.obs.Disconnected += disconnectEvent;
            Globals.obsControls.obs.Connected += connectedEvent;
        }

        private void connectedEvent(object sender, EventArgs e)
        {
            revertLayout();
        }

        private void disconnectEvent(object sender, ObsDisconnectionInfo e)
        {
            Globals.obsControls.obs.StreamStateChanged -= updateStreamChecker;
            Globals.obsControls.obs.RecordStateChanged -= updateRecordChecker;
            Globals.obsControls.obs.ReplayBufferStateChanged -= updateBufferChecker;
            Globals.obsControls.obs.VirtualcamStateChanged -= camChecker;
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Visible;
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.obsControls.connectionSuccessful)
            {
                try
                {
                    Globals.obsControls.obs.StreamStateChanged += updateStreamChecker;
                    Globals.obsControls.obs.RecordStateChanged += updateRecordChecker;
                    Globals.obsControls.obs.ReplayBufferStateChanged += updateBufferChecker;
                    Globals.obsControls.obs.VirtualcamStateChanged += camChecker;
                    RecordButton.IsChecked = Globals.obsControls.obs.GetRecordStatus().IsRecording || Globals.obsControls.obs.GetRecordStatus().IsRecordingPaused;
                    PauseButton.IsChecked = Globals.obsControls.obs.GetRecordStatus().IsRecordingPaused;
                    PauseButton.IsEnabled = RecordButton.IsChecked.Value;
                    StreamButton.IsChecked = Globals.obsControls.obs.GetStreamStatus().IsActive;
                    CameraButton.IsChecked = Globals.obsControls.obs.GetVirtualCamStatus().IsActive;
                    ToggleBuffer.IsChecked = Globals.obsControls.obs.GetReplayBufferStatus();
                    BufferSave.IsEnabled = ToggleBuffer.IsChecked.Value;
                }
                catch
                {
                }
            }
            else
            {
                OBSError.Visibility = Visibility.Visible;
            }
        }

        private void camChecker(object sender, VirtualcamStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CameraButton.IsChecked = e.OutputState.IsActive;
            });
        }

        private void updateBufferChecker(object sender, ReplayBufferStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ToggleBuffer.IsChecked = Globals.obsControls.obs.GetReplayBufferStatus();
                BufferSave.IsEnabled = ToggleBuffer.IsChecked.Value;
            });
        }

        private void updateRecordChecker(object sender, RecordStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                RecordButton.IsChecked = (e.OutputState.IsActive || e.OutputState.State == OBSWebsocketDotNet.Types.OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED);
                PauseButton.IsEnabled = RecordButton.IsChecked.Value;
                PauseButton.IsChecked = e.OutputState.State == OBSWebsocketDotNet.Types.OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED;
            });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.obsControls.obs.StreamStateChanged -= updateStreamChecker;
            Globals.obsControls.obs.RecordStateChanged -= updateRecordChecker;
            Globals.obsControls.obs.ReplayBufferStateChanged -= updateBufferChecker;
            Globals.obsControls.obs.VirtualcamStateChanged -= camChecker;
        }
        private void actionSelector(ToggleButton sender, bool start)
        {
            try
            {
                switch (sender.Tag)
                {
                    case "Streaming":
                        if (start)
                        {
                            Globals.obsControls.StartStreaming();
                        }
                        else
                        {
                            Globals.obsControls.StopStreaming();
                        }
                        break;
                    case "Recording":
                        if (start)
                        {
                            Globals.obsControls.StartRecording();
                        }
                        else
                        {
                            Globals.obsControls.StopRecording();
                        }
                        break;
                }
            }
            catch
            {
                (sender as ToggleButton).IsChecked = !(sender as ToggleButton).IsChecked;
            }
            
        }
        private void updateStreamChecker(object sender, StreamStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                StreamButton.IsChecked = e.OutputState.IsActive;
            });
        }

        private async void StreamButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            string tof = "Stop ";
            if ((sender as ToggleButton).IsChecked.Value)
            {
                tof = "Start ";
            }
            dialog.Title = "Do you want to " + tof + (sender as ToggleButton).Tag + "?";
            dialog.PrimaryButtonText = tof + (sender as ToggleButton).Tag.ToString();
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "Confirming will perform this action in OBS.";
            dialog.Tag = (sender as ToggleButton).Tag;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                actionSelector(sender as ToggleButton, (sender as ToggleButton).IsChecked.Value);
            }
            (sender as ToggleButton).IsChecked = !(sender as ToggleButton).IsChecked;
        }

        private void ToggleButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.pauseToggle();
        }

        private void CameraButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.cameraToggle();
        }

        private void MicButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.micToggle();
            MicButton.IsChecked = Globals.obsControls.micMute;
        }

        private void ToggleBuffer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Globals.obsControls.bufferToggle();
            }
            catch
            {
                ToggleBuffer.IsChecked = false;
                System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
                ToggleBuffer.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ToggleBuffer.Content = "";
                        timer.Dispose();
                    });
                };
            }
        }

        private void BufferSave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Globals.obsControls.bufferSave();
                
                System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
                BufferSave.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BufferSave.Content = "";
                        timer.Dispose();
                    });
                };
            }
            catch
            {
                System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
                BufferSave.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BufferSave.Content = "";
                        timer.Dispose();
                    });
                };
            }
            
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Thread thread = new(Globals.obsControls.manualConnectReq);
            thread.Start();
            System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
            (sender as Button).IsEnabled = false;
            timer.Elapsed += (sender, args) =>
            {
                if (!Globals.obsControls.connectionSuccessful)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ReconnectBtn.IsEnabled = true;
                        ReconnectBtn.Content = "Connection failed, try again.";
                        timer.Dispose();
                    });
                }
            };
        }

        private void revertLayout()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Collapsed;
                Page_Loaded(null, null);
            });
        }
    }
}
