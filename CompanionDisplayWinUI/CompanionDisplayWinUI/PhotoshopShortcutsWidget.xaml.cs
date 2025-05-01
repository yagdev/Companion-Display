using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanionDisplayWinUI.ClassImplementations
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PhotoshopShortcutsWidget : Page
	{
		public PhotoshopShortcutsWidget()
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
