using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Power;

namespace CompanionDisplayWinUI
{
    internal class PhoneADB
    {
        public static double GetDeviceBrightness(string ID)
        {
            using (Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                }
            })
            {
                process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell settings get system screen_brightness";
                process.Start();
                process.WaitForExit();
                double brightness;
                try
                {
                    brightness = Double.Parse(process.StandardOutput.ReadToEnd());
                }
                catch
                {
                    //File.AppendAllText("ErrorLog.crlh", ex.Message);
                    brightness = 0;
                }
                return brightness;
            }
        }
        public static string GetDeviceName(string ID)
        {
            using (Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                }
            })
            {
                process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell getprop ro.product.model";
                process.Start();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
        }
        public static int GetDeviceBattery(string ID)
        {
            int level0 = 0;
            try
            {
                using (Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "CMD.exe",
                        CreateNoWindow = true
                    }
                })
                {
                    process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell dumpsys battery | findstr level";
                    process.Start();
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd().Remove(0, 9);
                    level0 = int.Parse(output);
                }
            }
            catch
            {
            }
            return level0;
        }
    }
}
