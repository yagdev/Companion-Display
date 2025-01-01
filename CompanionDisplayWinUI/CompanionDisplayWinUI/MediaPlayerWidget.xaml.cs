using EmbedIO.Sessions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
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
using CoreAudio;
using Windows.Media;
using Windows.System;
using System.Runtime.InteropServices;

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
            PressKey(sender, null);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            PressKey(sender, null);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PressKey(sender, null);
        }
        [LibraryImport("user32.dll", SetLastError = true)]
        static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(VirtualKey key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            else
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        }
        private bool Updating = false;
        private void PressKey(object sender, RoutedEventArgs e)
        {
            Windows.UI.Core.CoreVirtualKeyStates numkey = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.NumberKeyLock);
            try
            {
                PressKey((VirtualKey)int.Parse(((HyperlinkButton)sender).Tag.ToString()), up: false);
                PressKey((VirtualKey)int.Parse(((HyperlinkButton)sender).Tag.ToString()), up: true);
            }
            catch
            {

            }
        }

        private void VolumeBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsManipulative)
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(VolumeBar.Value / 100);
            }
        }
        private MMDevice mDevice;
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
            MMDeviceEnumerator DevEnum = new();
            mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            mDevice.AudioEndpointVolume.OnVolumeNotification += ChangeVol;
            VolumeCur = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
            DispatcherQueue.TryEnqueue(() =>
            {
                VolumeBar.Value = VolumeCur;
            });
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private bool IsManipulative = false;
        private void ChangeVol(AudioVolumeNotificationData data)
        {
            if (!IsManipulative)
            {
                try
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        VolumeBar.Value = data.MasterVolume * 100;
                    });
                }
                catch { }
            }
        }

        private void SongProgressBar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            MMDeviceEnumerator DevEnum = new();
            mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            Thread thread0 = new(StartUI);
            thread0.Start();
        }
        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(Globals.sessionManager.GetCurrentSession().TryChangePlaybackPositionAsync(newpos));
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
                long maxpos = long.Parse(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(Globals.sessionManager.GetCurrentSession().TryChangePlaybackPositionAsync(newpos));
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
                long maxpos = long.Parse(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await (Globals.sessionManager.GetCurrentSession().TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch
            {
                IsDragging = false;
            }
        }
        private string LastTitle = "-", LastDetail = "-", LastLyric = "-", LastTime = "-", LastEnd = "-", LastAlbum = "-";

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow.IsEnabled = false;
            LyricsView m_window = new LyricsView();
            m_window.Closed += (s, e) =>
            {
                OpenWindow.IsEnabled = true;
            };
            m_window.Activate();
        }

        private void VolumeBar_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            IsManipulative = true;
        }

        private void VolumeBar_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            IsManipulative = false;
        }

        private bool LastSpotifyCheck = false;
        private BitmapImage bitmapImage;
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
                if (LastAlbum != Globals.AlbumName)
                {
                    LastAlbum = Globals.AlbumName;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AlbumName.Text = Globals.AlbumName;
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
                        if (Globals.IsSpotify)
                        {
                            SpotifyLogo.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            SpotifyLogo.Visibility = Visibility.Collapsed;
                        }
                    });
                }
            }
            catch
            {
            }
            if (Globals.SongBackground != AlbumCoverCache || !Globals.IsSpotify)
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
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    AlbumCoverImg.Source = (ImageSource)Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                                    AlbumCoverCache = "";
                                    SongTitleCache = Globals.songInfo.Title;
                                }
                                catch
                                {
                                    SongTitle.Text = "-"; 
                                    SongInfo.Text = "-";
                                    SongLyrics.Text = "-";
                                    AlbumName.Text = "-";
                                    EndTime.Text = "--:--";
                                    CurrentTime.Text = "--:--";
                                    SongTitleCache = "-";
                                    LastDetail = "-";
                                    LastTitle = "-";
                                    LastLyric = "-";
                                    LastTime = "-";
                                    LastEnd = "-";
                                    LastAlbum = "-";
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
                if (Globals.sessionManager.GetCurrentSession().GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPauseBtn.Content = "\uf8ae";
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPauseBtn.Content = "\uf5b0";
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
