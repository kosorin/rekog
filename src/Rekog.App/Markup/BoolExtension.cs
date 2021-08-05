using System;
using System.Windows.Markup;

namespace Rekog.App.Markup
{
    [MarkupExtensionReturnType(typeof(bool))]
    public class BoolExtension : MarkupExtension
    {
        public BoolExtension(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
