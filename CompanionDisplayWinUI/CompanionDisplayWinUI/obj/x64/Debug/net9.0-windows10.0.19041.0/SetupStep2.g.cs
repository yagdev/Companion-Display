﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\SetupStep2.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "DE0CBEDC9A93AFDD44E8BD8870D75509D09DF811D9C630942E3CA7CACBE033C4"
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
    partial class SetupStep2 : 
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
            case 1: // SetupStep2.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.Page element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Page>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // SetupStep2.xaml line 21
                {
                    this.obsIP = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                }
                break;
            case 3: // SetupStep2.xaml line 22
                {
                    this.obsPass = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                }
                break;
            case 4: // SetupStep2.xaml line 38
                {
                    global::Microsoft.UI.Xaml.Controls.HyperlinkButton element4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.HyperlinkButton>(target);
                    ((global::Microsoft.UI.Xaml.Controls.HyperlinkButton)element4).Tapped += this.HyperlinkButton_Tapped;
                }
                break;
            case 5: // SetupStep2.xaml line 35
                {
                    global::Microsoft.UI.Xaml.Controls.Button element5 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element5).Tapped += this.Button_Tapped;
                }
                break;
            case 6: // SetupStep2.xaml line 36
                {
                    global::Microsoft.UI.Xaml.Controls.Button element6 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element6).Tapped += this.Button_Tapped_1;
                }
                break;
            case 7: // SetupStep2.xaml line 26
                {
                    this.TwitchBrowser = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.WebView2>(target);
                    ((global::Microsoft.UI.Xaml.Controls.WebView2)this.TwitchBrowser).CoreWebView2Initialized += this.TwitchBrowser_CoreWebView2Initialized;
                }
                break;
            case 8: // SetupStep2.xaml line 28
                {
                    this.WebViewScaleTransform = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Media.ScaleTransform>(target);
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

