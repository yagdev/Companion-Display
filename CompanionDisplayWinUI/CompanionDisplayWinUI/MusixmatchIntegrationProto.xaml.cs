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
            LoadMonitorNames();
        }
        private List<PHYSICAL_MONITOR> physicalMonitors = new List<PHYSICAL_MONITOR>();

        private void LoadMonitorNames()
        {
            physicalMonitors = new List<PHYSICAL_MONITOR>();

            // Enumerate all monitors
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);

            foreach (var monitor in physicalMonitors)
            {
                MonitorComboBox.Items.Add(monitor.szPhysicalMonitorDescription);
            }

            if (MonitorComboBox.Items.Count > 0)
            {
                MonitorComboBox.SelectedIndex = 0;
            }
        }

        private bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            uint monitorCount = 0;
            GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref monitorCount);

            if (monitorCount > 0)
            {
                PHYSICAL_MONITOR[] physicalMonitorArray = new PHYSICAL_MONITOR[monitorCount];
                if (GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, physicalMonitorArray))
                {
                    physicalMonitors.AddRange(physicalMonitorArray);
                }
            }

            return true; // Continue enumeration
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int brightness = (int)e.NewValue;
            BrightnessValueText.Text = $"Brightness: {brightness}";

            if (MonitorComboBox.SelectedIndex >= 0)
            {
                var selectedMonitor = physicalMonitors[MonitorComboBox.SelectedIndex];
                SetMonitorBrightness(selectedMonitor.hPhysicalMonitor, (uint)brightness);
            }
        }

        private void LogError(string message)
        {
            // Implement your logging here. For example:
            Console.WriteLine($"{message}");
            // Optionally log to a file or monitoring system
        }

        ~MusixmatchIntegrationProto()
        {
            DestroyPhysicalMonitors();
        }

        private void DestroyPhysicalMonitors()
        {
            if (physicalMonitors != null && physicalMonitors.Count > 0)
            {
                PHYSICAL_MONITOR[] monitorArray = physicalMonitors.ToArray();
                DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
