using System;
using System.Windows.Markup;

namespace Rekog.App.Markup
{
    [MarkupExtensionReturnType(typeof(double))]
    public class DoubleExtension : MarkupExtension
    {
        public DoubleExtension(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
