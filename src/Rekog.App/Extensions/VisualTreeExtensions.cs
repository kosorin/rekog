using System.Windows;
using System.Windows.Media;

namespace Rekog.App.Extensions
{
    public static class VisualTreeExtensions
    {
        public static T? FindChild<T>(this DependencyObject parent, string? name = null) where T : FrameworkElement
        {
            T? child = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var visual = (FrameworkElement)VisualTreeHelper.GetChild(parent, i);

                child = visual as T;
                if (child != null)
                {
                    if (name == null || name == child.Name)
                    {

                    }
                    break;
                }

                child = visual.FindChild<T>(name);
            }

            return child;
        }
    }
}
