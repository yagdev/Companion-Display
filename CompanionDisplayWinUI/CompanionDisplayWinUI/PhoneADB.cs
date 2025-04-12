using CompanionDisplayWinUI.ClassImplementations;
using System;

namespace CompanionDisplayWinUI
{
    internal class PhoneADB
    {
        public static double GetDeviceBrightness(string ID)
        {
            try
            {
                return(Double.Parse(CMDOperations.GetCMDLog("runtimes\\adb.exe -s " + ID + " shell settings get system screen_brightness")));
            }
            catch
            {
                return 0;
            }
        }
        public static string GetDeviceName(string ID)
        {
            try
            {
                return CMDOperations.GetCMDLog("runtimes\\adb.exe -s " + ID + " shell getprop ro.product.model");
            }
            catch
            {
                return AppStrings.adbPhoneNameError;
            }
        }
        public static int GetDeviceBattery(string ID)
        {
            try
            {
                return int.Parse(CMDOperations.GetCMDLog("runtimes\\adb.exe -s " + ID + " shell dumpsys battery | findstr level").Replace(CMDOperations.GetCMDLog("runtimes\\adb.exe -s " + ID + " shell dumpsys battery | findstr Capacity"), "")[9..]);
            }
            catch
            {
                return 0;
            }
        }
    }
}
