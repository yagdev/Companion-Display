using EmbedIO.Sessions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using CoreAudio;
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
                using (IRandomAccessStreamWithContentType imageStream = Thumbnail.OpenReadAsync().GetAwaiter().GetResult())
                using (DataReader reader = new(imageStream))
                using (var stream = new InMemoryRandomAccessStream())
                using (var writer = new DataWriter(stream))
                {
                    byte[] fileBytes = new byte[imageStream.Size];
                    reader.LoadAsync((uint)imageStream.Size).GetAwaiter().GetResult();
                    reader.ReadBytes(fileBytes);
                    BitmapImage image = new();
                    writer.WriteBytes(fileBytes);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    writer.StoreAsync();
                    writer.FlushAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    writer.DetachStream();
                    stream.Seek(0);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    image.SetSourceAsync(stream);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    return image;
                }
            }
        }
        private async void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TryTogglePlayPauseAsync();
            }
            catch
            {
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TrySkipPreviousAsync();
            }
            catch
            {
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await GlobalSystemMediaTransportControlsSessionManager.RequestAsync()).GetCurrentSession().TrySkipNextAsync();
            }
            catch
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
            }
        }

        private void VolumeBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MMDeviceEnumerator DevEnum = new (Guid.NewGuid());
            using(var defaultPlaybackDevice = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(VolumeBar.Value / 100);
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
            MMDeviceEnumerator DevEnum = new(Guid.NewGuid());
            using (var mDeviceEnumerator = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                VolumeCur = mDeviceEnumerator.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            Thread thread0 = new(StartUI);
            thread0.Start();
        }
        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
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
            catch
            {
                IsDragging = false;
            }
        }

        private async void SongProgressBar_Tapped(object sender, TappedRoutedEventArgs e)
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
            catch
            {
                IsDragging = false;
            }
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
            catch
            {
                IsDragging = false;
            }
        }
        private string LastTitle = "-", LastDetail = "-", LastLyric = "-", LastTime = "-", LastEnd = "-";
        private float LastVol = -2;
        private bool LastSpotifyCheck = false;
        private async void UpdateUI()
        {
            try
            {
                if(LastTitle != Globals.SongName)
                {
                    LastTitle = Globals.SongName;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SongTitle.Text = Globals.SongName;
                    });
                }
                if (LastDetail != Globals.SongDetails)
                {
                    LastDetail = Globals.SongDetails;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SongInfo.Text = Globals.SongDetails;
                    });
                }
                if (LastLyric != Globals.SongLyrics)
                {
                    LastLyric = Globals.SongLyrics;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SongLyrics.Text = Globals.SongLyrics;
                    });
                }
                if (LastTime != Globals.SongTime)
                {
                    LastTime = Globals.SongTime;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        CurrentTime.Text = Globals.SongTime;
                        if (IsDragging == false)
                        {
                            try
                            {
                                SongProgressBar.Value = Globals.SongProgress;
                            }
                            catch
                            {
                                SongProgressBar.Value = 0;
                            }
                        }
                    });
                }
                if (LastEnd != Globals.SongEnd)
                {
                    LastEnd = Globals.SongEnd;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        EndTime.Text = Globals.SongEnd;
                    });
                }
                if(LastSpotifyCheck != Globals.IsSpotify)
                {
                    LastSpotifyCheck = Globals.IsSpotify;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (Globals.IsSpotify == true)
                        {
                            SpotifyLogo.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            SpotifyLogo.Visibility = Visibility.Collapsed;
                        }
                    });
                }
                try
                {
                    MMDeviceEnumerator DevEnum = new(Guid.NewGuid());
                    using (var defaultPlaybackDevice = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                    {
                        if (LastVol != defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar)
                        {
                            LastVol = defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                VolumeCur = LastVol * 100;
                                VolumeBar.Value = VolumeCur;
                            });
                        }
                    }
                }
                catch { }
            }
            catch
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
                        catch
                        {
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
                    catch
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
            catch
            {
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
