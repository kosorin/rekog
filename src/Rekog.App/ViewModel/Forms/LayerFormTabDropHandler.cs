using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class LayerFormTabDropHandler : IDropTarget
    {
        private readonly BoardModel _model;
        private readonly UndoContext _undoContext;

        public LayerFormTabDropHandler(BoardModel model, UndoContext undoContext)
        {
            _model = model;
            _undoContext = undoContext;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = false;

            var draggedLayers = dropInfo.Data switch
            {
                LayerFormTab tab => new[] { tab.Model, },
                IList<object?> tabs when tabs.All(x => x is LayerFormTab) => tabs
                    .Cast<LayerFormTab>()
                    .Select(x => x.Model)
                    .OrderBy(x => x.Order)
                    .ToArray(),
                _ => null,
            };

            if (draggedLayers == null)
            {
                return;
            }

            var moveAfterTarget = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem);
            if (dropInfo.TargetItem is not LayerFormTab { Model: var targetLayer, })
            {
                var targetOrder = moveAfterTarget ? _model.Layers.Values.Max(x => x.Order) : _model.Layers.Values.Min(x => x.Order);
                targetLayer = _model.Layers.Values.First(x => x.Order == targetOrder);
            }

            var beforeLayers = _model.Layers.Values
                .Where(x => moveAfterTarget ? x.Order <= targetLayer.Order : x.Order < targetLayer.Order)
                .Except(draggedLayers)
                .OrderBy(x => x.Order)
                .ToList();
            var afterLayers = _model.Layers.Values
                .Where(x => moveAfterTarget ? x.Order > targetLayer.Order : x.Order >= targetLayer.Order)
                .Except(draggedLayers)
                .OrderBy(x => x.Order)
                .ToList();

            if (draggedLayers.Length == 1 && draggedLayers.First() == targetLayer)
            {
                return;
            }

            using (_undoContext.Batch())
            {
                beforeLayers
                    .Concat(draggedLayers)
                    .Concat(afterLayers)
                    .Select((x, i) => (order: i, layer: x))
                    .ToList()
                    .ForEach(x => x.layer.Order = x.order);
            }
        }
    }
}
