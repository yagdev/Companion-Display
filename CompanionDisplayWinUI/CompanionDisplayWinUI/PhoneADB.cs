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
        public void Start()
        {
            //if (File.Exists("ADB_Configuration.crlh"))
            //{
            //    StreamReader streamReader = new StreamReader("ADB_Configuration.crlh");
            //    if (streamReader.ReadLine() != null)
            //    {
            //        ID = streamReader.ReadLine();
            //        process2.StartInfo.Arguments = "/C runtimes\\adb.exe connect  " + ID;
            //        process2.Start();
            //        Thread thread = new Thread(Phone1);
            //        thread.Start();
            //    }
            //    if (streamReader.ReadLine() != null)
            //    {
            //        Globals.ADB2 = streamReader.ReadLine();
            //        process2.StartInfo.Arguments = "/C runtimes\\adb.exe connect  " + Globals.ADB2;
            //        process2.Start();
            //        Thread thread = new Thread(Phone2);
            //        thread.Start();
            //    }
            //}
        }
        public double GetDeviceBrightness(string ID)
        {
            Process process = new();
            process.StartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "CMD.exe",
                CreateNoWindow = true
            };
            process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell settings get system screen_brightness";
            process.Start();
            process.WaitForExit();
            double brightness;
            try
            {
                brightness = Double.Parse(process.StandardOutput.ReadToEnd());
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
                brightness = 0;
            }
            return brightness;
        }
        public string GetDeviceName(string ID)
        {
            Process process = new();
            process.StartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "CMD.exe",
                CreateNoWindow = true
            };
            process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell getprop ro.product.model";
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }
        public int GetDeviceBattery(string ID)
        {
            int level0 = 0;
            try
            {
                Process process = new();
                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "CMD.exe",
                    CreateNoWindow = true
                };
                process.StartInfo.Arguments = "/C runtimes\\adb.exe -s " + ID + " shell dumpsys battery | findstr level";
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd().Remove(0, 9);
                level0 = int.Parse(output);
            }
            catch (Exception ex)
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
            }
            return level0;
        }
    }
}
