using System;
using System.Collections.Generic;
using System.Linq;
using Rekog.App.ObjectModel;
using Rekog.App.Undo.Actions;

namespace Rekog.App.Undo
{
    public class UndoContext : ObservableObject
    {
        private readonly Stack<UndoBatch> _undoStack = new Stack<UndoBatch>();
        private readonly Stack<UndoBatch> _redoStack = new Stack<UndoBatch>();
        private readonly List<IUndoAction> _batchActions = new List<IUndoAction>();
        private int _batchDepth;
        private bool _isProcessing;

        public UndoContext()
        {
            UndoCommand = new DelegateCommand(Undo, CanUndo);
            RedoCommand = new DelegateCommand(Redo, CanRedo);
        }

        public DelegateCommand UndoCommand { get; }

        public DelegateCommand RedoCommand { get; }

        public IDisposable Batch()
        {
            return new BatchScope(this);
        }

        public void BeginBatch()
        {
            _batchDepth++;

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void EndBatch()
        {
            _batchDepth--;

            if (_batchDepth == 0 && _batchActions.Count > 0)
            {
                PushBatch(_batchActions);
                _batchActions.Clear();
            }

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void PushBatch(IEnumerable<IUndoAction> actions)
        {
            if (_isProcessing || _batchDepth > 0)
            {
                return;
            }

            _redoStack.Clear();
            RedoCommand.RaiseCanExecuteChanged();

            _undoStack.Push(new UndoBatch(actions));
            UndoCommand.RaiseCanExecuteChanged();
        }

        public void PushAction(IUndoAction action)
        {
            if (_isProcessing)
            {
                return;
            }

            if (_batchDepth > 0)
            {
                _batchActions.Add(action);
            }
            else
            {
                PushBatch(new[] { action, });
            }
        }

        public void PushAction(Action? undo, Action? redo)
        {
            PushAction(new DelegateUndoAction(undo, redo));
        }

        private void Undo()
        {
            if (!CanUndo())
            {
                return;
            }

            _isProcessing = true;
            try
            {
                if (_undoStack.TryPop(out var batch))
                {
                    batch.Undo();

                    _redoStack.Push(batch);
                    RedoCommand.RaiseCanExecuteChanged();
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void Redo()
        {
            if (!CanRedo())
            {
                return;
            }

            _isProcessing = true;
            try
            {
                if (_redoStack.TryPop(out var batch))
                {
                    batch.Redo();

                    _undoStack.Push(batch);
                    UndoCommand.RaiseCanExecuteChanged();
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private bool CanUndo()
        {
            return !_isProcessing && _batchDepth == 0 && _undoStack.Any();
        }

        private bool CanRedo()
        {
            return !_isProcessing && _batchDepth == 0 && _redoStack.Any();
        }

        private class BatchScope : IDisposable
        {
            private readonly UndoContext _context;

            public BatchScope(UndoContext context)
            {
                _context = context;
                _context.BeginBatch();
            }

            public void Dispose()
            {
                _context.EndBatch();
            }
        }
    }
}
