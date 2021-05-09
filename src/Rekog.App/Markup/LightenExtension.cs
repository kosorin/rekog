using System;
using System.Windows.Markup;
using System.Windows.Media;
using Rekog.App.Extensions;

namespace Rekog.App.Markup
{
    [MarkupExtensionReturnType(typeof(Color))]
    public class LightenExtension : MarkupExtension
    {
        public Color Color { get; set; }

        public double Amount { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Color.Lighten(Amount);
        }
    }
}
