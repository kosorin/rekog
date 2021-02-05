using System.Windows;

namespace Rekog.App.AttachedProperties
{
    public static class Key
    {
        public static Rect GetBounds(DependencyObject obj) => (Rect)obj.GetValue(BoundsProperty);
        public static void SetBounds(DependencyObject obj, Rect value) => obj.SetValue(BoundsProperty, value);

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.RegisterAttached("Bounds", typeof(Rect), typeof(Key), new PropertyMetadata(new Rect()));

        public static bool GetIsPreviewSelected(DependencyObject obj) => (bool)obj.GetValue(IsPreviewSelectedProperty);
        public static void SetIsPreviewSelected(DependencyObject obj, bool value) => obj.SetValue(IsPreviewSelectedProperty, value);

        public static readonly DependencyProperty IsPreviewSelectedProperty =
            DependencyProperty.RegisterAttached("IsPreviewSelected", typeof(bool), typeof(Key), new PropertyMetadata(false));

        public static bool GetIsSelected(DependencyObject obj) => (bool)obj.GetValue(IsSelectedProperty);
        public static void SetIsSelected(DependencyObject obj, bool value) => obj.SetValue(IsSelectedProperty, value);

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(Key), new PropertyMetadata(false));
    }
}
