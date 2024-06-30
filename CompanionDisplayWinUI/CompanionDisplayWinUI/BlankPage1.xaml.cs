using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NAudio.Gui;
using Swan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        bool CleanUp = false;
        public BlankPage1()
        {
            this.InitializeComponent();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private void GridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                // Set the drag data
                var page = sender as GridView;
                e.Data.SetData("sourceGridView", page.Tag);
            });
        }

        private void GridView_DragOver(object sender, DragEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                var sourceGridView = e.DataView.Properties["sourceGridView"] as string;
                if (sourceGridView != (sender as GridView).Tag)
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                }
            });
        }

        private void GridView_Drop(object sender, DragEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                var sourceGridView = e.DataView.Properties["sourceGridView"] as string;
                var targetGridView = sender as GridView;
                if (sourceGridView == targetGridView.Tag)
                {
                    // Assuming the items are strings for this example
                    var items = e.DataView.GetDataAsync("Text").GetResults() as IEnumerable<string>;

                    // Add items to the target GridView
                    foreach (var item in items)
                    {
                        (targetGridView.ItemsSource as IList<string>).Add(item);
                    }
                    Thread thread = new Thread(SaveTo);
                    thread.Start();
                }
            });
        }
        private void SaveTo()
        {
            if(CleanUp == false && Globals.IsAllApps == false)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    var Items = BasicGridView.Items;
                    string Order = "";
                    foreach (var widget in Items)
                    {
                        try
                        {
                            var item = widget as Frame;
                            string deedify = item.Content.ToString().Replace("WidgetSettings", "");
                            if (!deedify.Contains("CompanionDisplayWinUI.UpdateWarning"))
                            {
                                if (deedify.Contains("CompanionDisplayWinUI.WidgetPhoto"))
                                {
                                    Order = Order + deedify + "IMAGESOURCE" + item.Tag.ToString() + Environment.NewLine;
                                }
                                else
                                {
                                    Order = Order + deedify + Environment.NewLine;
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            //File.AppendAllText("ErrorLog.crlh", ex.Message);
                        }
                        File.WriteAllText("Config/WidgetOrder.crlh", Order);
                    }
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.IsAllApps = true;
                var frame = this.Parent as Frame;
                frame.Navigate(typeof(AllWidgets));
            }
            catch
            {
                Globals.IsAllApps = false;
            }
            
        }

        private void Button_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
        }

        private void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Remove", Name = senderElement.Name + "Flyout" };
            Frame frame = senderElement as Frame;
            Type type1 = Type.GetType(frame.Content.ToString() + "WidgetSettings");
            if (type1 != null)
            {
                MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "Edit", Name = senderElement.Name + "Edit" };
                secondItem.Click += MenuFlyoutEdit_Click;
                myFlyout.Items.Add(secondItem);
            }
            firstItem.Click += MenuFlyoutItem_Click;
            myFlyout.Items.Add(firstItem);
            myFlyout.ShowAt(senderElement, new Point(0, 0));
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(senderElement.Name.Remove (senderElement.Name.Length - 6, 6));
            Debug.Text = senderElement.Name.Remove(senderElement.Name.Length - 6, 6);
            BasicGridView.Items.Remove(childControl);
            Thread thread = new Thread(SaveTo);
            thread.Start();
        }
        private async void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            var senderElement = sender as MenuFlyoutItem;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(senderElement.Name.Remove(senderElement.Name.Length - 4, 4));
            var frame = childControl as Frame;
            Type type1 = Type.GetType(frame.Content + "WidgetSettings");
            frame.Navigate(type1);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
            BasicGridView.Items.Clear();
        }

        private void BasicGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Thread thread = new Thread(SaveTo);
            thread.Start();
        }

        private void UpdateUI()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if(isElevated == true)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BasicGridView.CanReorderItems = false;
                    BasicGridView.CanDragItems = false;
                });
            }
            int i = 1;
            string WidgetOrder = "";
            try
            {
                WidgetOrder = File.ReadAllText("Config/WidgetOrder.crlh");
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
                Directory.CreateDirectory("Config");
                File.WriteAllText("Config/WidgetOrder.crlh", "");
            }
            foreach (string line in WidgetOrder.Split('\n'))
            {
                try
                {
                    if (line.Length != 0)
                    {
                        string fix = line.Replace("\r", "");
                        if (fix.Contains("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE"))
                        {
                            string fix2 = "CompanionDisplayWinUI.WidgetPhoto";
                            string folderpath = fix.Replace("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE", "");
                            Type type1 = Type.GetType(fix2);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                Frame frame = new()
                                {
                                    Name = "Widget" + i,
                                    Tag = folderpath,
                                };
                                frame.IsEnabledChanged += Frame_IsEnabledChanged2;
                                if (fix.Contains("NORC_") != true)
                                {
                                    frame.RightTapped += Frame_RightTapped;
                                }
                                else
                                {
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                }
                                BasicGridView.Items.Add(frame);
                                frame.Navigate(type1);
                                i++;
                            });
                        }
                        else
                        {
                            Type type1 = Type.GetType(fix);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                Frame frame = new()
                                {
                                    Name = "Widget" + i,
                                };
                                if (fix.Contains("NORC_") != true)
                                {
                                    frame.RightTapped += Frame_RightTapped;
                                }
                                else
                                {
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                }
                                BasicGridView.Items.Add(frame);
                                frame.Navigate(type1);
                                i++;
                            });
                        }
                        
                        
                    }
                }
                catch (Exception ex)
                {
                    //File.AppendAllText("ErrorLog.crlh", ex.Message);
                }
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                Button button = new()
                {
                    Name = "AddWidget",
                    Height = 300,
                    Width = 500,
                    Content = "+",
                    FontFamily = new FontFamily("Segoe UI Variable Display Light"),
                    FontSize = 100,
                    AllowDrop = true,
                };
                button.Click += Button_Click;
                button.DropCompleted += Button_DropCompleted;
                BasicGridView.Items.Add(button);
            });
            WebClient client = new WebClient();
            string reply = client.DownloadString(Globals.UpdateString);
            if (reply == Globals.Version)
            {
                Globals.IsUpdateAvailable = false;
            }
            else
            {
                Globals.IsUpdateAvailable = true;
            }
            if (Globals.IsUpdateAvailable == true)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Frame frame = new()
                    {
                        Name = "UpdateWidget1",
                    };
                    frame.RightTapped += Frame_RightTapped;
                    BasicGridView.Items.Add(frame);
                    frame.Navigate(typeof(UpdateWarning));
                });
            }
        }

        private void BasicGridView_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }
        private void Frame_IsEnabledChanged2(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frame2 = (Frame)sender;
            if(frame2 != null && frame2.IsEnabled == true)
            {
                Thread thread = new Thread(SaveTo);
                thread.Start();
            }
        }
        private void Frame_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicGridView.Items.Remove(sender);
            Thread thread = new Thread(SaveTo);
            thread.Start();
        }
    }
}
