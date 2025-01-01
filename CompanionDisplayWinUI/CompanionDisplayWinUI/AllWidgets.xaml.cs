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
                    frame.CornerRadius = new CornerRadius(10);
                    frame.Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
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
            Globals.ResetHome = true;
            try
            {
                Globals.IsAllApps = false;
                var frame = e.ClickedItem as Frame;
                if (frame.Content.ToString().Contains("NotesWidget"))
                {
                    File.AppendAllText("Config/WidgetOrder.crlh", frame.Content + "ID" + DateTime.Now.ToString("yyyymmddhhmmssff") + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText("Config/WidgetOrder.crlh", frame.Content + Environment.NewLine);
                }
                var frame2 = this.Parent as Frame;
                var navviewparent = frame2.Parent as NavigationView;
                navviewparent.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)navviewparent.FindName("HomeItem");
                frame2.Navigate(typeof(BlankPage1));
            }
            catch 
            {
                Globals.IsAllApps = true;
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
    }
}
