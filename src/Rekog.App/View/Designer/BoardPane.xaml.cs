using System.Windows.Controls;
using System.Windows.Input;

namespace Rekog.App.View.Designer
{
    public partial class BoardPane
    {
        public BoardPane()
        {
            InitializeComponent();
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
