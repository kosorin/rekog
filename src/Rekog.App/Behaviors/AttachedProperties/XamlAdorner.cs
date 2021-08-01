using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Rekog.App.Behaviors.AttachedProperties
{
    // Source: https://www.codeproject.com/Articles/709266/Design-Adorners-in-XAML-with-Data-Binding-Support
    public class XamlAdorner : Adorner
    {
        private static readonly DependencyProperty AdornerProperty =
            DependencyProperty.RegisterAttached("Adorner", typeof(XamlAdorner), typeof(XamlAdorner), new PropertyMetadata(null));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(DataTemplate), typeof(XamlAdorner), new PropertyMetadata(null, OnTemplateChanged));

        public static readonly DependencyProperty TemplateSelectorProperty =
            DependencyProperty.RegisterAttached("TemplateSelector", typeof(DataTemplateSelector), typeof(XamlAdorner), new PropertyMetadata(null, OnTemplateSelectorChanged));

        public static readonly DependencyProperty IsAdornerVisibleProperty =
            DependencyProperty.RegisterAttached("IsAdornerVisible", typeof(bool), typeof(XamlAdorner), new PropertyMetadata(false, OnIsAdornerVisibleChanged));

        private readonly ContentPresenter _presenter;

        private XamlAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _presenter = new ContentPresenter();
            BindingOperations.SetBinding(_presenter, ContentPresenter.ContentProperty, new Binding(nameof(adornedElement.DataContext))
            {
                Source = adornedElement,
            });

            Template = GetTemplate(adornedElement)!;
            TemplateSelector = GetTemplateSelector(adornedElement)!;

            AddVisualChild(_presenter);
            AddLogicalChild(_presenter);
        }

        public DataTemplate Template
        {
            get => _presenter.ContentTemplate;
            set => _presenter.ContentTemplate = value;
        }

        public DataTemplateSelector TemplateSelector
        {
            get => _presenter.ContentTemplateSelector;
            set => _presenter.ContentTemplateSelector = value;
        }

        protected override int VisualChildrenCount => 1;

        private static XamlAdorner? GetAdorner(DependencyObject obj)
        {
            return (XamlAdorner?)obj.GetValue(AdornerProperty);
        }

        private static void SetAdorner(DependencyObject obj, XamlAdorner? value)
        {
            obj.SetValue(AdornerProperty, value);
        }

        public static DataTemplate? GetTemplate(DependencyObject obj)
        {
            return (DataTemplate?)obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, DataTemplate? value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        private static void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (GetAdorner(obj) is { } adorner)
            {
                adorner.Template = (DataTemplate)args.NewValue;
            }
        }

        public static DataTemplateSelector? GetTemplateSelector(DependencyObject obj)
        {
            return (DataTemplateSelector?)obj.GetValue(TemplateSelectorProperty);
        }

        public static void SetTemplateSelector(DependencyObject obj, DataTemplateSelector? value)
        {
            obj.SetValue(TemplateSelectorProperty, value);
        }

        private static void OnTemplateSelectorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (GetAdorner(obj) is { } adorner)
            {
                adorner.TemplateSelector = (DataTemplateSelector)args.NewValue;
            }
        }

        public static bool GetIsAdornerVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsAdornerVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAdornerVisibleProperty, value);
        }

        private static void OnIsAdornerVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not FrameworkElement adornedElement)
            {
                throw new InvalidOperationException("Adorners can only be applied to elements deriving from FrameworkElement.");
            }
            if (AdornerLayer.GetAdornerLayer(adornedElement) is not { } adornerLayer)
            {
                throw new InvalidOperationException("Cannot show adorner since no adorner layer was found in the visual tree.");
            }

            var adorner = GetAdorner(adornedElement);

            if (args.NewValue is true)
            {
                if (adorner == null)
                {
                    adorner = new XamlAdorner(adornedElement);

                    SetAdorner(adornedElement, adorner);
                    adornerLayer.Add(adorner);
                }
            }
            else
            {
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                    SetAdorner(adornedElement, null);
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _presenter;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _presenter.Measure(availableSize);
            return _presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _presenter.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }
    }
}
