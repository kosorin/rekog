using System.Windows;
using System.Windows.Controls;

namespace Rekog.App.Behaviors.AttachedProperties
{
    public static class TabItemHelper
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("Header", typeof(object), typeof(TabItemHelper));

        public static object GetHeader(TabItem tabItem)
        {
            return tabItem.GetValue(HeaderProperty);
        }

        public static void SetHeader(TabItem tabItem, object value)
        {
            tabItem.SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(TabItemHelper));

        public static DataTemplate GetHeaderTemplate(TabItem tabItem)
        {
            return (DataTemplate)tabItem.GetValue(HeaderTemplateProperty);
        }

        public static void SetHeaderTemplate(TabItem tabItem, DataTemplate value)
        {
            tabItem.SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.RegisterAttached("Footer", typeof(object), typeof(TabItemHelper));

        public static object GetFooter(TabItem tabItem)
        {
            return tabItem.GetValue(FooterProperty);
        }

        public static void SetFooter(TabItem tabItem, object value)
        {
            tabItem.SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.RegisterAttached("FooterTemplate", typeof(DataTemplate), typeof(TabItemHelper));

        public static DataTemplate GetFooterTemplate(TabItem tabItem)
        {
            return (DataTemplate)tabItem.GetValue(FooterTemplateProperty);
        }

        public static void SetFooterTemplate(TabItem tabItem, DataTemplate value)
        {
            tabItem.SetValue(FooterTemplateProperty, value);
        }
    }
}
