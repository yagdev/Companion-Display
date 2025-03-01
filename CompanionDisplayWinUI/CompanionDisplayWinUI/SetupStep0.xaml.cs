using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanionDisplayWinUI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SetupStep0 : Page
	{
		public SetupStep0()
		{
			this.InitializeComponent();
		}
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep1), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            navviewparent.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)navviewparent.FindName("HomeItem");
            navviewparent.IsPaneVisible = true;
            frame.Navigate(typeof(BlankPage1), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
