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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static CompanionDisplayWinUI.MediaPlayerWidget;
using Windows.System;
using Windows.Media.Control;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LyricsView : Window
    {
        public LyricsView()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 800, Height = 450 });
            this.ExtendsContentIntoTitleBar = true;
        }

        // Constants for Window Styles
        private const int GWL_STYLE = -16;
        private const int WS_SIZEBOX = 0x00040000;   // Enables resizing
        private const int WS_MAXIMIZEBOX = 0x00010000; // Enables maximize button
        private const int WS_MINIMIZEBOX = 0x00020000; // Enables minimize button
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        // Import SetWindowLong and GetWindowLong from User32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);
        private void ToggleButton1_Checked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void ToggleButton1_Unchecked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
        private async void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            PressKey(sender, null);
        }
        private bool IsManipulative = false;
        public bool CleanUp = false, IsDragging = false;
        private async void SongProgressBar_Tapped(object sender, TappedRoutedEventArgs e)
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


        private void SongProgressBar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
        }

        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
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
        private string SongIDTemp = "";
        private string[] LyricsCache;
        private string AlbumCoverCache = "";
        public async void updateUI()
        {
            while (!closed)
            {
                await updateUI0();
                Thread.Sleep(1000);
            }
        }
        private async Task updateUI0()
        {
            try
            {
                if (Globals.SongID != null && Globals.Lyrics != null && (SongIDTemp != Globals.SongID || Globals.Lyrics != LyricsCache) && Globals.Lyrics.Length > 0)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        try
                        {
                            if (Globals.IsSpotify == true)
                            {
                                try
                                {
                                    AlbumCoverImg.Source = new BitmapImage(new Uri(Globals.SongBackground));
                                    AlbumCoverCache = Globals.SongBackground;
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                try
                                {
                                    try
                                    {
                                        AlbumCoverImg.Source = (ImageSource)Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                                        AlbumCoverCache = "";
                                    }
                                    catch
                                    {
                                    }
                                }
                                catch
                                {
                                }
                            }
                            BackgroundImage.Source = AlbumCoverImg.Source;
                            LyricsCache = Globals.Lyrics;
                            LyricsList.Children.Clear();
                            SongIDTemp = Globals.SongID;
                            for (int i = 0; i < Globals.Lyrics.Length; i++)
                            {
                                TextBlock textBlock = new TextBlock();
                                textBlock.Text = Globals.Lyrics[i];
                                textBlock.FontSize = 36;
                                textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                                LyricsList.Children.Add(textBlock);
                            }
                            LyricsList.Children[0].StartBringIntoView();
                        }
                        catch
                        {
                            titleSong.Text = "No song playing";
                            detailsSong.Text = "";
                            CurrentTime.Text = "--:--";
                            EndTime.Text = "--:--";
                            SongProgressBar.Value = 0;
                        }
                    });
                }
                else if (Globals.Lyrics == null || Globals.Lyrics.Length == 0)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        LyricsList.Children.Clear();
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = "No lyrics are available for this song.";
                        textBlock.FontSize = 36;
                        textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                        LyricsList.Children.Add(textBlock);
                    });
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        titleSong.Text = Globals.SongName;
                        detailsSong.Text = Globals.SongDetails;
                        CurrentTime.Text = Globals.SongTime;
                        EndTime.Text = Globals.SongEnd;
                        SongProgressBar.Value = Globals.SongProgress;
                        if (LyricsList.Children.Count != 0)
                        {
                            for (int i = 0; i < LyricsList.Children.Count; i++)
                            {
                                if (i < Globals.currentLyric)
                                {
                                    (LyricsList.Children[i] as TextBlock).Opacity = 0.9;
                                }
                                else if (i == Globals.currentLyric)
                                {
                                    (LyricsList.Children[i] as TextBlock).Opacity = 1;
                                    if (!ManualScrolling)
                                    {
                                        try
                                        {
                                            (LyricsList.Children[i + 2] as TextBlock).StartBringIntoView();
                                            (LyricsList.Children[i] as TextBlock).StartBringIntoView();
                                        }
                                        catch
                                        {
                                            (LyricsList.Children[i] as TextBlock).StartBringIntoView();

                                        }
                                    }
                                }
                                else
                                {
                                    (LyricsList.Children[i] as TextBlock).Opacity = 0.6;
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
                    }
                    catch
                    {
                            
                    }
                });
            }
            catch
            {

            }
        }
        private bool ManualScrolling = false;
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = true;
        }

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            ImageOptionalBlur.Stretch = Stretch.None;
            ImageOptionalBlur.Stretch = Stretch.UniformToFill;
        }
        bool closed = false;
        private void Window_Closed(object sender, WindowEventArgs args)
        {
           closed = true;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(updateUI);
            thread.Start();
            try
            {
                if (Globals.IsSpotify == true)
                {
                    AlbumCoverImg.Source = new BitmapImage(new Uri(Globals.SongBackground));
                    AlbumCoverCache = Globals.SongBackground;
                }
                else
                {
                    try
                    {
                        AlbumCoverImg.Source = (ImageSource)Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                        AlbumCoverCache = "";
                    }
                    catch
                    {
                    }
                }
                BackgroundImage.Source = AlbumCoverImg.Source;
            }
            catch { }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = false;
        }
    }
}
