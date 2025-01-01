using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Communication;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OBSStreamInfoWidget : Page
    {
        public OBSStreamInfoWidget()
        {
            this.InitializeComponent();
            Globals.obsControls.obs.Disconnected += disconnectEvent;
            Globals.obsControls.obs.Connected += connectedEvent;
        }
        private void connectedEvent(object sender, EventArgs e)
        {
            revertLayout();
        }
        private void revertLayout()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Collapsed;
                Page_Loaded(null, null);
            });
        }
        private void disconnectEvent(object sender, ObsDisconnectionInfo e)
        {
            Globals.obsControls.callUpdateStats -= callUpdate;
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Visible;
            });
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.obsControls.connectionSuccessful)
            {
                UpdateOBSStats();
                UpdateRecordingStats();
                UpdateStreamStats();
                Globals.obsControls.callUpdateStats += callUpdate;
            }
            else
            {
                OBSError.Visibility = Visibility.Visible;
            }
        }

        private void callUpdate()
        {
            UpdateOBSStats();
            UpdateRecordingStats();
            UpdateStreamStats();
        }

        private void UpdateOBSStats()
        {
            ObsStats data = Globals.obsControls.obsData;
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    OBSRendered.Text = $"{data.RenderTotalFrames} frames";
                    OBSMissed.Text = $"{data.RenderMissedFrames} frames";
                    OBSOutputTotal.Text = $"{data.OutputTotalFrames} frames";
                    OBSMissed.Text = $"{data.OutputSkippedFrames} frames";
                    OBSAvgRenderTime.Text = $"{data.AverageFrameTime:F2} ms";
                    OBSFramerate.Text = $"{(int)data.FPS}";
                    OBSCPUUsage.Text = $"{data.CpuUsage:F2}%";
                    OBSRAMUsage.Text = $"{data.MemoryUsage:F2} MB";
                }
                catch
                {

                }
                
            });
        }

        private void UpdateStreamStats()
        {
            OutputStatus data = Globals.obsControls.outputStatus;
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    StreamingStatus.Text = $"{(data.IsActive ? "True" : "False")}";
                    ReconnectingStats.Text = $"{(data.IsReconnecting ? "True" : "False")}";
                    StreamTimeCode.Text = $"{data.TimeCode}";
                    StreamDuration.Text = $"{data.Duration} ms";
                    StreamCongestion.Text = $"{data.Congestion:F2}";
                    StreamingTotalFrames.Text = $"{data.TotalFrames} frames";
                    SkippedFramesStreaming.Text = $"{data.SkippedFrames} frames";
                    StreamingUploadedBytes.Text = $"{data.BytesSent} bytes";
                }
                catch
                {

                }
            });
        }

        private void UpdateRecordingStats()
        {
            RecordingStatus data = Globals.obsControls.recStatus;
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    RecordingStatus.Text = $"{(data.IsRecording ? "True" : "False")}";
                    PausedStatus.Text = $"{(data.IsRecordingPaused ? "True" : "False")}";
                    TimeCodeRecording.Text = $"{data.RecordTimecode}";
                    RecordingDuration.Text = $"{data.RecordingDuration} ms";
                    RecordingSize.Text = $"{data.RecordingBytes:F2} bytes";
                }
                catch
                {

                }
            });
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.obsControls.callUpdateStats -= callUpdate;
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
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        revertLayout();
                    });
                }
            };
        }
    }
}
