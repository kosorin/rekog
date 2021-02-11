using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Rekog.App.Converters
{
    // Source: https://stackoverflow.com/a/8392590/1933104
    // TODO: Pass correct targetType through converters chain
    [ContentProperty(nameof(Converters))]
    [ContentWrapper(typeof(ValueConverterChainItemCollection))]
    public class ValueConverterChain : IValueConverter
    {
        public ValueConverterChainItemCollection Converters { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Converters
                .Aggregate(value, (current, converter) => converter.Converter?.Convert(current, targetType, converter.ConverterParameter, culture));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Converters
                .Reverse()
                .Aggregate(value, (current, converter) => converter.Converter?.ConvertBack(current, targetType, converter.ConverterParameter, culture));
        }
    }

    public sealed class ValueConverterChainItemCollection : Collection<ValueConverterChainItem>
    {
    }

    public class ValueConverterChainItem : DependencyObject
    {
        public IValueConverter? Converter
        {
            get => (IValueConverter?)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register(nameof(Converter), typeof(IValueConverter), typeof(ValueConverterChainItem), new PropertyMetadata(null));


        public object? ConverterParameter
        {
            get => (object?)GetValue(ConverterParameterProperty);
            set => SetValue(ConverterParameterProperty, value);
        }

        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register(nameof(ConverterParameter), typeof(object), typeof(ValueConverterChainItem), new PropertyMetadata(null));
    }
}
