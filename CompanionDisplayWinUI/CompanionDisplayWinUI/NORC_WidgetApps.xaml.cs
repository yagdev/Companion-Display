using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.Storage.FileProperties;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CompanionDisplayWinUI.ClassImplementations;

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
        }
        private void UpdateUI()
        {
            string WidgetOrder = "";
            try
            {
                WidgetOrder = File.ReadAllText("Config/AppOrder.crlh");
            }
            catch
            {
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (string line in WidgetOrder.Split('\n'))
                {
                    if (line.Length != 0)
                    {
                        string fix = line.Replace("\r", "");
                        CreateEntry(true, fix);
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
            });
        }
        private void ElevationCheck()
        {
            if (Globals.IsAdmin)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BasicGridView.CanReorderItems = false;
                    BasicGridView.CanDragItems = false;
                });
            }
        }
        private void OpenShortcut(object sender, RoutedEventArgs e)
        {
            using Process process = new();
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
            MenuFlyout myFlyout = new();
            MenuFlyoutItem thirdItem = new () { Text = AppStrings.widgetReplace, Name = senderElement.Name + "Replace" };
            MenuFlyoutItem fourthItem = new() { Text = AppStrings.widgetRemove, Name = senderElement.Name + "Remove" };
            thirdItem.Click += MenuFlyoutItem2_Click;
            fourthItem.Click += MenuFlyoutItem4_Click;
            myFlyout.Items.Add(thirdItem);
            myFlyout.Items.Add(fourthItem);
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }
        private void MenuFlyoutItem4_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Button)BasicGridView.FindName(senderElement.Name[..^6]);
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
            SaveList();
        }
        private void MenuFlyoutItem2_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Button)BasicGridView.FindName(senderElement.Name[..^7]);
            string btntag = FileFolderPicker.OpenFileDialog(false)[0];
            if(btntag != "")
            {
                childControl.Tag = btntag;
                var childControl2 = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(childControl.Name + "Icon");
                BitmapImage bitmapImage = new();
                using (Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(childControl.Tag.ToString()))
                using (MemoryStream iconStream = new ())
                {
                    icon.Save(iconStream);
                    iconStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                }
                childControl2.Source = bitmapImage;
                SaveList();
            }
            
        }
        private void SaveList()
        {
            string Apps = "";
            foreach (UIElement item in BasicGridView.Items.Cast<UIElement>())
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
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(((Button)sender).Name + "Icon");
            if (childControl.Source == null)
            {
                try
                {
                    childControl.Source = await GetThumbnailImageAsync(((Button)sender).Tag.ToString());
                }
                catch
                {
                    try
                    {
                        using Icon icon = GetFileIcon(((Button)sender).Tag.ToString(), true);
                        using Bitmap bitmap = icon.ToBitmap();
                        using MemoryStream iconStream = new();
                        bitmap.Save(iconStream, ImageFormat.Png);
                        iconStream.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                        childControl.Source = bitmapImage;
                    }
                    catch
                    {

                    }
                }
            }
        }
        private void CreateEntry(bool isHitTestVisible, object tag)
        {
            Grid grid = new()
            {
                Name = "AppGrid" + i,
                CornerRadius = new CornerRadius(8),
                Height = 77,
                Width = 77,
            };
            Grid grid0 = new()
            {
                CornerRadius = new CornerRadius(8),
            };
            Microsoft.UI.Xaml.Controls.Image appIcon = new()
            {
                Name = "App" + i + "Icon",
                Stretch = Stretch.Uniform
            };
            grid0.Children.Add(appIcon);
            Button button1 = Launchpad.CreateLaunchPadButton(grid0, new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"), Name = "App" + i);
            button1.HorizontalAlignment = HorizontalAlignment.Stretch;
            button1.VerticalAlignment = VerticalAlignment.Stretch;
            button1.Tag = tag;
            button1.Click += OpenShortcut;
            button1.RightTapped += Button_RightTapped;
            button1.Loaded += GetIcons;
            button1.IsHitTestVisible = isHitTestVisible;
            grid.Children.Add(button1);
            BasicGridView.Items.Add(grid);
        }
        public static Icon GetFileIcon(string fileName, bool largeIcon)
        {
            SHFILEINFO shinfo = new();
            uint flags = SHGFI_ICON | (largeIcon ? 0x4 : SHGFI_SMALLICON);

            IntPtr hImg = SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

            if (hImg != IntPtr.Zero)
            {
                return Icon.FromHandle(shinfo.hIcon);
            }

            return null;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        const uint SHGFI_ICON = 0x000000100;
        const uint SHGFI_SMALLICON = 0x4; // Small icon
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
        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((this.Parent as Frame).Parent as Grid == null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = AppStrings.removeWidget, Name = senderElement.Name + "Flyout" };
                MenuFlyoutItem fifthItem = new() { Text = AppStrings.widgetEdit, Name = senderElement.Name + "Edit" };
                MenuFlyoutItem sixthItem = new() { Text = AppStrings.removeAllShortcuts, Name = senderElement.Name + "RemoveAll" };
                MenuFlyoutItem thirdItem = new() { Text = AppStrings.widgetPinUnpin, Name = senderElement.Name + "Pin" };
                MenuFlyoutItem fourthItem = new() { Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" };
                firstItem.Click += MenuFlyoutItem_Click;
                fifthItem.Click += MenuFlyoutItem5_Click;
                thirdItem.Click += PinButton;
                fourthItem.Click += PiPButton;
                sixthItem.Click += RemoveAll;
                if (this.Frame.Parent is not FlipView)
                {
                    if (MainGrid.FindName("AddWidget") == null)
                    {
                        myFlyout.Items.Add(fifthItem);
                        myFlyout.Items.Add(sixthItem);
                    }
                    myFlyout.Items.Add(thirdItem);
                    myFlyout.Items.Add(fourthItem);
                }
                myFlyout.Items.Add(firstItem);
                myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
            }
        }
        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Clear();
            EmptyApps.Visibility = Visibility.Visible;
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
        private void MenuFlyoutItem5_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in BasicGridView.Items.Cast<UIElement>())
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl = (Microsoft.UI.Xaml.Controls.Button)grid.FindName(grid.Name.Replace("Grid", ""));
                    childControl.IsHitTestVisible = false;
                }
            }
            Button button = Launchpad.CreateLaunchPadButton("+", new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"), "AddWidget");
            Button button2 = Launchpad.CreateLaunchPadButton("\ue73e", new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"), "Finish");
            EmptyApps.Visibility = Visibility.Collapsed;
            button.Click += Button_Click_1;
            button2.Tag = button;
            button2.Click += Button_Click;
            BasicGridView.Items.Insert(0, button2);
            BasicGridView.Items.Insert(0, button);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Remove(sender as Button);
            BasicGridView.Items.Remove((sender as Button).Tag);
            foreach (UIElement item in BasicGridView.Items.Cast<UIElement>())
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
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (string btntag in FileFolderPicker.OpenFileDialog(true))
                {
                    if (btntag != "")
                    {
                        CreateEntry(false, btntag);
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
                }
                SaveList();
            });
        }
        private void BasicGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            SaveList();
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                Thread thread = new(UpdateUI);
                thread.Start();
                Thread thread0 = new(ElevationCheck);
                thread0.Start();
                FTU = false;
            }
        }
    }
}
