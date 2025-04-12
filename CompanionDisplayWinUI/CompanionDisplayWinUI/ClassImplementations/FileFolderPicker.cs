using System.Windows.Forms;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class FileFolderPicker
    {
        public static string[] OpenFileDialog(bool multiselect)
        {
            using OpenFileDialog openFileDialog1 = new() { DereferenceLinks = false, Multiselect = multiselect, InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs", FilterIndex = 0, RestoreDirectory = true };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
            string[] filename = openFileDialog1.FileNames;
            return filename;
        }
        public static string OpenFolder()
        {
            using FolderBrowserDialog openFileDialog1 = new() { InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs" };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
            string folder = openFileDialog1.SelectedPath;
            return folder;
        }
        public static string SaveFileDialog(string Filter, string Title)
        {
            SaveFileDialog saveFileDialog1 = new()
            {
                Filter = Filter,
                Title = Title
            };
            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }
    }
}
