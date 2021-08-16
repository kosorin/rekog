using System;
using System.Windows;
using System.Windows.Controls;
using Rekog.App.ViewModel;
using Rekog.App.ViewModel.Values;

namespace Rekog.App.View
{
    public class BoardViewItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? KeyTemplate { get; set; }

        public DataTemplate? RotationOriginTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                KeyViewModel => KeyTemplate ?? throw new NullReferenceException(),
                PointValueSource { Key: nameof(BoardViewModel.SelectedKeysRotationOrigin), } => RotationOriginTemplate ?? throw new NullReferenceException(),
                _ => throw new ArgumentException(null, nameof(item)),
            };
        }
    }
}
