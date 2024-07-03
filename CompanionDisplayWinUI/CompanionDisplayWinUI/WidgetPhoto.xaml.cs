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
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Core;
using Windows.Storage;
using System.Threading;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhoto : Page
    {
        public WidgetPhoto()
        {
            this.InitializeComponent();
        }
        Frame frame;
        string path = "";
        bool Cooldown = false;
        int ItemCount = 0, ChosenImage = 0;
        public void UpdateUI()
        {
            try
            {
                string SF = File.ReadAllText(Globals.PhotoConfigFile);
                switch (SF)
                {
                    case "False":
                        break;
                        Globals.SmartFlipToggle = false;
                    case "True":
                        Globals.SmartFlipToggle = true;
                        break;
                }
            }
            catch
            {

            }
            int pos = path.LastIndexOf("/") + 1;
            if ((path.Substring(pos, path.Length - pos)).Contains("."))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Image image = new Image();
                    BitmapImage bitmapImage = new BitmapImage();
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bitmapImage.SetSource(stream.AsRandomAccessStream());
                    }
                    image.Source = bitmapImage;
                    image.Stretch = Stretch.UniformToFill;
                    image.Name = path;
                    FlipViewImages.Items.Add(image);
                });
            }
            else
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    foreach (var file in d.GetFiles("*"))
                    {
                        string imagePath = file.FullName;
                        if (MimeMapping.MimeUtility.GetMimeMapping(imagePath).StartsWith("image/"))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    Image image = new Image();
                                    image.Stretch = Stretch.UniformToFill;
                                    image.Name = imagePath;
                                    image.Loaded += Image_Loaded;
                                    image.Unloaded += Image_Unloaded;
                                    FlipViewImages.Items.Add(image);
                                    ItemCount++;
                                }
                                catch (Exception ex)
                                {
                                }
                            });
                        }
                    }
                }
                catch { }
            }
            Thread thread = new(SmartFlip);
            thread.Start();
        }
        private BitmapImage bitmapImage = new BitmapImage();
        public void SmartFlip()
        {
            Random random = new Random();
            ChosenImage = random.Next(0, ItemCount);
            DispatcherQueue.TryEnqueue(() =>
            {
                FlipViewImages.SelectedIndex = ChosenImage;
            });
            Thread.Sleep(60000);
            if (Globals.SmartFlipToggle)
            {
                Thread thread = new(SmartFlip);
                thread.Start();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            frame = this.Parent as Frame;
            try
            {
                path = frame.Tag.ToString();
            }
            catch { }
            Thread thread = new Thread(UpdateUI);
            thread.Start();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            FlipViewImages.IsEnabled = false;
            try
            {
                var senderImage = sender as Image;
                senderImage.Stretch = Stretch.UniformToFill;
                senderImage.Source = bitmapImage;
            }
            catch
            {

            }
            FlipViewImages.IsEnabled = true;
        }
        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            var senderImage = sender as Image;
            if (senderImage != null)
            {
                string oldname = senderImage.Name;
                senderImage = null;
                senderImage = new Image();
                senderImage.Name = oldname;
                senderImage.Loaded += Image_Loaded;
            }
        }

        private void FlipViewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderImage = (sender as FlipView).SelectedItem as Image;
                bitmapImage.UriSource = new System.Uri(senderImage.Name);
            }
            catch
            {

            }
        }
    }
}
