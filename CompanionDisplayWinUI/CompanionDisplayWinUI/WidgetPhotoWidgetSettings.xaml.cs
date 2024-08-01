using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhotoWidgetSettings : Page
    {
        public WidgetPhotoWidgetSettings()
        {
            this.InitializeComponent();
            try
            {
                string SF = File.ReadAllText(Globals.PhotoConfigFile);
                switch (SF)
                {
                    case "false":
                        SmartFlipToggle.IsOn = false;
                        break;
                    case "true":
                        SmartFlipToggle.IsOn = true;
                        break;
                }
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(WidgetPhoto));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SmartFlipToggle.IsOn)
            {
                Globals.SmartFlipToggle = true;
            }
            else
            {
                Globals.SmartFlipToggle = false;
            }
            var parent = this.Parent as Frame;
            parent.Tag = DirectoryTextBox.Text;
            var frame = this.Parent as Frame;
            frame.IsEnabled = false;
            frame.IsEnabled = true;
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.PhotoConfigFile));
            File.WriteAllText(Globals.PhotoConfigFile, SmartFlipToggle.IsOn.ToString());
            frame.Navigate(typeof(WidgetPhoto));
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AppPicker appPicker = new();
            string btntag = appPicker.SetIcon();
            DirectoryTextBox.Text = btntag;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AppPicker appPicker2 = new();
            string btntag2 = appPicker2.SetIcon2();
            DirectoryTextBox.Text = btntag2;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = this.Parent as Frame;
                DirectoryTextBox.Text = parent.Tag.ToString();
                if (Globals.SmartFlipToggle)
                {
                    SmartFlipToggle.IsOn = true;
                }
            }
            catch
            {

            }
        }

        private void SmartFlipToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (SmartFlipToggle.IsOn)
            {
                Globals.SmartFlipToggle = true;
            }
            else
            {
                Globals.SmartFlipToggle = false;
            }
        }
    }
}
