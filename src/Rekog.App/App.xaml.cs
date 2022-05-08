using System.Windows;
using Koda.ColorTools.Wpf;
using ModernWpf.Controls;
using Rekog.App.View;
using Rekog.App.ViewModel;

namespace Rekog.App
{
    public partial class App
    {
        public static double UnitSize => (double)Current.Resources[nameof(UnitSize)];

        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainViewModel();
            MainWindow.Show();
        }

        private void OnColorPickerFlyoutOpened(object sender, object args)
        {
            if (sender is Flyout { Content: ColorPicker picker, })
            {
                picker.OriginalColor = picker.SelectedColor;
            }
        }
    }
}
