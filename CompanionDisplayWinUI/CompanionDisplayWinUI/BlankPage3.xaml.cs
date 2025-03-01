using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using System.Threading;
using System.Diagnostics;
using Windows.System;
using System.Net;
using IWshRuntimeLibrary;
using System.Net.Http;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.DirectoryServices.ActiveDirectory;
using Windows.UI.WebUI;
using System.Text;
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage3 : Page
    {
        public bool LoadFinish = false;
        public BlankPage3()
        {
            this.InitializeComponent();
            if (System.IO.File.Exists("Config/SE.crlh"))
            {
                AppIconImg.Source = new BitmapImage(new Uri("https://i.imgur.com/ng8AhkJ.jpeg"));
            }
            this.NavigationCacheMode = NavigationCacheMode.Required;
            if(Globals.IsUpdateAvailable == true)
            {
                UpdateBtn.IsEnabled = true;
                UpdateBtn.Content = "Update";
            }
            else
            {
                UpdateBtn.IsEnabled = false;
                UpdateBtn.Content = "Up to date";
            }
            ColorSchemeSelect.SelectedIndex = Globals.ColorSchemeSelect;
            AccentSelect.SelectedIndex = Globals.InjectCustomAccent;
            AccentColorPicker.Color = Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB);
            BackdropSelect.SelectedIndex = Globals.Backdrop;
            BackgroundLink.Text = Globals.BackgroundLink;
            ImageBlurToggle.IsOn = Globals.Blur;
            VersionString.Text = Globals.Version;
            FocusToggle.IsOn = Globals.StealFocus;
            UpdateToggle.IsOn = Globals.IsBetaProgram;
            AddButtonToggle.IsOn = Globals.HideAddButton;
            StartupToggle.IsOn = Globals.LaunchOnStartup;
            LockToggle.IsOn = Globals.LockLayout;
            Opacity.Value = Globals.sleepModeOpacity;
            OvrColorSleepMode.IsChecked = Globals.OverrideColor;
            SleepModeColor.Color = Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB);
            SearchEngineCust.Text = Globals.SearchEngine.ToString();
            NewTabBehavior.SelectedIndex = Globals.NewTabBehavior;
            OBSIP.Text = Globals.obsIP;
            OBSPass.Password = Globals.obsPass;
            if (Globals.obsControls.connectionSuccessful)
            {
                OBSStatus.Text = "Status: Connected";
            }
            else
            {
                OBSStatus.Text = "Status: Disconnected";
            }
            InstalledFontCollection fontCollection = new InstalledFontCollection();
            foreach (var fontFamily in fontCollection.Families)
            {
                MenuFlyoutItem item = new MenuFlyoutItem();
                item.Text = fontFamily.Name;
                item.FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(fontFamily.Name);
                item.Click += MenuFlyoutItem_Click;
                FontSelectorActually.Items.Add(item);
            }
            FontSelector.Content = App.CurrentFont();
            LoadFinish = true;
        }
        private void ProcessShit(NavigationView sender, object args)
        {
            sender.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
            sender.PaneClosed -= ProcessShit;
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            App.SetFont(new Microsoft.UI.Xaml.Media.FontFamily((sender as MenuFlyoutItem).Text));
            FontSelector.Content = (sender as MenuFlyoutItem).Text;
            Globals.Save_Settings();
            (mainframe.Parent as NavigationView).PaneClosed += ProcessShit;
            (mainframe.Parent as NavigationView).PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            mainframe.Navigate(typeof(BlankPage1));
            mainframe.Navigate(typeof(BlankPage3));
        }
        private Frame mainframe;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFinish == true)
            {
                string Theme = e.AddedItems[0].ToString();
                switch (Theme)
                {
                    case "System Automatic":
                        ((App)Application.Current).SetAppTheme(ElementTheme.Default);
                        break;
                    case "Dark":
                        ((App)Application.Current).SetAppTheme(ElementTheme.Dark);
                        break;
                    case "Light":
                        ((App)Application.Current).SetAppTheme(ElementTheme.Light);
                        break;
                }
                Globals.ColorSchemeSelect = ColorSchemeSelect.SelectedIndex;
                Globals.Save_Settings();
            }
        }
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFinish == true)
            {
                string Theme = e.AddedItems[0].ToString();
                var Parent = MainScroll.Parent as BlankPage3;
                switch (Theme)
                {
                    case "System Automatic":
                        App.RevertToSystemAccentColor();
                        break;
                    case "Custom":
                        App.SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                        break;
                }
                Globals.InjectCustomAccent = AccentSelect.SelectedIndex;
                Globals.Save_Settings();
                mainframe.Navigate(typeof(BlankPage1));
                mainframe.Navigate(typeof(BlankPage3));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainframe = this.Parent as Frame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                Globals.ColorSchemeSelectAccentR = AccentColorPicker.Color.R;
                Globals.ColorSchemeSelectAccentG = AccentColorPicker.Color.G;
                Globals.ColorSchemeSelectAccentB = AccentColorPicker.Color.B;
                if (Globals.InjectCustomAccent == 1)
                {
                    App.SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                }
                Globals.Save_Settings();
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if(LoadFinish == true)
            {
                string Theme = e.AddedItems[0].ToString();
                Globals.Backdrop = BackdropSelect.SelectedIndex;
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
                Globals.Save_Settings();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                AppPicker appPicker = new();
                string btntag = appPicker.SetIcon();
                if (btntag != null)
                {
                    Globals.Wallpaper = btntag;
                }
                Globals.Save_Settings();
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (FocusToggle.IsOn)
                {
                    Globals.StealFocus = true;
                }
                else
                {
                    Globals.StealFocus = false;
                }
                Globals.Save_Settings();
            }
        }
        private void Save_SettingsOBS()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Globals.ResetHome = true;
                string settingsfile = Globals.obsIP + "\n" + Globals.obsPass;
                System.IO.File.WriteAllText("Config/OBSSettings.crlh", settingsfile);
            });
        }

        private void ImageBlurToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (ImageBlurToggle.IsOn)
                {
                    Globals.Blur = true;
                }
                else
                {
                    Globals.Blur = false;
                }
                Globals.Save_Settings();
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
            }
        }

        private void BackgroundLink_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.BackgroundLink = BackgroundLink.Text;
            Globals.Save_Settings();
            mainframe.IsEnabled = false;
            mainframe.IsEnabled = true;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/yagdev";
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            using HttpClient client = new();
            UpdateBtn.IsEnabled = false;
            UpdateBtn.Content = "Updating...";
            if (Globals.IsBetaProgram)
            {
                using var s = await client.GetStreamAsync(Globals.UpdateZipBeta);
                using var fs = new FileStream("release.zip", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
            else
            {
                using var s = await client.GetStreamAsync(Globals.UpdateZip);
                using var fs = new FileStream("release.zip", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
            Process cmd = new();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.Arguments = "/C mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move release.zip ../ & cd .. & tar -xf release.zip & del /f /q release.zip & taskkill /f /im CompanionDisplayWinUI.exe & timeout 1 & rmdir /s /q Update & CompanionDisplayWinUI.exe";
            cmd.Start();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                Globals.BackgroundColorR = BackgroundColorPicker.Color.R;
                Globals.BackgroundColorG = BackgroundColorPicker.Color.G;
                Globals.BackgroundColorB = BackgroundColorPicker.Color.B;
                Globals.Save_Settings();
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
            }
        }

        private void UpdateToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (UpdateToggle.IsOn)
                {
                    Globals.IsBetaProgram = true;
                }
                else
                {
                    Globals.IsBetaProgram = false;
                }
                Globals.Save_Settings();
            }
        }

        private void AddButtonToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (AddButtonToggle.IsOn)
                {
                    Globals.HideAddButton = true;
                }
                else
                {
                    Globals.HideAddButton = false;
                }
                Globals.Save_Settings();
            }
        }

        private void StartupToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (StartupToggle.IsOn)
                {
                    try
                    {
                        object shStartup = (object)"startup";
                        WshShell shell = new();
                        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shStartup) + @"\Companion Display.lnk";
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                        shortcut.Description = "Comanion Display launch on startup object";
                        shortcut.TargetPath = System.IO.Path.Combine(System.IO.Path.GetFullPath(Environment.ProcessPath.ToString()));
                        shortcut.WorkingDirectory = System.IO.Path.Combine(System.IO.Path.GetFullPath(Environment.CurrentDirectory.ToString()));
                        shortcut.Save();
                        Globals.LaunchOnStartup = true;
                    }
                    catch
                    {
                        AddButtonToggle.IsOn = false;
                        Globals.LaunchOnStartup = false;
                    }
                }
                else
                {
                    object shStartup = (object)"startup";
                    WshShell shell = new();
                    string shortcutAddress = (string)shell.SpecialFolders.Item(ref shStartup) + @"\Companion Display.lnk";
                    if (System.IO.File.Exists(shortcutAddress))
                    {
                        System.IO.File.Delete(shortcutAddress);
                    }
                    Globals.LaunchOnStartup = false;
                }
                Globals.Save_Settings();
            }
        }

        private void LockToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                if (LockToggle.IsOn)
                {
                    Globals.LockLayout = true;
                }
                else
                {
                    Globals.LockLayout = false;
                }
                Globals.Save_Settings();
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.paypal.com/paypalme/dinisp25";
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                Globals.SleepColorR = SleepModeColor.Color.R;
                Globals.SleepColorG = SleepModeColor.Color.G;
                Globals.SleepColorB = SleepModeColor.Color.B;
                Globals.Save_Settings();
            }
        }

        private void OvrColorSleepMode_Checked(object sender, RoutedEventArgs e)
        {
            Globals.OverrideColor = true;
            Globals.Save_Settings();
        }

        private void OvrColorSleepMode_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.OverrideColor = false;
            Globals.Save_Settings();
        }

        private void Opacity_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Globals.sleepModeOpacity = (sender as Slider).Value;
            Globals.Save_Settings();
        }

        private void SearchEngineCust_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if((sender as TextBox).Text != "")
                {
                    Globals.SearchEngine = new Uri("https://" + (sender as TextBox).Text.Replace("https://", "").Replace("http://", ""));
                }
                else
                {
                    Globals.SearchEngine = new Uri("https://www.google.com/");
                }
                Globals.Save_Settings();
            }
            catch
            {

            }
        }

        private void NewTabBehavior_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.NewTabBehavior = (sender as ComboBox).SelectedIndex;
            Globals.Save_Settings();
        }

        private void SearchEngineCust_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        private async void ResetTwitch_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = "/C taskkill /f /im msedgewebview2.exe";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            resetTwitch.CoreWebView2Initialized += DeleteCookies;
            var environmentOptions = new CoreWebView2EnvironmentOptions();
            environmentOptions.AreBrowserExtensionsEnabled = true;
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync("", "", environmentOptions);
            await resetTwitch.EnsureCoreWebView2Async(environment);
            resetTwitch.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.59.0_0"));
            resetTwitch.Source = new Uri("https://google.com/");
        }

        private async void DeleteCookies(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Web.WebView2.Core.CoreWebView2Cookie> cookies = await resetTwitch.CoreWebView2.CookieManager.GetCookiesAsync("https://twitch.tv");
            foreach(var Cookie in cookies)
            {
                resetTwitch.CoreWebView2.Profile.CookieManager.DeleteCookie(Cookie);
            }
        }

        private void Opacity_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.sleepModeOpacity = (sender as Slider).Value;
            Globals.Save_Settings();
        }

        private void OBSIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if((sender as TextBox).Text == "")
            {
                Globals.obsIP = (sender as TextBox).PlaceholderText;
            }
            else
            {
                Globals.obsIP = (sender as TextBox).Text;
            }
            Thread thread = new(Save_SettingsOBS);
            thread.Start();
        }

        private void ReconnectOBS_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new(Globals.obsControls.manualConnectReq);
            thread.Start();
            System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
            (sender as Button).IsEnabled = false;
            timer.Elapsed += (sender, args) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!Globals.obsControls.connectionSuccessful)
                    {
                        OBSStatus.Text = "Status: Disconnected";
                        ReconnectOBS.Content = "Connection failed, try again.";
                        timer.Dispose();
                    }
                    else
                    {
                        OBSStatus.Text = "Status: Connected";
                        ReconnectOBS.Content = "Reconnect";
                    }
                    ReconnectOBS.IsEnabled = true;
                });
            };
        }

        private void OBSPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Globals.obsPass = (sender as PasswordBox).Password;
            Thread thread = new(Save_SettingsOBS);
            thread.Start();
        }

        private void SetupBtn_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            navviewparent.IsPaneVisible = false;
            frame.Navigate(typeof(SetupStep0), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
