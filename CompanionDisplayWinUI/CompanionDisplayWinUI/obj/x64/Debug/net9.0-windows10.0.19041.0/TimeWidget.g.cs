﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\TimeWidget.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E2BF67A6A435492AFC2A4F2778689F58FAD742E28CE838687F3A37376AA653B1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CompanionDisplayWinUI
{
    partial class TimeWidget : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // TimeWidget.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.Page element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Page>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Page)element1).Unloaded += this.Page_Unloaded;
                    ((global::Microsoft.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // TimeWidget.xaml line 75
                {
                    this.Date = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 3: // TimeWidget.xaml line 76
                {
                    this.Time = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 4: // TimeWidget.xaml line 62
                {
                    this.Album = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 5: // TimeWidget.xaml line 67
                {
                    this.SongTitle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 6: // TimeWidget.xaml line 68
                {
                    global::Microsoft.UI.Xaml.Controls.HyperlinkButton element6 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.HyperlinkButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.HyperlinkButton)element6).Tapped += this.HyperlinkButton_Tapped;
                }
                break;
            case 7: // TimeWidget.xaml line 69
                {
                    this.PlayPause = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.HyperlinkButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.HyperlinkButton)this.PlayPause).Tapped += this.PlayPause_Tapped;
                }
                break;
            case 8: // TimeWidget.xaml line 65
                {
                    this.Album2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 9: // TimeWidget.xaml line 27
                {
                    this.TogglesView = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.GridView>(target);
                }
                break;
            case 10: // TimeWidget.xaml line 28
                {
                    this.WiFiToggleGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 11: // TimeWidget.xaml line 31
                {
                    this.BTToggleGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 12: // TimeWidget.xaml line 34
                {
                    this.DarkModeToggleGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 13: // TimeWidget.xaml line 37
                {
                    this.MuteToggleGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 14: // TimeWidget.xaml line 40
                {
                    this.AirPlaneToggleGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 15: // TimeWidget.xaml line 43
                {
                    this.ShutdownGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 16: // TimeWidget.xaml line 46
                {
                    this.RestartGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 17: // TimeWidget.xaml line 49
                {
                    this.SuspendGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 18: // TimeWidget.xaml line 52
                {
                    this.LockGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 19: // TimeWidget.xaml line 55
                {
                    this.LogoffGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 20: // TimeWidget.xaml line 56
                {
                    this.Logoff = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Logoff).Tapped += this.shutdown_Tapped;
                }
                break;
            case 21: // TimeWidget.xaml line 53
                {
                    this.Lock = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Lock).Tapped += this.shutdown_Tapped;
                }
                break;
            case 22: // TimeWidget.xaml line 50
                {
                    this.Suspend = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Suspend).Tapped += this.shutdown_Tapped;
                }
                break;
            case 23: // TimeWidget.xaml line 47
                {
                    this.Restart = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Restart).Tapped += this.shutdown_Tapped;
                }
                break;
            case 24: // TimeWidget.xaml line 44
                {
                    this.Shutdown = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Shutdown).Tapped += this.shutdown_Tapped;
                }
                break;
            case 25: // TimeWidget.xaml line 41
                {
                    this.AirPlaneToggle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.AirPlaneToggle).Tapped += this.AirPlaneToggle_Tapped;
                }
                break;
            case 26: // TimeWidget.xaml line 38
                {
                    this.MuteToggle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.MuteToggle).Tapped += this.MuteToggle_Tapped;
                }
                break;
            case 27: // TimeWidget.xaml line 35
                {
                    this.DarkModeToggle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.DarkModeToggle).Tapped += this.DarkModeToggle_Tapped;
                }
                break;
            case 28: // TimeWidget.xaml line 32
                {
                    this.BTToggle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.BTToggle).Tapped += this.BTToggle_Tapped;
                }
                break;
            case 29: // TimeWidget.xaml line 29
                {
                    this.WiFiToggle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.WiFiToggle).Tapped += this.WiFiToggle_Tapped;
                }
                break;
            case 30: // TimeWidget.xaml line 24
                {
                    this.Edit = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.Edit).Checked += this.Edit_Checked;
                    ((global::Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)this.Edit).Unchecked += this.Edit_Unchecked;
                }
                break;
            case 31: // TimeWidget.xaml line 19
                {
                    this.FullDate = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 32: // TimeWidget.xaml line 21
                {
                    this.TimeLeft = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 33: // TimeWidget.xaml line 16
                {
                    this.BTStackBattery = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.StackPanel>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

