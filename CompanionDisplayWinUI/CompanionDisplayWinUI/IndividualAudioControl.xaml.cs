using CoreAudio;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualAudioControl : Page
    {
        public IndividualAudioControl()
        {
            this.InitializeComponent();
        }
        private AudioSessionControl2 audio;
        private MMDevice mDevice;
        private bool IsGlobal = false, IsStarted = false, IsVisible = false;
        private float LastValue = -2;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsVisible = true;
            if (!IsStarted)
            {
                if ((this.Parent as Frame).Name == "GLOBAL")
                {
                    try
                    {
                        mDevice = (this.Parent as Frame).Tag as MMDevice;
                        Volume.Value = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                        SysIcon.Visibility = Visibility.Visible;
                        ProcessName.Text = "Master Volume";
                        IsGlobal = true;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        audio = (this.Parent as Frame).Tag as AudioSessionControl2;
                        Volume.Value = audio.SimpleAudioVolume.MasterVolume * 100;
                        if (audio.IsSystemSoundsSession)
                        {
                            ProcessName.Text = "System sounds";
                            SysIcon.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            using (var p = Process.GetProcessById((int)audio.ProcessID)) { ProcessName.Text = p.ProcessName; }
                            SysIcon.Visibility = Visibility.Collapsed;
                        }
                        using (var process = Process.GetProcessById((int)audio.ProcessID))
                        using (Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName))
                        using (Bitmap bitmap = icon.ToBitmap())
                        using (MemoryStream iconStream = new())
                        {
                            bitmap.Save(iconStream, ImageFormat.Png);
                            iconStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                            ProcessImage.Source = bitmapImage;
                        }
                    }
                    catch
                    {

                    }
                }
                IsStarted = true;
            }
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private void UpdateUI()
        {
            if (IsGlobal)
            {
                using (mDevice)
                {
                    if (mDevice.AudioEndpointVolume.MasterVolumeLevelScalar != LastValue)
                    {
                        LastValue = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Volume.Value = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                            if (mDevice.AudioEndpointVolume.Mute == true)
                            {
                                SysIcon.Symbol = Symbol.Mute;
                            }
                            else
                            {
                                SysIcon.Symbol = Symbol.Volume;
                            }
                        });
                    }
                }
            }
            else
            {
                using (audio)
                {
                    if (audio.SimpleAudioVolume.MasterVolume != LastValue)
                    {
                        LastValue = audio.SimpleAudioVolume.MasterVolume;
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Volume.Value = audio.SimpleAudioVolume.MasterVolume * 100;
                            if (audio.SimpleAudioVolume.Mute == true)
                            {
                                if (!audio.IsSystemSoundsSession)
                                {
                                    SysIcon.Visibility = Visibility.Visible;
                                    ProcessImage.Visibility = Visibility.Collapsed;
                                }
                                SysIcon.Symbol = Symbol.Mute;
                            }
                            else
                            {
                                if (!audio.IsSystemSoundsSession)
                                {
                                    SysIcon.Visibility = Visibility.Collapsed;
                                    ProcessImage.Visibility = Visibility.Visible;
                                }
                                SysIcon.Symbol = Symbol.Volume;
                            }
                        });
                    }
                }
            }
            if (IsVisible)
            {
                Thread.Sleep(1000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsStarted)
            {
                if (IsGlobal)
                {
                    using (mDevice)
                    {
                        mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(Volume.Value / 100);
                    }
                }
                else
                {
                    using (mDevice)
                    {
                        audio.SimpleAudioVolume.MasterVolume = (float)(Volume.Value / 100);
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void SysIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(ProcessName.Text == "Master Volume")
            {
                using (mDevice)
                {
                    if (mDevice.AudioEndpointVolume.Mute == false)
                    {
                        SysIcon.Symbol = Symbol.Mute;
                        mDevice.AudioEndpointVolume.Mute = true;
                    }
                    else
                    {
                        SysIcon.Symbol = Symbol.Volume;
                        mDevice.AudioEndpointVolume.Mute = false;
                    }
                }
            }
            else
            {
                using (audio)
                {
                    if (audio.SimpleAudioVolume.Mute == false)
                    {
                        if (!audio.IsSystemSoundsSession)
                        {
                            SysIcon.Visibility = Visibility.Visible;
                            ProcessImage.Visibility = Visibility.Collapsed;
                        }
                        SysIcon.Symbol = Symbol.Mute;
                        audio.SimpleAudioVolume.Mute = true;
                    }
                    else
                    {
                        if (!audio.IsSystemSoundsSession)
                        {
                            SysIcon.Visibility = Visibility.Collapsed;
                            ProcessImage.Visibility = Visibility.Visible;
                        }
                        SysIcon.Symbol = Symbol.Volume;
                        audio.SimpleAudioVolume.Mute = false;
                    }
                }
            }
        }
    }
}
