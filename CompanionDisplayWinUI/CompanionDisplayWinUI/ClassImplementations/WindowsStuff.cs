using System.Security.Principal;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class WindowsStuff
    {
        public static void SetAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            Globals.IsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
