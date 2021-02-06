using System.Windows;

namespace Rekog.App
{
    public partial class App : Application
    {
        public static double UnitSize => (double)Current.Resources["UnitSize"];
    }
}
