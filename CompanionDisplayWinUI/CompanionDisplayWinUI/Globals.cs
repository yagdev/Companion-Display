using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI
{
    static class Globals
    {
        // Runtime variables
        public static string Version = "24.7";
        public static string UpdateZip = "https://www.dropbox.com/scl/fi/3pmay1a18v9wdi6y9ridh/release.zip?rlkey=9sbehxn68p4i18geb5acbe6y3&dl=1";
        public static string UpdateString = "https://www.dropbox.com/scl/fi/eftx6ql3107c1j0gywb90/release.txt?rlkey=lylcncrgnhtw2q3w1l439uc45&dl=1";
        public static string DiscordID = "";
        public static string SP_DC = "";
        public static string _clientId = "";
        public static string _secretId = "";
        public static string _clientId2 = "";
        public static string _secretId2 = "";
        public static string MusixMatchToken = "";
        public static string RefreshToken = "";
        public static string RefreshToken2 = "";
        public static string SpotifyFullStatus = "";
        public static string SongID = "";
        public static string SongName, SongDetails, SongLyrics, SongTime, SongEnd, SongBackground;
        public static bool BackgroundChanged, IsSpotify, StartedPlayer = false;
        public static double SongProgress;
        public static int currenttimestamp;
        public static bool IsAllApps = false;
        public static bool InjectedCustomAccent = false;
        public static bool ImageFailed = false;
        public static string LastWebPage = "";
        public static string CurrentHW = "";
        // App config files
        public static string RefreshTokenPath = "Config/RefreshToken.crlh";
        public static string RefreshToken2Path = "Config/RefreshToken2.crlh";
        public static string MediaConfigFile = "Config/MediaConfig.crlh";
        public static string TimeConfigFile = "Config/TimeConfig.crlh";
        public static string PhotoConfigFile = "Config/PhotoConfig.crlh";
        // Loaded Configs
        public static int ColorSchemeSelect = 0;
        public static int ColorSchemeSelectAccentR = 0;
        public static int ColorSchemeSelectAccentG = 0;
        public static int ColorSchemeSelectAccentB = 0;
        public static int BackgroundColorR = 0;
        public static int BackgroundColorG = 0;
        public static int BackgroundColorB = 0;
        public static int InjectCustomAccent = 0;
        public static int Backdrop = 0;
        public static string BackgroundLink = "";
        public static string Wallpaper = "";
        public static bool Blur = true;
        public static bool StealFocus = false;
        public static bool IgnoreUpdates = false;
        public static bool IsUpdateAvailable = false;
        public static bool SmartFlipToggle = false;
    }
}
