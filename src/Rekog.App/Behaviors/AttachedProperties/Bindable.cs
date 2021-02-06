using System.Windows;

namespace Rekog.App.Behaviors.AttachedProperties
{
    public static class Bindable
    {
        public static double GetY(DependencyObject obj) => (double)obj.GetValue(YProperty);
        public static void SetY(DependencyObject obj, double value) => obj.SetValue(YProperty, value);

        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached("Y", typeof(double), typeof(Bindable), new PropertyMetadata(0d));
    }
}
