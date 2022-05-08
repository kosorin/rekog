using System.Windows.Input;
using ModernWpf;

namespace Rekog.App.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMainWindowPreviewKeyDown(object? sender, KeyEventArgs args)
        {
            args.Handled = true;

            switch (args.Key)
            {
                case Key.Escape:
                    if (!Board.IsFocused)
                    {
                        Board.Focus();
                    }
                    break;
                case Key.F8:
                    ToggleTheme();
                    break;
                default:
                    args.Handled = false;
                    break;
            }
        }

        private void ToggleTheme()
        {
            ThemeManager.SetRequestedTheme(this, ThemeManager.GetActualTheme(this) == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);
        }
    }
}
