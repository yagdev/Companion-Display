using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Swan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
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
                if (sourceGridView != (sender as GridView).Tag as string)
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
                if (sourceGridView == targetGridView.Tag as string)
                {
                    // Assuming the items are strings for this example
                    var items = e.DataView.GetDataAsync("Text").GetResults() as IEnumerable<string>;

                    // Add items to the target GridView
                    foreach (var item in items)
                    {
                        (targetGridView.ItemsSource as IList<string>).Add(item);
                    }
                    Thread thread = new(SaveTo);
                    thread.Start();
                }
            });
        }
        private void SaveTo()
        {
            if(Globals.IsAllApps == false)
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
                            if (!deedify.Contains("CompanionDisplayWinUI.UpdateWarning") && !deedify.Contains("Microsoft.UI.Xaml.Controls.Button"))
                            {
                                switch (deedify)
                                {
                                    case var s when (deedify.Contains("CompanionDisplayWinUI.NotesWidget") || deedify.Contains("CompanionDisplayWinUI.WidgetStack")):
                                        Order = Order + deedify + "ID" + item.Name.ToString() + Environment.NewLine;
                                        break;
                                    case var s when deedify.Contains("CompanionDisplayWinUI.WidgetPhoto"):
                                        Order = Order + deedify + "IMAGESOURCE" + item.Tag.ToString() + Environment.NewLine;
                                        break;
                                    default:
                                        Order = Order + deedify + Environment.NewLine;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    File.WriteAllText("Config/WidgetOrder.crlh", Order);
                    var Items2 = PinnedView.Items;
                    string Order2 = "";
                    foreach (var widget in Items2)
                    {
                        try
                        {
                            var item = widget as Frame;
                            string deedify = item.Content.ToString().Replace("WidgetSettings", "");
                            if (!deedify.Contains("CompanionDisplayWinUI.UpdateWarning") && !deedify.Contains("Microsoft.UI.Xaml.Controls.Button"))
                            {
                                switch (deedify)
                                {
                                    case var s when (deedify.Contains("CompanionDisplayWinUI.NotesWidget") || deedify.Contains("CompanionDisplayWinUI.WidgetStack")):
                                        Order2 = Order2 + deedify + "ID" + item.Name.ToString() + Environment.NewLine;
                                        break;
                                    case var s when deedify.Contains("CompanionDisplayWinUI.WidgetPhoto"):
                                        Order2 = Order2 + deedify + "IMAGESOURCE" + item.Tag.ToString() + Environment.NewLine;
                                        break;
                                    default:
                                        Order2 = Order2 + deedify + Environment.NewLine;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    File.WriteAllText("Config/PinnedOrder.crlh", Order2);
                });
            }
        }
        Button addBtn;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.IsAllApps = true;
                var frame = this.Parent as Frame;
                var navviewparent = frame.Parent as NavigationView;
                frame.Navigate(typeof(AllWidgets));
                navviewparent.SelectedItem = -1;
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
            if((sender as Frame).Parent as GridView != null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = "Remove", Name = senderElement.Name + "Flyout" };
                Frame frame = senderElement as Frame;
                Type type1 = Type.GetType(frame.Content.ToString() + "WidgetSettings");
                if (type1 != null)
                {
                    MenuFlyoutItem secondItem = new() { Text = "Edit", Name = senderElement.Name + "Edit" };
                    secondItem.Click += MenuFlyoutEdit_Click;
                    myFlyout.Items.Add(secondItem);
                }
                MenuFlyoutItem thirdItem = new() { Text = "Pin", Name = senderElement.Name + "Flyout" };
                if (senderElement.Parent == PinnedView)
                {
                    thirdItem.Text = "Unpin";
                }
                MenuFlyoutItem fourthItem = new() { Text = "Picture in Picture", Name = senderElement.Name + "PiP" };
                Frame frame3 = senderElement as Frame;
                firstItem.Click += MenuFlyoutItem_Click;
                thirdItem.Tag = sender as Frame;
                thirdItem.Click += Pin_Click;
                fourthItem.Tag = sender as Frame;
                fourthItem.Click += OpenPiP;
                myFlyout.Items.Add(firstItem);
                myFlyout.Items.Add(thirdItem);
                myFlyout.Items.Add(fourthItem);
                myFlyout.ShowAt(senderElement, new Point(0, 0));
            }
        }

        private void OpenPiP(object sender, RoutedEventArgs e)
        {
            Frame darkframe = (sender as MenuFlyoutItem).Tag as Frame;
            gridViewPiPTemp = darkframe.Parent as Microsoft.UI.Xaml.Controls.GridView;
            if (gridViewPiPTemp != null)
            {
                // Get the original index of the frame in the parent
                indexTemp = gridViewPiPTemp.Items.IndexOf(darkframe);
                darkframe.Unloaded += LoadWindowStage2;
                // Replace the frame with a placeholder
                gridViewPiPTemp.Items.RemoveAt(indexTemp);
            }
        }
        public int indexTemp = 0;
        GridView gridViewPiPTemp;
        private void LoadWindowStage2(object sender, RoutedEventArgs e)
        {
            (sender as Frame).Unloaded -= LoadWindowStage2;
            var placeholder = new Grid();
            placeholder.Height = 300;
            placeholder.Width = 500;
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "This item is open in PiP mode.";
            textBlock.FontSize = 20;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            placeholder.Children.Add(textBlock);
            gridViewPiPTemp.Items.Insert(indexTemp, placeholder);
            PopupWidget m_window = new PopupWidget(sender as Frame, gridViewPiPTemp, placeholder);
            m_window.Closed += (s, e) =>
            {
                Globals.PiPAmount--;
                if(Globals.PiPAmount == 0)
                {
                    addBtn.IsEnabled = true;
                }
            };
            m_window.Activate();
            Globals.PiPAmount++;
            addBtn.IsEnabled = false;
        }

        public void Pin_Click_NC(object sender, RoutedEventArgs e)
        {
            if ((sender as Frame).Parent == BasicGridView)
            {
                BasicGridView.Items.Remove(sender);
                PinnedView.Items.Add(sender);
            }
            else
            {
                PinnedView.Items.Remove(sender);
                BasicGridView.Items.Add(sender);
            }
            SaveTo();
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(calculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + calculatedPinHeight(), 0, 0);
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength();
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
            }
        }
        
        public int calculatedPinHeight()
        {
            int calculatedInitial = (int)(Math.Ceiling((500 * PinnedView.Items.Count + PinnedView.Items.Count * 2 + 6) / (CompleteGrid.ActualWidth)) * 302 + 60);
            int convertedHeight = (int)CompleteGrid.ActualHeight;
            if (calculatedInitial <= convertedHeight - 360)
            {
                if (convertedHeight - 360 < 360)
                {
                    return 360;
                }
                else
                {
                    return calculatedInitial;
                }
            }
            else
            {
                return 360;
            }
        }
        public void Pin_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuFlyoutItem).Text == "Pin")
            {
                BasicGridView.Items.Remove((sender as MenuFlyoutItem).Tag as Frame);
                PinnedView.Items.Add((sender as MenuFlyoutItem).Tag as Frame);
            }
            else
            {
                PinnedView.Items.Remove((sender as MenuFlyoutItem).Tag as Frame);
                BasicGridView.Items.Add((sender as MenuFlyoutItem).Tag as Frame);
            }
            SaveTo();
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(calculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + calculatedPinHeight(), 0, 0);
                PinScrollView.Height = PinnedRow.Height.Value - 60;
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength(0);
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
                PinScrollView.Height = 0;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(senderElement.Name.Remove (senderElement.Name.Length - 6, 6));
            BasicGridView.Items.Remove(childControl);
            PinnedView.Items.Remove(childControl);
            try
            {
                File.Delete("Config/WidgetNotes/" + childControl.Tag.ToString() + ".crlh");
            }
            catch { }
            Thread thread = new(SaveTo);
            thread.Start();
        }
        private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            var senderElement = sender as MenuFlyoutItem;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(senderElement.Name.Remove(senderElement.Name.Length - 4, 4));
            var frame = childControl as Frame;
            Type type1 = Type.GetType(frame.Content + "WidgetSettings");
            frame.Navigate(type1);
        }

        private void BasicGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Thread thread = new(SaveTo);
            thread.Start();
        }

        private async void UpdateUI()
        {
            int i = 1;
            string WidgetOrder = "";
            try
            {
                WidgetOrder = File.ReadAllText("Config/WidgetOrder.crlh");
            }
            catch
            {
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
                        switch (fix)
                        {
                            case string a when a.Contains("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE"):
                                string fix2 = "CompanionDisplayWinUI.WidgetPhoto";
                                string folderpath = fix.Replace("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE", "");
                                Type type1 = Type.GetType(fix2);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        Name = "Widget" + i,
                                        CornerRadius = new CornerRadius(8),
                                        Tag = folderpath,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged2;
                                    frame.Loaded += BugcheckAcrylic;
                                    BasicGridView.Items.Add(frame);
                                    frame.Navigate(type1);
                                    i++;
                                });
                                break;
                            case string b when b.Contains("CompanionDisplayWinUI.NotesWidgetID"):
                                string fix3 = "CompanionDisplayWinUI.NotesWidget";
                                string ID = fix.Replace("CompanionDisplayWinUI.NotesWidgetID", "");
                                Type type2 = Type.GetType(fix3);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        Name = "Widget" + i,
                                        CornerRadius = new CornerRadius(8),
                                        Tag = ID,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged2;
                                    frame.Loaded += BugcheckAcrylic;
                                    frame.RightTapped += Frame_RightTapped;
                                    BasicGridView.Items.Add(frame);
                                    frame.Navigate(type2);
                                    i++;
                                });
                                break;
                            case string c when c.Contains("CompanionDisplayWinUI.WidgetStackID"):
                                string fix4 = "CompanionDisplayWinUI.WidgetStack";
                                string ID2 = fix.Replace("CompanionDisplayWinUI.WidgetStackID", "");
                                Type type3 = Type.GetType(fix4);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        CornerRadius = new CornerRadius(8),
                                        Name = ID2,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                    frame.Loaded += BugcheckAcrylic;
                                    BasicGridView.Items.Add(frame);
                                    frame.Navigate(type3);
                                    i++;
                                });
                                break;
                            default:
                                Type type0 = Type.GetType(fix);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    try
                                    {
                                        Frame frame = new()
                                        {
                                            Name = "Widget" + i,
                                            CornerRadius = new CornerRadius(8),
                                            Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                        };
                                        frame.Loaded += BugcheckAcrylic;
                                        if (fix.Contains("NORC_") != true)
                                        {
                                            frame.RightTapped += Frame_RightTapped;
                                        }
                                        else
                                        {
                                            frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                        }
                                        BasicGridView.Items.Add(frame);
                                        frame.Navigate(type0);
                                        i++;
                                    }
                                    catch
                                    {

                                    }
                                });
                                break;
                        }
                    }
                }
                catch
                {
                }
            }
            try
            {
                WidgetOrder = File.ReadAllText("Config/PinnedOrder.crlh");
            }
            catch
            {
                WidgetOrder = "";
                Directory.CreateDirectory("Config");
                File.WriteAllText("Config/PinnedOrder.crlh", "");
            }
            foreach (string line in WidgetOrder.Split('\n'))
            {
                try
                {
                    if (line.Length != 0)
                    {
                        string fix = line.Replace("\r", "");
                        switch (fix)
                        {
                            case string a when a.Contains("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE"):
                                string fix2 = "CompanionDisplayWinUI.WidgetPhoto";
                                string folderpath = fix.Replace("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE", "");
                                Type type1 = Type.GetType(fix2);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        Name = "Widget" + i,
                                        CornerRadius = new CornerRadius(8),
                                        Tag = folderpath,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged2;
                                    frame.Loaded += BugcheckAcrylic;
                                    PinnedView.Items.Add(frame);
                                    frame.Navigate(type1);
                                    i++;
                                });
                                break;
                            case string b when b.Contains("CompanionDisplayWinUI.NotesWidgetID"):
                                string fix3 = "CompanionDisplayWinUI.NotesWidget";
                                string ID = fix.Replace("CompanionDisplayWinUI.NotesWidgetID", "");
                                Type type2 = Type.GetType(fix3);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        Name = "Widget" + i,
                                        CornerRadius = new CornerRadius(8),
                                        Tag = ID,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged2;
                                    frame.Loaded += BugcheckAcrylic;
                                    frame.RightTapped += Frame_RightTapped;
                                    PinnedView.Items.Add(frame);
                                    frame.Navigate(type2);
                                    i++;
                                });
                                break;
                            case string c when c.Contains("CompanionDisplayWinUI.WidgetStackID"):
                                string fix4 = "CompanionDisplayWinUI.WidgetStack";
                                string ID2 = fix.Replace("CompanionDisplayWinUI.WidgetStackID", "");
                                Type type3 = Type.GetType(fix4);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    Frame frame = new()
                                    {
                                        CornerRadius = new CornerRadius(8),
                                        Name = ID2,
                                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                    };
                                    frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                    frame.Loaded += BugcheckAcrylic;
                                    PinnedView.Items.Add(frame);
                                    frame.Navigate(type3);
                                    i++;
                                });
                                break;
                            default:
                                Type type0 = Type.GetType(fix);
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    try
                                    {
                                        Frame frame = new()
                                        {
                                            Name = "Widget" + i,
                                            CornerRadius = new CornerRadius(8),
                                            Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                                        };
                                        frame.Loaded += BugcheckAcrylic;
                                        if (fix.Contains("NORC_") != true)
                                        {
                                            frame.RightTapped += Frame_RightTapped;
                                        }
                                        else
                                        {
                                            frame.IsEnabledChanged += Frame_IsEnabledChanged;
                                        }
                                        PinnedView.Items.Add(frame);
                                        frame.Navigate(type0);
                                        i++;
                                    }
                                    catch
                                    {

                                    }
                                });
                                break;
                        }
                    }
                }
                catch
                {
                }
            }
            if (!Globals.HideAddButton)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Frame grid = new()
                    {
                        Height = 300,
                        Width = 500,
                        CornerRadius = new CornerRadius(8),
                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                    };
                    grid.Loaded += BugcheckAcrylic;
                    Button button = new()
                    {
                        Name = "AddWidget",
                        Content = "+",
                        FontFamily = new FontFamily("Segoe UI Variable Display Light"),
                        Height = 300,
                        Width = 500,
                        FontSize = 100,
                        Margin = new Thickness(0),
                        CornerRadius = new CornerRadius(8),
                        BorderThickness = new Thickness(0),
                        Background = new SolidColorBrush(Color.FromArgb(0,0,0,0)),
                        AllowDrop = false,
                    };
                    addBtn = button;
                    grid.Content = button;
                    button.Click += Button_Click;
                    button.DropCompleted += Button_DropCompleted;
                    BasicGridView.Items.Add(grid);
                });
            }
           
            try
            {
                using (HttpClient client = new())
                {
                    string reply;
                    if (Globals.IsBetaProgram)
                    {
                        reply = await client.GetStringAsync(Globals.UpdateStringBeta);
                    }
                    else
                    {
                        reply = await client.GetStringAsync(Globals.UpdateString);
                    }
                    if (reply == Globals.Version)
                    {
                        Globals.IsUpdateAvailable = false;
                    }
                    else
                    {
                        Globals.IsUpdateAvailable = true;
                    }
                    if (Globals.IsUpdateAvailable == true && !Globals.isConfidential)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Frame frame = new()
                            {
                                Name = "UpdateWidget1",
                                CornerRadius = new CornerRadius(8),
                            };
                            frame.RightTapped += Frame_RightTapped;
                            BasicGridView.Items.Insert(0,frame);
                            frame.Navigate(typeof(UpdateWarning));
                        });
                    }
                }
            }
            catch
            {

            }
            
        }

        private void BugcheckAcrylic(object sender, RoutedEventArgs e)
        {
            int Backdrop = Globals.Backdrop;
            if(Globals.Backdrop == 0 || Globals.Backdrop == 1)
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                (sender as Frame).Background = null;
                (sender as Frame).Background = new SolidColorBrush(uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background));
                (sender as Frame).Background.Opacity = 0.3;
            }
            else
            {
                (sender as Frame).Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                (sender as Frame).Background.Opacity = 1;
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
                Thread thread = new(SaveTo);
                thread.Start();
            }
        }
        int bugcheckpin = 0;
        private void Frame_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (bugcheckpin == 0)
            {
                bugcheckpin = 1;
            }
            else
            {
                switch ((sender as Frame).Tag)
                {
                    case "pin":
                        Pin_Click_NC(sender, null);
                        break;
                    case "pip":
                        MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
                        menuFlyoutItem.Tag = sender;
                        OpenPiP(menuFlyoutItem, null);
                        break;
                    default:
                        BasicGridView.Items.Remove(sender);
                        PinnedView.Items.Remove(sender);
                        Thread thread = new(SaveTo);
                        thread.Start();
                        break;
                }
                bugcheckpin = 0;
            }
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.IsAdmin || Globals.LockLayout)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BasicGridView.CanReorderItems = false;
                    BasicGridView.CanDragItems = false;
                    PinnedView.CanReorderItems = false;
                    PinnedView.CanDragItems = false;
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BasicGridView.CanReorderItems = true;
                    BasicGridView.CanDragItems = true;
                    PinnedView.CanReorderItems = true;
                    PinnedView.CanDragItems = true;
                });
            }
            if (Globals.ResetHome)
            {
                BasicGridView.Items.Clear();
                PinnedView.Items.Clear();
                Thread thread0 = new(UpdateUI);
                thread0.Start();
                Globals.ResetHome = false;
            }
        }

        private void CompleteGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(calculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + calculatedPinHeight(), 0, 0);
                PinScrollView.Height = PinnedRow.Height.Value + 20;
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength(0);
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
                PinScrollView.Height = 0;
            }
        }

        private void ImageOptionalBlur_Loaded(object sender, RoutedEventArgs e)
        {
            int Backdrop = Globals.Backdrop;
            if (Globals.Backdrop == 0 || Globals.Backdrop == 1)
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                (sender as Rectangle).Fill = null;
                Color uiDefault = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
                if(((App)Application.Current).GetTheme() == ElementTheme.Dark)
                {
                    (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(255, 33, 33, 33));
                }
                else
                {
                    (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(255, 212, 212, 212));
                }
                (sender as Rectangle).Fill.Opacity = 1;
            }
            else
            {
                (sender as Rectangle).Fill = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                (sender as Rectangle).Fill.Opacity = 1;
            }
        }
    }
}
