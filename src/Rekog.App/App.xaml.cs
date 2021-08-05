using Rekog.App.View;
using System.Windows;
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
    }
}
