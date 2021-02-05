using System.Windows;

namespace Rekog.App.AttachedProperties
{
    public static class Board
    {
        public static Thickness GetCanvasOffset(DependencyObject obj) => (Thickness)obj.GetValue(CanvasOffsetProperty);
        public static void SetCanvasOffset(DependencyObject obj, Thickness value) => obj.SetValue(CanvasOffsetProperty, value);

        public static readonly DependencyProperty CanvasOffsetProperty =
            DependencyProperty.RegisterAttached("CanvasOffset", typeof(Thickness), typeof(Board), new PropertyMetadata(new Thickness()));
    }
}
