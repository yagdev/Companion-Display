using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeWidget : Page
    {
        readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        public TimeWidget()
        {
            this.InitializeComponent();
            try
            {
                string config = File.ReadAllText(Globals.TimeConfigFile);
                if (config != "")
                {
                    using StringReader readerconfig = new(config);
                    int RTime = int.Parse(readerconfig.ReadLine());
                    int GTime = int.Parse(readerconfig.ReadLine());
                    int BTime = int.Parse(readerconfig.ReadLine());
                    int RDate = int.Parse(readerconfig.ReadLine());
                    int GDate = int.Parse(readerconfig.ReadLine());
                    int BDate = int.Parse(readerconfig.ReadLine());
                    Time.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RTime, (byte)GTime, (byte)BTime));
                    Date.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RDate, (byte)GDate, (byte)BDate));
                }
            }
            catch
            {

            }
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        public bool CleanUp = false;
        private void UpdateUI()
        {
            try
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    Date.Text = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                    Time.Text = TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm");
                });
            }
            catch
            {

            }
            if (CleanUp == false)
            {
                Thread.Sleep(1000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
    }
}
