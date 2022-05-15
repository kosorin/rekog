using System;
using System.Windows;
using System.Windows.Markup;

namespace Rekog.App.Markup
{
    [MarkupExtensionReturnType(typeof(Point))]
    public class PointExtension : MarkupExtension
    {
        public double X { get; set; }

        public double Y { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Point(X, Y);
        }
    }
}
