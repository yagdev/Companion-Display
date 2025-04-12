﻿using IWshRuntimeLibrary;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class ShortcutWorkings
    {
        public static void CreateShortcut(string shortcutAddress, string Description, string TargetPath, string WorkingDirectory)
        {
            WshShell shell = new();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = Description;
            shortcut.TargetPath = TargetPath;
            shortcut.WorkingDirectory = WorkingDirectory;
            shortcut.Save();
        }
    }
}
