﻿#pragma checksum "C:\Users\Dinis\source\repos\CompanionDisplayWinUI\CompanionDisplayWinUI\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3F0018251D4A6429D8B6ED1B94221BE5633AECE68F13DC6DEE144CADF322F4B1"
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
    partial class MainWindow : 
        global::Microsoft.UI.Xaml.Window, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Microsoft_UI_Xaml_Controls_NavigationView_SelectedItem(global::Microsoft.UI.Xaml.Controls.NavigationView obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.SelectedItem = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private partial class MainWindow_obj1_Bindings :
            global::Microsoft.UI.Xaml.Markup.IDataTemplateComponent,
            global::Microsoft.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Microsoft.UI.Xaml.Markup.IComponentConnector,
            IMainWindow_Bindings
        {
            private global::CompanionDisplayWinUI.MainWindow dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.UI.Xaml.Controls.NavigationView obj7;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj7SelectedItemDisabled = false;

            public MainWindow_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 20 && columnNumber == 56)
                {
                    isobj7SelectedItemDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 7: // MainWindow.xaml line 20
                        this.obj7 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationView>(target);
                        break;
                    default:
                        break;
                }
            }
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
                        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target) 
                        {
                            return null;
                        }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IMainWindow_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = global::WinRT.CastExtensions.As<global::CompanionDisplayWinUI.MainWindow>(newDataRoot);
                    return true;
                }
                return false;
            }

            public void Activated(object obj, global::Microsoft.UI.Xaml.WindowActivatedEventArgs data)
            {
                this.Initialize();
            }

            public void Loading(global::Microsoft.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::CompanionDisplayWinUI.MainWindow obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_HomeItem(obj.HomeItem, phase);
                    }
                }
            }
            private void Update_HomeItem(global::Microsoft.UI.Xaml.Controls.NavigationViewItem obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // MainWindow.xaml line 20
                    if (!isobj7SelectedItemDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_NavigationView_SelectedItem(this.obj7, obj, null);
                    }
                }
            }
        }

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // MainWindow.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Window element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Window>(target);
                    ((global::Microsoft.UI.Xaml.Window)element1).Closed += this.Window_Closed;
                }
                break;
            case 2: // MainWindow.xaml line 15
                {
                    this.GridMain = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 3: // MainWindow.xaml line 16
                {
                    this.BackgroundImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Image)this.BackgroundImage).ImageOpened += this.BackgroundImage_ImageOpened;
                }
                break;
            case 4: // MainWindow.xaml line 17
                {
                    this.ImageOptionalBlur = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 5: // MainWindow.xaml line 18
                {
                    this.SleepReptangle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 6: // MainWindow.xaml line 19
                {
                    this.BackgroundVideo = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MediaPlayerElement>(target);
                }
                break;
            case 7: // MainWindow.xaml line 20
                {
                    this.nvSample = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationView>(target);
                    ((global::Microsoft.UI.Xaml.Controls.NavigationView)this.nvSample).SelectionChanged += this.NavigationView_SelectionChanged;
                    ((global::Microsoft.UI.Xaml.Controls.NavigationView)this.nvSample).Loaded += this.nvSample_Loaded;
                }
                break;
            case 8: // MainWindow.xaml line 38
                {
                    this.CornerMask = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 9: // MainWindow.xaml line 39
                {
                    this.AppTitleBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 10: // MainWindow.xaml line 27
                {
                    this.DebugPage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.NavigationViewItem)this.DebugPage).Tapped += this.DebugPage_Tapped;
                }
                break;
            case 11: // MainWindow.xaml line 28
                {
                    this.HomeItem = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                }
                break;
            case 12: // MainWindow.xaml line 29
                {
                    this.BrowserPage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                }
                break;
            case 13: // MainWindow.xaml line 30
                {
                    this.SpotifyBuiltInPage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                }
                break;
            case 14: // MainWindow.xaml line 31
                {
                    this.SecretStuff = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                }
                break;
            case 15: // MainWindow.xaml line 34
                {
                    this.SleepPage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.NavigationViewItem>(target);
                }
                break;
            case 16: // MainWindow.xaml line 36
                {
                    this.contentFrame = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Frame)this.contentFrame).IsEnabledChanged += this.ContentFrame_IsEnabledChanged;
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
            switch(connectionId)
            {
            case 1: // MainWindow.xaml line 2
                {                    
                    global::Microsoft.UI.Xaml.Window element1 = (global::Microsoft.UI.Xaml.Window)target;
                    MainWindow_obj1_Bindings bindings = new MainWindow_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Activated += bindings.Activated;
                }
                break;
            }
            return returnValue;
        }
    }
}

