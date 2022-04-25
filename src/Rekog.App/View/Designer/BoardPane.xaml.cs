﻿using System.Windows;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls.Primitives;

namespace Rekog.App.View.Designer
{
    public partial class BoardPane
    {
        public BoardPane()
        {
            InitializeComponent();
        }

        private void ShowFlyoutButton_OnClick(object sender, RoutedEventArgs args)
        {
            if (sender is ButtonBase button && FlyoutBase.GetAttachedFlyout(button) is { } flyout)
            {
                flyout.ShowAt(button);
            }
        }

        private void HideFlyoutButton_OnClick(object sender, RoutedEventArgs args)
        {
            if (sender is ButtonBase button && FlyoutBase.GetAttachedFlyout(button) is { } flyout)
            {
                flyout.Hide();
            }
        }
    }
}
