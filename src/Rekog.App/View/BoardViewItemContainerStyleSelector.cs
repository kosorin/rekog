using System;
using System.Windows;
using System.Windows.Controls;
using Rekog.App.ViewModel;

namespace Rekog.App.View
{
    public class BoardViewItemContainerStyleSelector : StyleSelector
    {
        public Style? KeyStyle { get; set; }

        public Style? RotationOriginStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return item switch
            {
                KeyViewModel => KeyStyle ?? throw new NullReferenceException(),
                RotationOriginViewModel => RotationOriginStyle ?? throw new NullReferenceException(),
                _ => throw new ArgumentException(null, nameof(item)),
            };
        }
    }
}
