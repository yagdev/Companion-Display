﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\WidgetAudioControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2FCA18F99767B5332EE20F4342E5E963FE1727A36830AD7F3E375CFB332B6110"
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
    partial class WidgetAudioControl : 
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
            case 1: // WidgetAudioControl.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.Page element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Page>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // WidgetAudioControl.xaml line 14
                {
                    this.CurrentDevice = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.DropDownButton>(target);
                }
                break;
            case 3: // WidgetAudioControl.xaml line 20
                {
                    this.DeviceView = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                }
                break;
            case 4: // WidgetAudioControl.xaml line 16
                {
                    this.ListOfDevices = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyout>(target);
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

