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
using Microsoft.Web.WebView2.Core;
using Microsoft.VisualBasic.Devices;
using System.Drawing;
using Windows.System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using ABI.System;
using Windows.UI.WebUI;
using System.Web;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserTab : Page
    {
        public BrowserTab()
        {
            this.InitializeComponent();
            if (Globals.IsAdmin)
            {
                BrowserTabs.CanDrag = false;
                BrowserTabs.CanDragTabs = false;
                BrowserTabs.CanReorderTabs = false;
            }
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private void MenuFlyoutItem5_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl = (Microsoft.UI.Xaml.Controls.Button)grid.Children[0];
                    childControl.IsHitTestVisible = false;
                }
            }
            Button button = new()
            {
                Name = "AddWidget",
                Height = 200,
                Width = 200,
                Content = "+",
                CornerRadius = new CornerRadius(10),
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"),
                FontSize = 72,
                AllowDrop = true,
            };
            Button button2 = new()
            {
                Name = "Finish",
                Height = 200,
                Width = 200,
                Content = "\ue73e",
                CornerRadius = new CornerRadius(10),
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"),
                FontSize = 72,
                AllowDrop = true,
            };
            NoItem.Visibility = Visibility.Collapsed;
            button.Click += Button_Click_1a;
            button2.Click += Button_Click0;
            Launchpad.Items.Add(button);
            Launchpad.Items.Add(button2);
        }
        private void MenuFlyoutItem5a_Click(object sender, RoutedEventArgs e)
        {
            Launchpad.Items.Remove((sender as MenuFlyoutItem).Tag as Grid);
            if(Launchpad.Items.Count == 0)
            {
                NoItem.Visibility = Visibility.Visible;
            }
            else
            {
                NoItem.Visibility = Visibility.Collapsed;
            }
            Launchpad_LayoutUpdated(Launchpad, null);
        }
        private async void Button_Click_1a(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Add Launchpad Tile";
            dialog.PrimaryButtonText = "Add";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            TextBox textBox = new()
            {
                Margin = new Thickness(0),
                Height = 30,
                PlaceholderText = "Insert URL here"
            };
            dialog.Content = textBox;
            dialog.Tag = (sender as Button).Tag;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string URL = (dialog.Content as TextBox).Text;
                System.Uri myUri = new System.Uri("https://" + URL);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new System.Uri("https://www.google.com/s2/favicons?domain=" + myUri.Host + "&sz=256");
                Microsoft.UI.Xaml.Controls.Image image = new Microsoft.UI.Xaml.Controls.Image();
                image.Source = bitmapImage;
                Grid grid0 = new();
                Grid grid1 = new()
                {
                    CornerRadius = new CornerRadius(10),
                    Width = 150,
                    Height = 150
                };
                grid1.Children.Add(image);
                Button button = new()
                {
                    Tag = myUri,
                    Content = grid1,
                    Height = 200,
                    Width = 200,
                    CornerRadius = new CornerRadius(10),
                    IsHitTestVisible = false,
                };
                grid0.RightTapped += IndividualItemRC;
                grid0.Children.Add(button);
                button.Click += newTabLaunchpad;
                Launchpad.Items.Add(grid0);
                Launchpad_LayoutUpdated(Launchpad, null);
            }
        }

        private void IndividualItemRC(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new();
            MenuFlyoutItem fifthItem = new() { Text = "Remove", Name = senderElement.Name + "Remove", Tag = sender };
            fifthItem.Click += MenuFlyoutItem5a_Click;
            if (Launchpad.FindName("AddWidget") == null)
            {
                myFlyout.Items.Add(fifthItem);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }

        private void newTabLaunchpad(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            TabViewItem tab = CreateNewTVI("New Tab", "New Tab", (sender as Button).Tag as System.Uri, navviewparent);
            BrowserTabs.TabItems.Remove(BrowserTabs.SelectedItem);
            BrowserTabs.TabItems.Add(tab);
            BrowserTabs.SelectedItem = tab;
        }

        private void GridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new();
            MenuFlyoutItem fifthItem = new() { Text = "Edit" };
            fifthItem.Tag = sender;
            fifthItem.Click += MenuFlyoutItem5_Click;
            if (Launchpad.FindName("AddWidget") == null)
            {
                myFlyout.Items.Add(fifthItem);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private void Button_Click0(object sender, RoutedEventArgs e)
        {
            var childControl = (Microsoft.UI.Xaml.Controls.Button)Launchpad.FindName("AddWidget");
            var childControl2 = (Microsoft.UI.Xaml.Controls.Button)Launchpad.FindName("Finish");
            Launchpad.Items.Remove(childControl);
            Launchpad.Items.Remove(childControl2);
            foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
            {
                if (item is Grid)
                {
                    try
                    {
                        Grid grid = item as Grid;
                        var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.Children[0];
                        childControl3.IsHitTestVisible = true;
                    }
                    catch { }
                }
            }
            if (Launchpad.Items.Count == 0)
            {
                NoItem.Visibility = Visibility.Visible;
            }
            else
            {
                NoItem.Visibility = Visibility.Collapsed;
            }
        }
        private static TabViewItem CreateNewTVI(string header, string dataContext, System.Uri uri, NavigationView navigationView)
        {
            var newTab = new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Placeholder
                },
                Header = header,
                Content = new BlankPage2(uri, navigationView)
                {
                    DataContext = dataContext
                }
            };
            return newTab;
        }
        private static TabViewItem CreateNewTVI2(string header, string dataContext)
        {
            var newTab = new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Placeholder
                },
                Header = header,
            };
            return newTab;
        }
        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                System.Uri uri;
                if (AddressBar.Text.Length > 3 && AddressBar.Text.Remove(0, 1).Contains("://"))
                {
                    uri = new System.Uri(AddressBar.Text);
                }
                else if (AddressBar.Text.Length > 3 && AddressBar.Text.Remove(0, 1).Contains('.') && AddressBar.Text.Remove(AddressBar.Text.Length - 2, 2).Contains('.') && AddressBar.Text.Contains(' ') == false)
                {
                    try
                    {
                        uri = new System.Uri("https://" + AddressBar.Text);
                    }
                    catch
                    {
                        uri = new System.Uri(Globals.SearchEngine + "/search?q=" + HttpUtility.UrlEncode(AddressBar.Text));
                    }
                }
                else
                {
                    uri = new System.Uri(Globals.SearchEngine + "/search?q=" + HttpUtility.UrlEncode(AddressBar.Text));
                }
                var frame = this.Parent as Frame;
                var navviewparent = frame.Parent as NavigationView;
                TabViewItem tab = CreateNewTVI("New Tab", "New Tab", uri, navviewparent);
                BrowserTabs.TabItems.Add(tab);
                BrowserTabs.TabItems.Remove(BrowserTabs.SelectedItem);
                BrowserTabs.SelectedItem = tab;
            }
        }
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            TabViewItem tab;
            if(Globals.NewTabBehavior == 0)
            {
                tab = CreateNewTVI2("Launchpad", "Launchpad");
            }
            else
            {
                tab = CreateNewTVI("New Tab", "New Tab", null, navviewparent);
            }
            sender.TabItems.Add(tab);
            sender.SelectedItem = tab;
        }
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            try
            {
                var tab = args.Tab;
                var tabitself = tab.Content as BlankPage2;
                tabitself.ClearBrowser();
                if (sender.SelectedItem as BlankPage2 == tabitself)
                {
                    if (sender.SelectedIndex == 1)
                    {
                        try
                        {
                            sender.SelectedIndex++;
                        }
                        catch
                        {
                            sender.SelectedIndex = -1;
                        }
                    }
                    if (sender.SelectedIndex >= 2)
                    {
                        sender.SelectedIndex --;
                    }

                }
            }
            catch
            {
                
            }
            sender.TabItems.Remove(args.Tab);
        }
        public void CloseEverything()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var item in BrowserTabs.TabItems)
                {
                    BrowserTabs.TabItems.Remove(item);
                }
            });
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.IsEnabled = false;
        }

        private void Launchpad_LayoutUpdated(object sender, object e)
        {
            if (!FTU)
            {
                string Apps = "";
                foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
                {
                    if (item is Grid)
                    {
                        Grid grid = item as Grid;
                        var childControl3 = grid.Children[0] as Button;
                        Apps = Apps + childControl3.Tag.ToString() + "\n";
                    }
                }
                File.WriteAllText("Config/LaunchpadBrowserConfig.crlh", Apps);
            }
        }
        bool FTU = true;
        string WidgetOrder;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                try
                {
                    WidgetOrder = File.ReadAllText("Config/LaunchpadBrowserConfig.crlh");
                }
                catch
                {
                    File.AppendAllText("Config/LaunchpadBrowserConfig.crlh", "");
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        foreach (string line in WidgetOrder.Split('\n'))
                        {
                            if (line.Length != 0)
                            {
                                string fix = line.Replace("\r", "");
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.UriSource = new System.Uri("https://www.google.com/s2/favicons?domain=" + new System.Uri(fix).Host + "&sz=256");
                                Microsoft.UI.Xaml.Controls.Image image = new Microsoft.UI.Xaml.Controls.Image();
                                image.Source = bitmapImage;
                                Grid grid = new();
                                Grid grid1 = new()
                                {
                                    CornerRadius = new CornerRadius(10),
                                    Width = 150,
                                    Height = 150
                                };
                                grid1.Children.Add(image);
                                Button button = new()
                                {
                                    Tag = new System.Uri(line),
                                    Content = grid1,
                                    Width = 200,
                                    Height = 200,
                                    CornerRadius = new CornerRadius(10),
                                };
                                button.Click += newTabLaunchpad;
                                grid.RightTapped += IndividualItemRC;
                                grid.Children.Add(button);
                                Launchpad.Items.Add(grid);
                            }
                        }
                    }
                    catch
                    {

                    }
                    if (Launchpad.Items.Count == 0)
                    {
                        NoItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        NoItem.Visibility = Visibility.Collapsed;
                    }
                });
                FTU = false;
                if (Globals.IsAdmin || Globals.LockLayout)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Launchpad.CanReorderItems = false;
                        Launchpad.CanDragItems = false;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Launchpad.CanReorderItems = true;
                        Launchpad.CanDragItems = true;
                    });
                }
            }
        }

        private void Launchpad_LayoutUpdated_1(object sender, object e)
        {
            AddressBar.Width = Launchpad.ActualWidth;
        }
    }
}
