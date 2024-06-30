﻿using CompanionDisplayWinUI.Properties;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            try
            {
                Directory.CreateDirectory("Config");
                string WidgetOrder = File.ReadAllText("Config/GlobalSettings.crlh");
                using StringReader readerconfig = new(WidgetOrder);
                Globals.ColorSchemeSelect = int.Parse(readerconfig.ReadLine());
                Globals.InjectCustomAccent = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentR = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentG = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentB = int.Parse(readerconfig.ReadLine());
                Globals.Backdrop = int.Parse(readerconfig.ReadLine());
                Globals.BackgroundLink = readerconfig.ReadLine();
                Globals.Wallpaper = readerconfig.ReadLine();
                Globals.Blur = bool.Parse(readerconfig.ReadLine());
                Globals.StealFocus = bool.Parse(readerconfig.ReadLine());
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            try
            {
                switch (Globals.ColorSchemeSelect)
                {
                    case (0):
                        break;
                    case (1):
                        SetAppTheme(ElementTheme.Dark);
                        break;
                    case (2):
                        SetAppTheme(ElementTheme.Light);
                        break;
                }
                if (Globals.InjectedCustomAccent == false)
                {
                    var customResources = new ResourceDictionary
                    {
                        Source = new Uri("ms-appx:///AccentOverride.xaml")
                    };
                    Application.Current.Resources.MergedDictionaries.Add(customResources);
                    Globals.InjectedCustomAccent = true;
                }
                switch (Globals.InjectCustomAccent)
                {
                    case 0:
                        RevertToSystemAccentColor();
                        break;
                    case 1:
                        SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                        break;
                }
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
                File.WriteAllText("Config/GlobalSettings.crlh", "");
            }
        }
        internal Window m_window;
        public void SetAppTheme(ElementTheme theme)
        {
            if (m_window.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }
        }
        public void SetAccentColor(Color color)
        {
            var customResourceDictionary = new ResourceDictionary();
            customResourceDictionary = Application.Current.Resources;
            customResourceDictionary["SystemAccentColor"] = color;
            customResourceDictionary["SystemAccentColorLight1"] = color;
            customResourceDictionary["SystemAccentColorLight2"] = color;
            customResourceDictionary["SystemAccentColorLight3"] = color;
            customResourceDictionary["SystemAccentColorDark1"] = color;
            customResourceDictionary["SystemAccentColorDark2"] = color;
            Application.Current.Resources = customResourceDictionary;
        }
        public void RevertToSystemAccentColor()
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            var systemAccentColor = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            var customResourceDictionary = new ResourceDictionary();
            customResourceDictionary = Application.Current.Resources;
            customResourceDictionary["SystemAccentColor"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            customResourceDictionary["SystemAccentColorLight1"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight1);
            customResourceDictionary["SystemAccentColorLight2"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2);
            customResourceDictionary["SystemAccentColorLight3"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight3);
            customResourceDictionary["SystemAccentColorDark1"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentDark1);
            customResourceDictionary["SystemAccentColorDark2"] = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentDark2);
            Application.Current.Resources = customResourceDictionary;
        }
    }
}