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
            switch (args.Key)
            {
                case Key.Escape when !Board.IsFocused:
                    Board.Focus();
                    args.Handled = true;
                    break;
                case Key.F8:
                    ToggleTheme();
                    args.Handled = true;
                    break;
            }
        }

        private void ToggleTheme()
        {
            ThemeManager.SetRequestedTheme(this, ThemeManager.GetActualTheme(this) == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);
        }
    }
}
