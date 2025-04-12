using Microsoft.UI.Xaml.Controls;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class Windowing
    {
        public static void RemoveWidget(Frame currentWidget)
        {
            GridView gridView = currentWidget.Parent as GridView;
            gridView.Items.Remove(currentWidget);
        }
    }
}
