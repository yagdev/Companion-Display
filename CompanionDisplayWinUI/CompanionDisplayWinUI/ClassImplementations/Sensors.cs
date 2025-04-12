using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class Sensors
    {
        public static void UpdateSensorValue(ISensor sensor1, double lastValue, TextBlock textBlock, ProgressRing ring, string termination, DispatcherQueue dispatcher, bool extraPrecision)
        {
            try
            {
                int mult = 1;
                if (extraPrecision)
                {
                    mult = 100;
                }
                double value = (Math.Round((float)(sensor1.Value) * mult)) / mult;
                if (value != lastValue && (Globals.CurrentHW == sensor1.Hardware || Globals.CurrentHW == sensor1.Hardware.Parent) && lastValue != -2 && Math.Round((float)sensor1.Value) != lastValue)
                {
                    dispatcher.TryEnqueue(() =>
                    {
                        textBlock.Text = value + termination;
                        if (ring != null)
                        {
                            ring.Value = value;
                            if(ring.Maximum != 100)
                            {
                                ring.Maximum = sensor1.Max.Value;
                            }
                        }
                    });
                }
            }
            catch { }
        }
    }
}
