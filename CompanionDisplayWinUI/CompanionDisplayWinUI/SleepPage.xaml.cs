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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static CompanionDisplayWinUI.MediaPlayerWidget;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Windows.Media.Control;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SleepPage : Page
    {
        public SleepPage()
        {
            if (!Globals.StartedPlayer)
            {
                Globals.StartedPlayer = true;
                PlayerSpotify playerSpotify = new PlayerSpotify();
                playerSpotify.Page_Loaded();
            }
            this.InitializeComponent();
        }
        private string DateStr, TimeStr;
        private bool CleanUp = false;

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
        private Frame parent;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.OverrideColor)
            {
                try
                {
                    CleanUp = false;
                    Time.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    Date.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)(byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    SongTitle.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    Lyrics.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                }
                catch
                {

                }
            }
            StackUnderflow.Opacity = Globals.sleepModeOpacity / 100;
            Thread thread = new(UpdateUI);
            thread.Start();
            try
            {
                (((this.Parent) as Frame).Parent as NavigationView).IsPaneToggleButtonVisible = false;
                (((this.Parent) as Frame).Parent as NavigationView).PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }
            catch
            {

            }
            if (!Globals.StartedPlayer)
            {
                Globals.StartedPlayer = true;
                PlayerSpotify mediaPlayerWidget = new();
                mediaPlayerWidget.Page_Loaded();
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            (((this.Parent) as Frame).Parent as NavigationView).IsPaneOpen = true;
        }
        private string LastTitle, LastAlbum, LastDetail, LastLyric;
        private void UpdateUI()
        {
            try
            {
                if (DateStr != DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy"))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Date.Text = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                    });
                    DateStr = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                }
                if (TimeStr != TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm"))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Time.Text = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                        Random rnd = new Random();
                        StackUnderflow.Padding = new Thickness(0 + (rnd.Next(-30, 30)), 0 + (rnd.Next(-30, 30)), 0, 0);
                        Oppenheimer.Padding = new Thickness(0 + (rnd.Next(-5, 5)), 0 + (rnd.Next(-5, 5)), 0, 0);
                    });
                    TimeStr = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        if(Globals.playbackInfo != null && Globals.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing || Globals.IsSpotify)
                        {
                            SongTitle.Text = Globals.SongName + " · " + Globals.SongDetails;
                            SongTitle.Visibility = Visibility.Visible;
                            if (Globals.SongLyrics == "")
                            {
                                Lyrics.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                Lyrics.Text = Globals.SongLyrics;
                                Lyrics.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            SongTitle.Visibility = Visibility.Collapsed;
                            Lyrics.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch
                    {
                        SongTitle.Visibility = Visibility.Collapsed;
                        Lyrics.Visibility = Visibility.Collapsed;
                    }
                });
                try
                {
                    if (Globals.sessionManager.GetCurrentSession().GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused && Globals.IsSpotify == false && Globals.SongName != "")
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            SongTitle.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            SongTitle.Visibility = Visibility.Visible;
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
            if (CleanUp == false)
            {
                Thread.Sleep(1000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
    }
}
