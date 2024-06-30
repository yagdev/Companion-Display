using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input.Preview.Injection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NumPadWidget : Page
    {
        [LibraryImport("user32.dll")]
        private static partial short GetKeyState(int keyCode);
        [LibraryImport("user32.dll", SetLastError = true)]
        static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(VirtualKey key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            else
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        }
        public NumPadWidget()
        {
            this.InitializeComponent();
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        public bool CleanUp = false;
        private void UpdateUI()
        {
            Windows.UI.Core.CoreVirtualKeyStates numkey = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.NumberKeyLock);
            if (numkey != Windows.UI.Core.CoreVirtualKeyStates.Locked)
            {
            DispatcherQueue.TryEnqueue(() =>
            {
                Numpad0.Content = "Ins";
                Numpad1.Content = "End";
                Numpad2.Content = "↓";
                Numpad3.Content = "PgDn";
                Numpad4.Content = "←";
                Numpad5.Content = "";
                Numpad6.Content = "→";
                Numpad7.Content = "Home";
                Numpad8.Content = "↑";
                Numpad9.Content = "PgUp";
                NumpadDot.Content = "Del";
            });
            }
            else
            {
            DispatcherQueue.TryEnqueue(() =>
            {
                Numpad0.Content = "0";
                Numpad1.Content = "1";
                Numpad2.Content = "2";
                Numpad3.Content = "3";
                Numpad4.Content = "4";
                Numpad5.Content = "5";
                Numpad6.Content = "6";
                Numpad7.Content = "7";
                Numpad8.Content = "8";
                Numpad9.Content = "9";
                NumpadDot.Content = ".";
            });
            }
            
            {
                Thread.Sleep(100);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        private void PressKey(object sender, RoutedEventArgs e)
        {
            Windows.UI.Core.CoreVirtualKeyStates numkey = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.NumberKeyLock);
            try
            {
                if (numkey != Windows.UI.Core.CoreVirtualKeyStates.Locked)
                {
                    PressKey((VirtualKey)int.Parse(((RepeatButton)sender).DataContext.ToString()), up: false);
                    PressKey((VirtualKey)int.Parse(((RepeatButton)sender).DataContext.ToString()), up: true);
                }
                else
                {
                    PressKey((VirtualKey)int.Parse(((RepeatButton)sender).Tag.ToString()), up: false);
                    PressKey((VirtualKey)int.Parse(((RepeatButton)sender).Tag.ToString()), up: true);
                }
            }
            catch
            {
                PressKey((VirtualKey)int.Parse(((ToggleButton)sender).Tag.ToString()), up: false);
                PressKey((VirtualKey)int.Parse(((ToggleButton)sender).Tag.ToString()), up: true);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
    }
}
