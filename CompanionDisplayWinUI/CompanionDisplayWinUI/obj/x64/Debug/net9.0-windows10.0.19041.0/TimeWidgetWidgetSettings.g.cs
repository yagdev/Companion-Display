﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\TimeWidgetWidgetSettings.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7FCEED002EDEDB45AD90070A42D52B45B0E35BDFE7CEB3F6159955ED3854C5A2"
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
    partial class TimeWidgetWidgetSettings : 
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
            case 2: // TimeWidgetWidgetSettings.xaml line 30
                {
                    global::Microsoft.UI.Xaml.Controls.Button element2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element2).Click += this.Button_Click_3;
                }
                break;
            case 3: // TimeWidgetWidgetSettings.xaml line 31
                {
                    global::Microsoft.UI.Xaml.Controls.Button element3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element3).Click += this.Button_Click;
                }
                break;
            case 4: // TimeWidgetWidgetSettings.xaml line 32
                {
                    global::Microsoft.UI.Xaml.Controls.Button element4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element4).Click += this.Button_Click_1;
                }
                break;
            case 5: // TimeWidgetWidgetSettings.xaml line 17
                {
                    this.TimeColorPicker = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ColorPicker>(target);
                }
                break;
            case 6: // TimeWidgetWidgetSettings.xaml line 19
                {
                    this.DateColorPicker = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ColorPicker>(target);
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

