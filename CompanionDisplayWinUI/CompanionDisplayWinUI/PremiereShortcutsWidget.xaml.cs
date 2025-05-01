using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CompanionDisplayWinUI.ClassImplementations;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PremiereShortcutsWidget : Page
    {
        public PremiereShortcutsWidget()
        {
            this.InitializeComponent();
        }
        private void PressKeyCTRL(object sender, TappedRoutedEventArgs e)
        {
            KeyPressAPI.callKeys(int.Parse((string)(sender as Button).Tag), 17);
        }

        private void PressKeyNoModifiers(object sender, TappedRoutedEventArgs e)
        {
            KeyPressAPI.callKeys(int.Parse((string)(sender as Button).Tag), -1);
        }
    }
}
