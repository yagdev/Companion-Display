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
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.ViewManagement;
using WinRT.Interop;
using static CompanionDisplayWinUI.MediaPlayerWidget;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Media.Control;
using Windows.UI;
using Windows.UI.WebUI;
using Microsoft.Web.WebView2.Core;
using static System.Net.WebRequestMethods;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongA")]
        private static partial int GetWindowLong(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongA")]
        private static partial void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            _appWindow = GetAppWindowForCurrentWindow();
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }
        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (sender.IsPaneToggleButtonVisible && sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto || sender.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !sender.IsPaneToggleButtonVisible)
            {
                try
                {
                    GC.Collect();
                    Globals.IsAllApps = false;
                    IntPtr hWnd = WindowNative.GetWindowHandle(this);
                    sender.PaneClosed -= ProcessShit;
                    switch (args.SelectedItemContainer.Tag.ToString())
                    {
                        case "SecretStuff":
                            contentFrame.Navigate(typeof(MusixmatchIntegrationProto), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage1":
                            EvadeCringeBehavior = false;
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(BlankPage1), null, args.RecommendedNavigationTransitionInfo);
                            if (Globals.StealFocus)
                            {
                                SetWindowLong(hWnd, -20, GetWindowLong(hWnd, -20) | 134480128);
                            }
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage2":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(BrowserTab), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage3":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(SpotifyPlayer), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SleepMode":
                            SleepReptangle.Visibility = Visibility.Visible;
                            contentFrame.Navigate(typeof(SleepPage), null, args.RecommendedNavigationTransitionInfo);
                            if (Globals.StealFocus)
                            {
                                SetWindowLong(hWnd, -20, GetWindowLong(hWnd, -20) | 134480128);
                            }
                            break;
                        case "Settings":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            SetWindowLong(hWnd, -20, 256);
                            contentFrame.Navigate(typeof(BlankPage3), null, args.RecommendedNavigationTransitionInfo);
                            sender.PaneClosed += ProcessShit;
                            break;
                    }
                }
                catch { }
            }
        }

        private void ProcessShit(NavigationView sender, object args)
        {
            if (!sender.IsPaneToggleButtonVisible)
            {
                sender.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
                sender.IsPaneToggleButtonVisible = true;
                sender.PaneClosed -= ProcessShit;
            }
        }

        private bool EvadeCringeBehavior = false;
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Globals.StartedPlayer = false;
            Environment.Exit(0);
        }
        string SongTitleCache = "";
        int SongCoverStarted = 0;
        private async void SongCoverBackground()
        {
            if (Globals.IsSpotify == true && Globals.SongName != SongTitleCache)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        BackgroundImage.Source = new BitmapImage(new Uri(Globals.SongBackground));
                        SongTitleCache = Globals.SongName;
                    }
                    catch
                    {
                    }
                });
            }
            else
            {
                if(Globals.IsSpotify == false)
                {
                    try
                    {
                        if (SongTitleCache != Globals.songInfo.Title)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    BackgroundImage.Source = null;
                                    BackgroundImage.Source = (ImageSource)Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                                    SongTitleCache = Globals.songInfo.Title;
                                }
                                catch
                                {

                                }
                            });
                        }
                    }
                    catch
                    {
                    }
                }
            }
            Thread.Sleep(1000);
            if(Globals.Backdrop == 3)
            {
                Thread thread = new(SongCoverBackground);
                thread.Start();
            }
            else
            {
                SongCoverStarted = 0;
            }
        }
        private async void UpdateUI()
        {
            try
            {
                switch (Globals.Backdrop)
                {
                    case (0):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                            SystemBackdrop = new DesktopAcrylicBackdrop();
                            BackgroundImage.Visibility = Visibility.Collapsed;
                            ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            BackgroundVideo.Visibility = Visibility.Collapsed;
                            BackgroundImage.Source = null;
                            BackgroundVideo.Source = null;
                        });
                        break;
                    case (1):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            SystemBackdrop = new MicaBackdrop();
                            BackgroundImage.Visibility = Visibility.Collapsed;
                            ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            BackgroundVideo.Visibility = Visibility.Collapsed;
                            BackgroundImage.Source = null;
                            BackgroundVideo.Source = null;
                        });
                        break;
                    case (2):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            SystemBackdrop = null;
                            if (Globals.Blur == true)
                            {
                                ImageOptionalBlur.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            }
                        });
                        try
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    if (Globals.BackgroundLink == "")
                                    {
                                        string imagePath = Globals.Wallpaper;
                                        BitmapImage bitmapImage = new();
                                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                                        {
                                            bitmapImage.SetSource(stream.AsRandomAccessStream());
                                        }
                                        BackgroundImage.Source = bitmapImage;
                                    }
                                    else
                                    {
                                        BitmapImage bitmapImage = new()
                                        {
                                            UriSource = new Uri(Globals.BackgroundLink)
                                        };
                                        BackgroundImage.Source = bitmapImage;
                                    }
                                    BackgroundImage.Visibility = Visibility.Visible;
                                    BackgroundVideo.Visibility = Visibility.Collapsed;
                                    BackgroundVideo.Source = null;
                                }
                                catch
                                {
                                }
                            });
                            if (Globals.BackgroundLink == "")
                            {
                                StorageFile file = await StorageFile.GetFileFromPathAsync(Globals.Wallpaper);
                                using (var mediaSource = MediaSource.CreateFromStorageFile(file))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        try
                                        {
                                            BackgroundVideo.Source = mediaSource;
                                            BackgroundVideo.ElementSoundMode = ElementSoundMode.Off;
                                            BackgroundVideo.Visibility = Visibility.Visible;
                                            BackgroundVideo.MediaPlayer.Volume = 0;
                                            BackgroundVideo.AutoPlay = true;
                                        }
                                        catch { }
                                    });
                                }
                            }
                            else
                            {
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    using (MediaSource mediaSource = MediaSource.CreateFromUri(new Uri(Globals.BackgroundLink)))
                                    {
                                        try
                                        {
                                            BackgroundVideo.Source = mediaSource;
                                            BackgroundVideo.ElementSoundMode = ElementSoundMode.Off;
                                            BackgroundVideo.Visibility = Visibility.Visible;
                                            BackgroundVideo.AutoPlay = true;
                                        }
                                        catch { }
                                    }
                                });
                            }
                        }
                        catch
                        {
                        }

                        break;
                    case (3):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            SystemBackdrop = null;
                            BackgroundImage.Visibility = Visibility.Visible;
                            if (Globals.Blur == true)
                            {
                                ImageOptionalBlur.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            }
                            BackgroundVideo.Visibility = Visibility.Collapsed;
                        });
                        if(SongCoverStarted == 0)
                        {
                            SongCoverStarted = 1;
                            Thread thread = new(SongCoverBackground);
                            thread.Start();
                        }
                        break;
                    case (4):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            BackgroundImage.Visibility = Visibility.Collapsed;
                            ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            BackgroundVideo.Visibility = Visibility.Collapsed;
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(255, (byte)Globals.BackgroundColorR, (byte)Globals.BackgroundColorG, (byte)Globals.BackgroundColorB));
                        });
                        if (SongCoverStarted == 0)
                        {
                            SongCoverStarted = 1;
                            Thread thread = new(SongCoverBackground);
                            thread.Start();
                        }
                        break;
                }
            }
            catch
            {

            }
        }
        private void ContentFrame_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Thread thread = new (UpdateUI);
            thread.Start();
        }
        private readonly AppWindow _appWindow;

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (ImageOptionalBlur.Visibility == Visibility.Visible)
            {
                ImageOptionalBlur.Stretch = Stretch.None;
                ImageOptionalBlur.Stretch = Stretch.UniformToFill;
            }
            try
            {
                foreach (var children in ((contentFrame.Content as BlankPage1).FindName("BasicGridView") as GridView).Items)
                {
                    if (children as Frame != null)
                    {
                        (children as Frame).Background = null;
                        (children as Frame).Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                    }
                }
            }
            catch { }
        }

        private void DebugPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
            {
                CornerMask.Visibility = Visibility.Visible;
                AppTitleBar.Visibility = Visibility.Visible;
                this.ExtendsContentIntoTitleBar = true;
                _appWindow.SetPresenter(AppWindowPresenterKind.Default);
                DebugPage.Content = "Enter Fullscreen";
                DebugPage.Icon = new SymbolIcon(Symbol.FullScreen);
            }
            else
            {
                this.ExtendsContentIntoTitleBar = false;
                CornerMask.Visibility = Visibility.Collapsed;
                AppTitleBar.Visibility = Visibility.Collapsed;
                _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                DebugPage.Content = "Exit Fullscreen";
                DebugPage.Icon = new SymbolIcon(Symbol.BackToWindow);
            }
        }

        private void nvSample_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.triggerSetup)
            {
                (sender as NavigationView).IsPaneVisible = false;
                contentFrame.Navigate(typeof(SetupStep0));
            }
        }
    }
}
