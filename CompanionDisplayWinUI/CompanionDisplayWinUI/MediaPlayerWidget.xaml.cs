using EmbedIO.Sessions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Control;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlayerWidget : Page
    {
        string AlbumCoverCache, SongTitleCache;
        public double VolumeCur;
        public MediaPlayerWidget()
        {
            this.InitializeComponent();
            Thread thread0 = new(StartUI);
            thread0.Start();
        }
        public bool CleanUp = false, IsDragging = false;
        internal static class Helper
        {
            internal static BitmapImage GetThumbnail(IRandomAccessStreamReference Thumbnail)
            {
                if (Thumbnail == null)
                {
                    return null;
                }
                IRandomAccessStreamWithContentType imageStream = Thumbnail.OpenReadAsync().GetAwaiter().GetResult();
                byte[] fileBytes = new byte[imageStream.Size];
                using (DataReader reader = new (imageStream))
                {
                    reader.LoadAsync((uint)imageStream.Size).GetAwaiter().GetResult();
                    reader.ReadBytes(fileBytes);
                }
                BitmapImage image = new ();
                using (var stream = new InMemoryRandomAccessStream())
                {
                    using (var writer = new DataWriter(stream))
                    {
                        writer.WriteBytes(fileBytes);
                        writer.StoreAsync();
                        writer.FlushAsync();
                        writer.DetachStream();
                    }
                    stream.Seek(0);
                    image.SetSourceAsync(stream);
                }
                return image;
            }
        }
        private async void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TryTogglePlayPauseAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TrySkipPreviousAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TrySkipNextAsync();
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
            }
        }

        private void VolumeBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            using (var deviceEnumerator = new MMDeviceEnumerator())
            {
                var defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(VolumeBar.Value/100) ;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
        private void StartUI()
        {
            if (Globals.StartedPlayer == false)
            {
                Globals.StartedPlayer = true;
                PlayerSpotify mediaPlayerWidget = new();
                mediaPlayerWidget.Page_Loaded();
            }
            using (var deviceEnumerator = new MMDeviceEnumerator())
            {
                var defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                VolumeCur = defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                DispatcherQueue.TryEnqueue(() =>
                {
                    VolumeBar.Value = VolumeCur;
                });
            }
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }

        private void SongProgressBar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
        }

        private async void Grid_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                GlobalSystemMediaTransportControlsSessionManager sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                long maxpos = long.Parse((await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TryChangePlaybackPositionAsync(newpos);
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch (Exception ex)
            {
                IsDragging = false;
            }
        }
        private async void UpdateUI()
        {
            try
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SongTitle.Text = Globals.SongName;
                    SongInfo.Text = Globals.SongDetails;
                    SongLyrics.Text = Globals.SongLyrics;
                    CurrentTime.Text = Globals.SongTime;
                    EndTime.Text = Globals.SongEnd;
                    if (Globals.IsSpotify == true)
                    {
                        SpotifyLogo.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SpotifyLogo.Visibility = Visibility.Collapsed;
                    }
                    if (IsDragging == false)
                    {
                        try
                        {
                            SongProgressBar.Value = Globals.SongProgress;
                        }
                        catch (Exception ex)
                        {
                            //File.AppendAllText("ErrorLog.crlh", ex.Message);
                            SongProgressBar.Value = 0;
                        }
                    }
                });
                try
                {
                    using (var deviceEnumerator = new MMDeviceEnumerator())
                    {
                        var defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                        VolumeCur = defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            VolumeBar.Value = VolumeCur;
                        });
                    }
                }
                catch { }
               
            }
            catch (Exception ex)
            {
            }
            if (Globals.SongBackground != AlbumCoverCache)
            {
                if (Globals.IsSpotify == true)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        try
                        {
                            AlbumCoverImg.Source = new BitmapImage(new Uri(Globals.SongBackground));
                            AlbumCoverCache = Globals.SongBackground;
                        }
                        catch (Exception ex)
                        {
                            //File.AppendAllText("ErrorLog.crlh", ex.Message);
                        }
                    });
                }
                else
                {
                    try
                    {
                        if (Globals.SongName != SongTitleCache)
                        {
                            GlobalSystemMediaTransportControlsSessionManager sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                            GlobalSystemMediaTransportControlsSessionMediaProperties songInfo = await sessionManager.GetCurrentSession().TryGetMediaPropertiesAsync();
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    AlbumCoverImg.Source = (ImageSource)Helper.GetThumbnail(songInfo.Thumbnail);
                                    AlbumCoverCache = "";
                                    SongTitleCache = songInfo.Title;
                                }
                                catch (Exception e)
                                {
                                    SongTitle.Text = e.Message; 
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            try
            {
                if ((await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().GetPlaybackInfo().PlaybackStatus.ToString() == "Paused")
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPauseBtn.Content = "\ue768";
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPauseBtn.Content = "\ue769";
                    });
                }
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
            }
            if(!CleanUp)
            {
                Thread.Sleep(1000);
                Thread thread0 = new(UpdateUI);
                thread0.Start();
            }
        }
    }
}
