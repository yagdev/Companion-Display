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
            LoadFinish = true;
        }
        private Frame mainframe;
        private Color colorTemp;
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
                if(ColorSchemeSelect.SelectedIndex != null)
                {
                    Globals.ColorSchemeSelect = ColorSchemeSelect.SelectedIndex;
                }
                Thread thread = new Thread(Save_Settings);
                thread.Start();
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
                        (Application.Current as App)?.RevertToSystemAccentColor();
                        break;
                    case "Custom":
                        (Application.Current as App)?.SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                        break;
                }
                if (AccentSelect.SelectedIndex != null)
                {
                    Globals.InjectCustomAccent = AccentSelect.SelectedIndex;
                }
                Thread thread = new Thread(Save_Settings);
                thread.Start();
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
                Thread thread = new Thread(Save_Settings);
                thread.Start();
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if(LoadFinish == true)
            {
                string Theme = e.AddedItems[0].ToString();
                switch (Theme)
                {
                    case "Acrylic":
                        Globals.Backdrop = 0;
                        break;
                    case "Mica":
                        Globals.Backdrop = 1;
                        break;
                    case "Image":
                        Globals.Backdrop = 2;
                        break;
                }
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
                if (BackdropSelect.SelectedIndex != null)
                {
                    Globals.Backdrop = BackdropSelect.SelectedIndex;
                }
                Thread thread = new Thread(Save_Settings);
                thread.Start();
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
                Thread thread = new Thread(Save_Settings);
                thread.Start();
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
                Thread thread = new Thread(Save_Settings);
                thread.Start();
            }
        }
        
        private void Save_Settings()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                string settingsfile = Globals.ColorSchemeSelect + "\n" + Globals.InjectCustomAccent + "\n" + Globals.ColorSchemeSelectAccentR + "\n" + Globals.ColorSchemeSelectAccentG + "\n" + Globals.ColorSchemeSelectAccentB + "\n" + Globals.Backdrop + "\n" + Globals.BackgroundLink + "\n" + Globals.Wallpaper + "\n" + Globals.Blur + "\n" + Globals.StealFocus + "\n" + Globals.BackgroundColorR + "\n" + Globals.BackgroundColorG + "\n" + Globals.BackgroundColorB;
                File.WriteAllText("Config/GlobalSettings.crlh", settingsfile);
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
                Thread thread = new Thread(Save_Settings);
                thread.Start();
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
            }
        }

        private void BackgroundLink_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.BackgroundLink = BackgroundLink.Text;
            Thread thread = new Thread(Save_Settings);
            thread.Start();
            mainframe.IsEnabled = false;
            mainframe.IsEnabled = true;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/yagdev";
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                UpdateBtn.IsEnabled = false;
                UpdateBtn.Content = "Updating...";
                client.DownloadFile(Globals.UpdateZip, "release.zip");
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.Arguments = "/C mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move release.zip ../ & cd .. & tar -xf release.zip & del /f /q release.zip & taskkill /f /im CompanionDisplayWinUI.exe & timeout 1 & rmdir /s /q Update & CompanionDisplayWinUI.exe";
                cmd.Start();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (LoadFinish == true)
            {
                Globals.BackgroundColorR = BackgroundColorPicker.Color.R;
                Globals.BackgroundColorG = BackgroundColorPicker.Color.G;
                Globals.BackgroundColorB = BackgroundColorPicker.Color.B;
                Thread thread = new Thread(Save_Settings);
                thread.Start();
                mainframe.IsEnabled = false;
                mainframe.IsEnabled = true;
            }
        }
    }
}
