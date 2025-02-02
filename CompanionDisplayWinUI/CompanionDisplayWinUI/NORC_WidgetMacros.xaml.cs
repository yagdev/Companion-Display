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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using System.Drawing;
using Microsoft.UI.Xaml.Shapes;
using System.Drawing.Imaging;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Windows.System;
using Microsoft.UI.Composition;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetMacros : Page
    {
        public NORC_WidgetMacros()
        {
            this.InitializeComponent();
        }
        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new();
            MenuFlyoutItem thirdItem = new() { Text = "Replace Image", Name = senderElement.Name + "ACTION1" };
            MenuFlyoutItem fourthItem = new() { Text = "Remove Image", Name = senderElement.Name + "ACTION2" };
            thirdItem.Click += ReplaceImageClick;
            fourthItem.Click += RemoveImageClick;
            myFlyout.Items.Add(thirdItem);
            myFlyout.Items.Add(fourthItem);
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((this.Parent as Frame).Parent as Grid == null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = "Remove Widget", Name = senderElement.Name + "Flyout" };
                MenuFlyoutItem thirdItem = new() { Text = "Toggle Pin", Name = senderElement.Name + "Pin" };
                MenuFlyoutItem fourthItem = new() { Text = "Picture in Picture", Name = senderElement.Name + "PiP" };
                firstItem.Click += MenuFlyoutItem_Click;
                thirdItem.Click += PinButton;
                fourthItem.Click += PiPButton;
                if (!(this.Frame.Parent is FlipView))
                {
                    myFlyout.Items.Add(thirdItem);
                    myFlyout.Items.Add(fourthItem);
                }
                myFlyout.Items.Add(firstItem);
                myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
            }
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }

        private void PiPButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "pip";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }

        private void PinButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "pin";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }

        private void RemoveImageClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(senderElement.Name.Remove(senderElement.Name.Length - 7, 7) + "_Image");
            childControl.Tag = "";
            childControl.Source = null;
            ((childControl.Parent as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Visible;
            saveItems();
        }

        private async void ReplaceImageClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(senderElement.Name.Remove(senderElement.Name.Length - 7, 7) + "_Image");
            AppPicker appPicker = new();
            string btntag = appPicker.SetIcon();
            if (btntag != "")
            {
                childControl.Tag = btntag;
                childControl.Source = new BitmapImage(new Uri(childControl.Tag.ToString()));
                ((childControl.Parent as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Collapsed;
            }
            saveItems();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 1;
            string LoadImages = "";
            try
            {
                LoadImages = File.ReadAllText("Config/MacroThumbs.crlh");
            }
            catch
            {
                Directory.CreateDirectory("Config");
                File.AppendAllText("Config/MacroThumbs.crlh", "");
            }
            foreach (string line in LoadImages.Replace("\r", "").Split('\n'))
            {
                try
                {
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag = line;
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Source = new BitmapImage(new Uri((((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag.ToString()));
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Collapsed;
                }
                catch
                {
                    try
                    {
                        (((MainGrid.Children[i] as Button).Content as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Visible;
                    }
                    catch
                    {

                    }
                }
                i++;
            }
        }
        private void saveItems()
        {
            string finalFile = "";
            foreach(IVisualElement visualElement in MainGrid.Children)
            {
                if(visualElement as Button != null)
                {
                    try
                    {
                        finalFile = finalFile + (((visualElement as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag.ToString() + Environment.NewLine;
                    }
                    catch
                    {
                        finalFile = finalFile + Environment.NewLine;
                    }
                }
            }
            File.Delete("Config/MacroThumbs.crlh");
            File.AppendAllText("Config/MacroThumbs.crlh", finalFile);
        }
        public static async Task<BitmapImage> GetThumbnailImageAsync(string filePath)
        {
            BitmapImage bitmapImage = new();
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
            const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
            const uint requestedSize = 100; // Size of the thumbnail
            using (StorageItemThumbnail thumbnail = await storageFile.GetThumbnailAsync(thumbnailMode, requestedSize))
            {
                bitmapImage.SetSource(thumbnail);
            }
            return bitmapImage;
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
        private void F13Btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VirtualKey key = (VirtualKey)int.Parse(((Button)sender).Tag.ToString());
            PressKey((VirtualKey)int.Parse(((Button)sender).Tag.ToString()), up: false);
            PressKey((VirtualKey)int.Parse(((Button)sender).Tag.ToString()), up: true);
        }
    }
}
