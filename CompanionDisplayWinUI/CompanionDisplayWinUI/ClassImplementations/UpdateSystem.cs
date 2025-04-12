using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class UpdateSystem
    {
        public async static void PerformUpdate()
        {
            if (Globals.IsBetaProgram)
            {
                using var s = await CommonlyAccessedInstances.client.GetStreamAsync(Globals.UpdateZipBeta);
                using var fs = new FileStream("release.zip", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
            else
            {
                using var s = await CommonlyAccessedInstances.client.GetStreamAsync(Globals.UpdateZip);
                using var fs = new FileStream("release.zip", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
            CMDOperations.PerformCMDCommand("mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move release.zip ../ & move setup.exe ../ & cd .. & tar -xf release.zip & del /f /q release.zip & taskkill /f /im CompanionDisplayWinUI.exe & timeout 1 & rmdir /s /q Update & CompanionDisplayWinUI.exe");
        }
        public static async Task CheckUpdate()
        {
            using HttpClient client = new();
            string reply;
            if (Globals.IsBetaProgram)
            {
                reply = await client.GetStringAsync(Globals.UpdateStringBeta);
            }
            else
            {
                reply = await client.GetStringAsync(Globals.UpdateString);
            }
            Globals.IsUpdateAvailable = !(reply == Globals.Version);
        }
    }
}
