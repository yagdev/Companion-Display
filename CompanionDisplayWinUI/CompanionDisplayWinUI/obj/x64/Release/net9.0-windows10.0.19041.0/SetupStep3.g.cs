﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\SetupStep3.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "ED16E3862DEFC10A533E58E16937315960FF40D6EA797A85E041044B7B2B0169"
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
    partial class SetupStep3 : 
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
            case 1: // SetupStep3.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.Page element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Page>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // SetupStep3.xaml line 55
                {
                    global::Microsoft.UI.Xaml.Controls.HyperlinkButton element2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.HyperlinkButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.HyperlinkButton)element2).Tapped += this.HyperlinkButton_Tapped;
                }
                break;
            case 3: // SetupStep3.xaml line 52
                {
                    global::Microsoft.UI.Xaml.Controls.Button element3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element3).Tapped += this.Button_Tapped;
                }
                break;
            case 4: // SetupStep3.xaml line 53
                {
                    global::Microsoft.UI.Xaml.Controls.Button element4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element4).Tapped += this.Button_Tapped_1;
                }
                break;
            case 5: // SetupStep3.xaml line 42
                {
                    this.SleepModeColor = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ColorPicker>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ColorPicker)this.SleepModeColor).ColorChanged += this.SleepModeColor_ColorChanged;
                }
                break;
            case 6: // SetupStep3.xaml line 43
                {
                    global::Microsoft.UI.Xaml.Controls.Button element6 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element6).Click += this.Button_Click;
                }
                break;
            case 7: // SetupStep3.xaml line 38
                {
                    this.OvrColorSleepMode = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.CheckBox>(target);
                }
                break;
            case 8: // SetupStep3.xaml line 34
                {
                    this.Opacity = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.Opacity).ValueChanged += this.Opacity_ValueChanged;
                }
                break;
            case 9: // SetupStep3.xaml line 22
                {
                    this.DemoUnderflow = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.StackPanel>(target);
                }
                break;
            case 10: // SetupStep3.xaml line 23
                {
                    this.DateDemo = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 11: // SetupStep3.xaml line 24
                {
                    this.TimeDemo = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
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

