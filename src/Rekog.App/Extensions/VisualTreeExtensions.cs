using System.Windows;
using System.Windows.Media;

namespace Rekog.App.Extensions
{
    public static class VisualTreeExtensions
    {
        public static T? FindChild<T>(this DependencyObject parent, string? name = null) 
            where T : FrameworkElement
        {
            T? child = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var element = (FrameworkElement)VisualTreeHelper.GetChild(parent, i);

                child = element as T;
                if (child != null)
                {
                    if (name == null || name == child.Name)
                    {
                        break;
                    }
                }

                child = element.FindChild<T>(name);
            }

            return child;
        }

        public static T? FindParent<T>(this DependencyObject child, string? name = null) 
            where T : FrameworkElement
        {
            T? parent = null;

            var current = child as FrameworkElement;
            while (current != null)
            {
                parent = current.Parent as T;
                if (parent != null)
                {
                    if (name == null || name == parent.Name)
                    {
                        break;
                    }
                    else
                    {
                        parent = null;
                    }
                }

                current = current.Parent as FrameworkElement;
            }

            return parent;
        }
    }
}
