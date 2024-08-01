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
        private static TabViewItem CreateNewTVI(string header, string dataContext)
        {
            var newTab = new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Placeholder
                },
                Header = header,
                Content = new BlankPage2()
                {
                    DataContext = dataContext
                }
            };
            return newTab;
        }
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var tab = CreateNewTVI("New Tab", "New Tab");
            sender.TabItems.Add(tab);
            sender.SelectedItem = tab;
        }
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            var tab = args.Tab;
            var tabitself = tab.Content as BlankPage2;
            tabitself.ClearBrowser();
            try
            {
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
                sender.TabItems.Remove(args.Tab);
            }
            catch { }
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
        public CoreWebView2 CreateTab()
        {
            var tab = CreateNewTVI("New Tab", "New Tab");
            BrowserTabs.TabItems.Add(tab);
            BrowserTabs.SelectedItem = tab;
            var nt = tab.Content as BlankPage2;
            CoreWebView2 coreWebView2 = nt.ReturnNewCoreWebView2Tab();
            return coreWebView2;
        }
    }
}
