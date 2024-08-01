using HidSharp.Utility;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using Swan.Formatters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MusixmatchIntegrationProto : Page
    {
        public MusixmatchIntegrationProto()
        {
            this.InitializeComponent();
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
            var tab = CreateNewTVI("New Item", "New Item");
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
                if(sender.SelectedItem as BlankPage2 == tabitself)
                {
                    if(sender.SelectedIndex == 1)
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
                    if(sender.SelectedIndex >= 2)
                    {
                        sender.SelectedIndex--;
                    }
                   
                }
                sender.TabItems.Remove(args.Tab);
            }
            catch { }
            
        }
    }
}
