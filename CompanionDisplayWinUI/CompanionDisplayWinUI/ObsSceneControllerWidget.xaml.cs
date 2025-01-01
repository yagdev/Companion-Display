using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.WSMan.Management;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ObsSceneControllerWidget : Page
    {
        public ObsSceneControllerWidget()
        {
            this.InitializeComponent();
            Globals.obsControls.obs.Disconnected += disconnectEvent;
            Globals.obsControls.obs.Connected += connectedEvent;
        }

        private void connectedEvent(object sender, EventArgs e)
        {
            revertLayout();
        }

        private void disconnectEvent(object sender, ObsDisconnectionInfo e)
        {
            Globals.obsControls.callUpdate -= updateUI;
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Visible;
            });
        }
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Thread thread = new(Globals.obsControls.manualConnectReq);
            thread.Start();
            System.Timers.Timer timer = new System.Timers.Timer(3000) { Enabled = true };
            (sender as Button).IsEnabled = false;
            timer.Elapsed += (sender, args) =>
            {
                if (!Globals.obsControls.connectionSuccessful)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ReconnectBtn.IsEnabled = true;
                        ReconnectBtn.Content = "Connection failed, try again.";
                        timer.Dispose();
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        revertLayout();
                    });
                }
            };
        }

        private void revertLayout()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Collapsed;
                Page_Loaded(null, null);
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.obsControls.connectionSuccessful)
            {
                updateUI();
                Globals.obsControls.callUpdate += updateUI;
            }
            else
            {
                OBSError.Visibility = Visibility.Visible;
            }
        }

        private void updateUI()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    TotalScenes.Items.Clear();
                    for (int i = 0; i < Globals.obsControls.scenes.Length; i++)
                    {
                        Grid grid = new Grid();
                        grid.Width = 90;
                        grid.Height = 90;
                        TextBlock individualScene = new();
                        individualScene.Text = Globals.obsControls.scenes[i].Name;
                        individualScene.TextWrapping = TextWrapping.WrapWholeWords;
                        individualScene.VerticalAlignment = VerticalAlignment.Center;
                        individualScene.HorizontalAlignment = HorizontalAlignment.Center;
                        grid.Children.Add(individualScene);
                        TotalScenes.Items.Insert(0, grid);
                        if (Globals.obsControls.scenes[i].Name == Globals.obsControls.currentSession)
                        {
                            TotalScenes.SelectedIndex = 0;
                            if (Application.Current.Resources.TryGetValue("SystemAccentColor", out var accentColorObj) && accentColorObj is Windows.UI.Color accentColor)
                            {
                                // Apply the brush to the rectangle's Fill property
                                grid.Background = new SolidColorBrush(accentColor);
                            }
                        }
                    }
                }
                catch
                {

                }
            });
        }

        private void TotalScenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Globals.obsControls.setScene(((TotalScenes.SelectedItem as Grid).Children[0] as TextBlock).Text);
            }
            catch
            {

            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.obsControls.callUpdate -= updateUI;
        }
    }
}
