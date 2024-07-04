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
using Windows.Foundation;
using static CompanionDisplayWinUI.MediaPlayerWidget;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Media.Control;
using Windows.UI;
using Microsoft.Web.WebView2.Core;
using Windows.UI.WebUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            _appWindow = GetAppWindowForCurrentWindow();
            Thread thread = new Thread(UpdateUI);
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
            try
            {
                GC.Collect();
                Globals.IsAllApps = false;
                IntPtr hWnd = WindowNative.GetWindowHandle(this);
                switch (args.SelectedItemContainer.Tag.ToString())
                {
                    case "Musixmatch":
                        contentFrame.Navigate(typeof(MusixmatchIntegrationProto));
                        SetWindowLong(hWnd, -20, 256);
                        SpotifyBuiltin.Visibility = Visibility.Collapsed;
                        break;
                    case "SamplePage1":
                        contentFrame.Navigate(typeof(BlankPage1));
                        SpotifyBuiltin.Visibility = Visibility.Collapsed;
                        if (Globals.StealFocus)
                        {
                            SetWindowLong(hWnd, -20, GetWindowLong(hWnd, -20) | 134480128);
                        }
                        break;
                    case "SamplePage2":
                        contentFrame.Navigate(typeof(BlankPage2));
                        SpotifyBuiltin.Visibility = Visibility.Collapsed;
                        SetWindowLong(hWnd, -20, 256);
                        break;
                    case "SamplePage3":
                        SpotifyBuiltin.Visibility = Visibility.Visible;
                        SetWindowLong(hWnd, -20, 256);
                        break;
                    case "Settings":
                        SetWindowLong(hWnd, -20, 256);
                        contentFrame.Navigate(typeof(BlankPage3));
                        SpotifyBuiltin.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            catch { }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Environment.Exit(0);
        }
        string AlbumCoverCache = "", SongTitleCache = "";
        int SongCoverStarted = 0;
        private async void SongCoverBackground()
        {
            if (Globals.IsSpotify == true && Globals.SongName != SongTitleCache)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.UriSource = new Uri(Globals.SongBackground);
                        BackgroundImage.Source = new BitmapImage(new Uri(Globals.SongBackground));
                        AlbumCoverCache = Globals.SongBackground;
                        SongTitleCache = Globals.SongName;
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("ErrorLog.crlh", ex.Message);
                    }
                });
            }
            else
            {
                if(Globals.IsSpotify == false)
                {
                    try
                    {
                        GlobalSystemMediaTransportControlsSessionManager sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                        GlobalSystemMediaTransportControlsSessionMediaProperties songInfo = await sessionManager.GetCurrentSession().TryGetMediaPropertiesAsync();
                        if (SongTitleCache != songInfo.Title)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    BackgroundImage.Source = (ImageSource)Helper.GetThumbnail(songInfo.Thumbnail);
                                    AlbumCoverCache = "";
                                    SongTitleCache = songInfo.Title;
                                }
                                catch (Exception e)
                                {

                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("ErrorLog.crlh", ex.Message);
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
                                        BitmapImage bitmapImage = new BitmapImage();
                                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                                        {
                                            bitmapImage.SetSource(stream.AsRandomAccessStream());
                                        }
                                        BackgroundImage.Source = bitmapImage;
                                    }
                                    else
                                    {
                                        BitmapImage bitmapImage = new BitmapImage();
                                        bitmapImage.UriSource = new Uri(Globals.BackgroundLink);
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
                                var mediaSource = MediaSource.CreateFromStorageFile(file);
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
                            else
                            {
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    try
                                    {
                                        MediaSource mediaSource = MediaSource.CreateFromUri(new Uri(Globals.BackgroundLink));
                                        BackgroundVideo.Source = mediaSource;
                                        BackgroundVideo.ElementSoundMode = ElementSoundMode.Off;
                                        BackgroundVideo.Visibility = Visibility.Visible;
                                        BackgroundVideo.AutoPlay = true;
                                    }
                                    catch { }
                                });
                            }


                        }
                        catch (Exception ex)
                        {
                            try
                            {

                            }
                            catch { }
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
        private async void contentFrame_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Thread thread = new Thread(UpdateUI);
            thread.Start();
        }
        private AppWindow _appWindow;
        private async void SpotifyBuiltin_Loaded(object sender, RoutedEventArgs e)
        {
            var environmentOptions = new CoreWebView2EnvironmentOptions();
            environmentOptions.AreBrowserExtensionsEnabled = true;
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, null, environmentOptions);
            await SpotifyBuiltin.EnsureCoreWebView2Async(environment);
            await SpotifyBuiltin.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.57.2_0"));
            SpotifyBuiltin.Source = new Uri("https://open.spotify.com/");
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
            {
                _appWindow.SetPresenter(AppWindowPresenterKind.Default);
                FullScreenBtn.Content = "\ue740";
            }
            else
            {
                _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                FullScreenBtn.Content = "\ue73f";
            }
        }
    }
}
