using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class Launchpad
    {
        public static Button CreateLaunchPadButton(object content, FontFamily font, string name)
        {
            return new Button
            {
                Name = name,
                Height = 77,
                Width = 77,
                CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8),
                FontFamily = font,
                Content = content,
                FontSize = 32,
                AllowDrop = true
            };
        }
    }
}
