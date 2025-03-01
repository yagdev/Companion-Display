using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllWidgets : Page
    {
        private string targetFile = "Config/WidgetOrder.crlh";
        public AllWidgets()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            var currentAssembly = this.GetType().GetTypeInfo().Assembly;
            var pageTypeInfo = typeof(Page).GetTypeInfo();
            int i = 1;
            foreach (var item in currentAssembly.DefinedTypes.Where(t => pageTypeInfo.IsAssignableFrom(t)).ToList())
            {
                if (item.Name.Contains("Widget") && item.Name != "AllWidgets" && !item.Name.Contains("WidgetSettings") && !item.Name.Contains("Individual"))
                {
                    Frame frame = new Frame();
                    frame.Name = "Widget" + i;
                    frame.CornerRadius = new CornerRadius(8);
                    frame.Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                    frame.Loaded += BugcheckAcrylic;
                    frame.IsHitTestVisible = false;
                    frame.Navigate(item);
                    try
                    {
                        String grouper = (frame.Content as Page).Tag.ToString();
                        (AllCategories.FindName(grouper + "GridView") as GridView).Items.Add(frame);
                    }
                    catch { }
                }
            }
        }
        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(!((e.ClickedItem as Frame).Content.ToString().Contains("WidgetStack") && targetFile != "Config/WidgetOrder.crlh"))
            {
                Globals.ResetHome = true;
                try
                {
                    Globals.IsAllApps = false;
                    var frame = e.ClickedItem as Frame;
                    if (frame.Content.ToString().Contains("NotesWidget") || frame.Content.ToString().Contains("WidgetStack"))
                    {
                        File.AppendAllText(targetFile, frame.Content + "ID" + DateTime.Now.ToString("yyyymmddhhmmssff") + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(targetFile, frame.Content + Environment.NewLine);
                    }
                    var frame2 = this.Parent as Frame;
                    var navviewparent = frame2.Parent as NavigationView;
                    frame2.GoBack();
                    navviewparent.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)navviewparent.FindName("HomeItem");
                }
                catch
                {
                    Globals.IsAllApps = true;
                }
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            foreach(GridView gridView in AllCategories.Children)
            {
                gridView.Visibility = Visibility.Collapsed;
            }
            (AllCategories.FindName((sender.SelectedItem as NavigationViewItem).Name + "GridView") as GridView).Visibility = Visibility.Visible;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (((this.Parent as Frame).Parent as NavigationView).Tag != null && ((this.Parent as Frame).Parent as NavigationView).Tag != "")
            {
                targetFile = "Config/Stacks/" + ((this.Parent as Frame).Parent as NavigationView).Tag.ToString() + ".crlh";
                ((this.Parent as Frame).Parent as NavigationView).Tag = "";
            }
            else
            {
                targetFile = "Config/WidgetOrder.crlh";
            }
        }
        private void BugcheckAcrylic(object sender, RoutedEventArgs e)
        {
            int Backdrop = Globals.Backdrop;
            if (Globals.Backdrop == 0 || Globals.Backdrop == 1)
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

    }
}
