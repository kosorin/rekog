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

        public static readonly DependencyProperty UpdatePropertySourceWhenModifierKeysProperty =
            DependencyProperty.RegisterAttached("UpdatePropertySourceWhenModifierKeys", typeof(ModifierKeys), typeof(InputBindingManager), new PropertyMetadata(ModifierKeys.None));

        public static DependencyProperty? GetUpdatePropertySourceWhenEnterPressed(DependencyObject obj)
        {
            return (DependencyProperty?)obj.GetValue(UpdatePropertySourceWhenEnterPressedProperty);
        }

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject obj, DependencyProperty? value)
        {
            obj.SetValue(UpdatePropertySourceWhenEnterPressedProperty, value);
        }

        public static ModifierKeys GetUpdatePropertySourceWhenModifierKeys(DependencyObject obj)
        {
            return (ModifierKeys)obj.GetValue(UpdatePropertySourceWhenModifierKeysProperty);
        }

        public static void SetUpdatePropertySourceWhenModifierKeys(DependencyObject obj, ModifierKeys value)
        {
            obj.SetValue(UpdatePropertySourceWhenModifierKeysProperty, value);
        }

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not UIElement element)
            {
                return;
            }

            if (args.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (args.NewValue != null)
            {
                element.PreviewKeyDown += HandlePreviewKeyDown;
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Source is DependencyObject obj && args.Key == Key.Enter)
            {
                var modifierKeys = GetUpdatePropertySourceWhenModifierKeys(obj);
                if (modifierKeys == ModifierKeys.None || args.KeyboardDevice.Modifiers.HasFlag(modifierKeys))
                {
                    DoUpdateSource(obj);
                }
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
