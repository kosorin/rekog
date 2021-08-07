using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Rekog.App.Behaviors.AttachedProperties
{
    // https://stackoverflow.com/questions/563195/bind-textbox-on-enter-key-press
    public static class InputBindingManager
    {
        public static readonly DependencyProperty UpdatePropertySourceWhenEnterPressedProperty =
            DependencyProperty.RegisterAttached("UpdatePropertySourceWhenEnterPressed", typeof(DependencyProperty), typeof(InputBindingManager), new PropertyMetadata(null, OnUpdatePropertySourceWhenEnterPressedPropertyChanged));

        static InputBindingManager()
        {
        }

        public static DependencyProperty? GetUpdatePropertySourceWhenEnterPressed(DependencyObject obj)
        {
            return (DependencyProperty?)obj.GetValue(UpdatePropertySourceWhenEnterPressedProperty);
        }

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject obj, DependencyProperty? value)
        {
            obj.SetValue(UpdatePropertySourceWhenEnterPressedProperty, value);
        }

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is not UIElement element)
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (e.NewValue != null)
            {
                element.PreviewKeyDown += HandlePreviewKeyDown;
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is DependencyObject obj && e.Key == Key.Enter)
            {
                DoUpdateSource(obj);
            }
        }

        private static void DoUpdateSource(DependencyObject obj)
        {
            var property = GetUpdatePropertySourceWhenEnterPressed(obj);
            if (property == null)
            {
                return;
            }

            var binding = BindingOperations.GetBindingExpression(obj, property);
            binding?.UpdateSource();
        }
    }
}
