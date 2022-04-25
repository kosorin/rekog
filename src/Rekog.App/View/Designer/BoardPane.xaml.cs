using System.Windows;
using ModernWpf.Controls.Primitives;

namespace Rekog.App.View.Designer
{
    public partial class BoardPane
    {
        public BoardPane()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs args)
        {
            if (sender is FrameworkElement element)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }
    }
}
