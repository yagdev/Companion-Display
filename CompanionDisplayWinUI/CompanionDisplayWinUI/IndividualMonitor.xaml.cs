using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static CompanionDisplayWinUI.WidgetMonitorBrightness;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualMonitor : Page
    {
        public IndividualMonitor()
        {
            this.InitializeComponent();
        }
        private List<PHYSICAL_MONITOR> physicalMonitors = new List<PHYSICAL_MONITOR>();
        private bool FTU = true;
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                FTU = false;
                Frame parentFrame = GetParentFrame();
                if (parentFrame != null)
                {
                    if (parentFrame.Name.Length > 14)
                    {
                        MonitorName.Text = parentFrame.Name.Remove(10, parentFrame.Name.Length - 10) + "...";
                    }
                    else
                    {
                        MonitorName.Text = parentFrame.Name;
                    }
                    GetMonitorBrightness(nint.Parse(parentFrame.Tag as string), out uint minBrightness, out uint currentBrightness, out uint maxBrightness);
                    MonitorBrightness.Value = currentBrightness;
                    MonitorBrightness.Maximum = maxBrightness;
                    MonitorBrightness.Minimum = minBrightness;
                    if (maxBrightness <= 0)
                    {
                        var GridElementMain = parentFrame.Parent as GridView;
                        GridElementMain.Items.Remove(parentFrame);
                    }
                }
                else
                {
                    // Handle the case where the parent is not a Frame
                }
            }
        }
        public Frame GetParentFrame()
        {
            var parent = this.Parent as Frame;
            return parent;
        }
        
        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int brightness = (int)e.NewValue;
            SetMonitorBrightness(nint.Parse((GetParentFrame().Tag as string)), (uint)brightness);
        }

        private void LogError(string message)
        {
            // Implement your logging here. For example:
            Console.WriteLine($"{message}");
            // Optionally log to a file or monitoring system
        }
        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);
        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
