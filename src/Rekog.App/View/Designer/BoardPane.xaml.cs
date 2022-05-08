using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;

namespace Rekog.App.View.Designer
{
    public partial class BoardPane
    {
        public BoardPane()
        {
            InitializeComponent();
        }

        private void OnShowFlyoutButtonClick(object sender, RoutedEventArgs args)
        {
            if (sender is ButtonBase button && FlyoutBase.GetAttachedFlyout(button) is { } flyout)
            {
                flyout.ShowAt(button);
            }
        }

        private void OnHideFlyoutButtonClick(object sender, RoutedEventArgs args)
        {
            if (sender is ButtonBase button && FlyoutBase.GetAttachedFlyout(button) is { } flyout)
            {
                flyout.Hide();
            }
        }

        private void OnFormTabPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (sender is ListBoxItem { IsSelected: false, Parent: not ListBox, } item)
            {
                item.IsSelected = true;
            }
        }

        private void OnFormTabPreviewKeyDown(object sender, KeyEventArgs args)
        {
            if (sender is ListBoxItem { IsSelected: false, Parent: not ListBox, } item && args.Key == Key.Space)
            {
                item.IsSelected = true;
            }
        }
    }
}
