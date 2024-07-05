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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetApps : Page
    {
        public int i = 1;
        public NORC_WidgetApps()
        {
            this.InitializeComponent();
            string WidgetOrder = "";
            try
            {
                WidgetOrder = File.ReadAllText("Config/AppOrder.crlh");
            }
            catch
            {
                File.AppendAllText("Config/AppOrder.crlh", "");
            }
            foreach (string line in WidgetOrder.Split('\n'))
            {
                if (line.Length != 0)
                {
                    string fix = line.Replace("\r", "");
                    Grid grid = new()
                    {
                        Name = "AppGrid" + i,
                        Height = 77,
                        Width = 77
                    };
                    Microsoft.UI.Xaml.Controls.Image appIcon = new Microsoft.UI.Xaml.Controls.Image
                    {
                        Name = "App" + i + "Icon",
                        Stretch = Stretch.Uniform
                    };
                    Button button1 = new() { Name = "App" + i , Content = appIcon, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Tag = line };
                    button1.Loaded += GetIcons;
                    button1.Click += OpenShortcut;
                    button1.RightTapped += Button_RightTapped;
                    grid.Children.Add(button1);
                    BasicGridView.Items.Add(grid);
                    i++;
                }
            }
            if (BasicGridView.Items.Count == 0)
            {
                EmptyApps.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyApps.Visibility = Visibility.Collapsed;
            }
            try
            {
                File.Delete("Config/AppOrder.crlh");
            }
            catch { }
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    File.AppendAllText("Config/AppOrder.crlh", childControl3.Tag.ToString() + "\n");
                }
            }
        }
        private void SetIcon(object sender, RightTappedRoutedEventArgs e)
        {
            
        }
        private void OpenShortcut(object sender, RoutedEventArgs e)
        {
            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = "/C \"" + ((Button)sender).Tag + "\""
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem thirdItem = new MenuFlyoutItem { Text = "Replace", Name = senderElement.Name + "Replace" };
            MenuFlyoutItem fourthItem = new MenuFlyoutItem { Text = "Remove", Name = senderElement.Name + "Remove" };
            thirdItem.Click += MenuFlyoutItem2_Click;
            fourthItem.Click += MenuFlyoutItem4_Click;
            myFlyout.Items.Add(thirdItem);
            myFlyout.Items.Add(fourthItem);
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.IsEnabled = false;
        }
        private async void MenuFlyoutItem4_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Button)BasicGridView.FindName(senderElement.Name.Remove(senderElement.Name.Length - 6, 6));
            var GridControl = childControl.Parent as Grid;
            var GridViewControl = GridControl.Parent as GridView;
            GridViewControl.Items.Remove(GridControl);
            if (BasicGridView.Items.Count == 0)
            {
                EmptyApps.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyApps.Visibility = Visibility.Collapsed;
            }
            string Apps = "";
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    Apps = Apps + childControl3.Tag.ToString() + "\n";
                }
            }
            File.WriteAllText("Config/AppOrder.crlh", Apps);
        }
        private async void MenuFlyoutItem2_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Button)BasicGridView.FindName(senderElement.Name.Remove(senderElement.Name.Length - 7, 7));
            AppPicker appPicker = new();
            string btntag = appPicker.SetIcon();
            if(btntag != "")
            {
                childControl.Tag = btntag;
                var childControl2 = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(childControl.Name + "Icon");
                Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(childControl.Tag.ToString());
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream iconStream = new MemoryStream())
                {
                    icon.Save(iconStream);
                    iconStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                }
                childControl2.Source = bitmapImage;
            }
            string Apps = "";
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    Apps = Apps + childControl3.Tag.ToString() + "\n";
                }
            }
            File.WriteAllText("Config/AppOrder.crlh", Apps);
        }
        private async void GetIcons(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(((Button)sender).Name + "Icon");
            try
            {
                childControl.Source = await GetThumbnailImageAsync(((Button)sender).Tag.ToString());
            }
            catch
            {
                try
                {
                    Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(((Button)sender).Tag.ToString());
                    using (Bitmap bitmap = icon.ToBitmap())
                    using (MemoryStream iconStream = new MemoryStream())
                    {
                        bitmap.Save(iconStream, ImageFormat.Png);
                        iconStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                        childControl.Source = bitmapImage;
                    }
                }
                catch
                {

                }
            }


        }
        public async Task<BitmapImage> GetThumbnailImageAsync(string filePath)
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
            const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
            const uint requestedSize = 100; // Size of the thumbnail

            StorageItemThumbnail thumbnail = await storageFile.GetThumbnailAsync(thumbnailMode, requestedSize);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            return bitmapImage;
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Remove Widget", Name = senderElement.Name + "Flyout" };
            MenuFlyoutItem fifthItem = new MenuFlyoutItem { Text = "Edit", Name = senderElement.Name + "Edit" };
            firstItem.Click += MenuFlyoutItem_Click;
            fifthItem.Click += MenuFlyoutItem5_Click;
            myFlyout.Items.Add(firstItem);
            if(MainGrid.FindName("AddWidget") == null)
            {
                myFlyout.Items.Add(fifthItem);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private async void MenuFlyoutItem5_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    childControl.IsHitTestVisible = false;
                }
            }
            Button button = new()
            {
                Name = "AddWidget",
                Height = 77,
                Width = 77,
                Content = "+",
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"),
                FontSize = 32,
                AllowDrop = true,
            };
            Button button2 = new()
            {
                Name = "Finish",
                Height = 77,
                Width = 77,
                Content = "\ue73e",
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"),
                FontSize = 32,
                AllowDrop = true,
            };
            EmptyApps.Visibility = Visibility.Collapsed;
            button.Click += Button_Click_1;
            button2.Click += Button_Click;
            BasicGridView.Items.Add(button);
            BasicGridView.Items.Add(button2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var childControl = (Microsoft.UI.Xaml.Controls.Button)MainGrid.FindName("AddWidget");
            var childControl2 = (Microsoft.UI.Xaml.Controls.Button)MainGrid.FindName("Finish");
            BasicGridView.Items.Remove(childControl);
            BasicGridView.Items.Remove(childControl2);
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    childControl3.IsHitTestVisible = true;
                }
            }
            if(BasicGridView.Items.Count == 0)
            {
                EmptyApps.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyApps.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppPicker appPicker = new();
            string btntag = appPicker.SetIcon();
            if (btntag != "")
            {
                Grid grid = new()
                {
                    Name = "AppGrid" + i,
                    Height = 77,
                    Width = 77
                };
                Microsoft.UI.Xaml.Controls.Image appIcon = new Microsoft.UI.Xaml.Controls.Image
                {
                    Name = "App" + i + "Icon",
                    Stretch = Stretch.Uniform
                };
                Button button1 = new() { Name = "App" + i, Content = appIcon, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Tag = btntag };
                button1.Click += OpenShortcut;
                button1.RightTapped += Button_RightTapped;
                button1.Loaded += GetIcons;
                button1.IsHitTestVisible = false;
                grid.Children.Add(button1);
                BasicGridView.Items.Add(grid);
                i++;
            }
            if (BasicGridView.Items.Count == 0)
            {
                EmptyApps.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyApps.Visibility = Visibility.Collapsed;
            }
            string Apps = "";
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    Apps = Apps + childControl3.Tag.ToString() + "\n";
                }
            }
            File.WriteAllText("Config/AppOrder.crlh", Apps);
        }

        private void BasicGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            string Apps = "";
            foreach (UIElement item in BasicGridView.Items)
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    Apps = Apps + childControl3.Tag.ToString() + "\n";
                }
            }
            File.WriteAllText("Config/AppOrder.crlh", Apps);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (isElevated == true)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BasicGridView.CanReorderItems = false;
                    BasicGridView.CanDragItems = false;
                });
            }
        }
    }
}
