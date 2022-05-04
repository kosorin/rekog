using System.Windows;
using Koda.ColorTools.Wpf;
using ModernWpf.Controls;
using Rekog.App.View;
using Rekog.App.ViewModel;

namespace Rekog.App
{
    public partial class App
    {
        public static double UnitSize => (double)Current.Resources["UnitSize"];

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainViewModel();
            MainWindow.Show();
        }

        private void ColorPickerFlyout_OnOpened(object sender, object args)
        {
            if (sender is Flyout { Content: ColorPicker picker, })
            {
                picker.OriginalColor = picker.SelectedColor;
            }
        }
    }
}
