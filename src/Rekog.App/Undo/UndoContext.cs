using System;
using System.Collections.Generic;
using System.Linq;
using Rekog.App.ObjectModel;
using Rekog.App.Undo.Actions;

namespace Rekog.App.Undo
{
    public class UndoContext : ObservableObject
    {
        private readonly Stack<IUndoBatch> _undoStack = new Stack<IUndoBatch>();
        private readonly Stack<IUndoBatch> _redoStack = new Stack<IUndoBatch>();
        private readonly UndoBatchBuilder _defaultBatchBuilder = new UndoBatchBuilder();
        private IUndoBatchBuilder? _batchBuilder;
        private IUndoBatch? _lastBatch;
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
            return Batch(_defaultBatchBuilder);
        }

        public void ClearCoalescing()
        {
            _lastBatch = null;
        }

        public IDisposable Batch(IUndoBatchBuilder batchBuilder)
        {
            return new BatchScope(this, batchBuilder);
        }

        public void BeginBatch()
        {
            BeginBatch(_defaultBatchBuilder);
        }

        public void BeginBatch(IUndoBatchBuilder batchBuilder)
        {
            if (_batchBuilder != null)
            {
                throw new InvalidOperationException("Nested batches are not supported.");
            }

            _batchBuilder = batchBuilder;
            _batchBuilder.Initialize();

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void EndBatch()
        {
            if (_batchBuilder == null)
            {
                throw new InvalidOperationException();
            }

            if (_lastBatch != null && _batchBuilder.TryCoalesce(_lastBatch))
            {
                _batchBuilder = null;
            }
            else
            {
                var batch = _batchBuilder.Build();

                _batchBuilder = null;

                if (batch != null)
                {
                    PushBatch(batch);
                }
            }

            RedoCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
        }

        public void PushBatch(IUndoBatch batch)
        {
            if (_isProcessing)
            {
                return;
            }

            if (_batchBuilder != null)
            {
                throw new InvalidOperationException("Nested batches are not supported.");
            }

            _lastBatch = batch;

            _redoStack.Clear();
            _undoStack.Push(batch);

            RedoCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
        }

        public void PushAction(IUndoAction action)
        {
            if (_isProcessing)
            {
                return;
            }

            if (_batchBuilder != null)
            {
                _batchBuilder.PushAction(action);
            }
            else
            {
                PushBatch(new UndoBatch(new[] { action, }));
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
                    ClearCoalescing();

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
                    ClearCoalescing();

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
            return !_isProcessing && _batchBuilder == null && _undoStack.Any();
        }

        private bool CanRedo()
        {
            return !_isProcessing && _batchBuilder == null && _redoStack.Any();
        }

        private class BatchScope : IDisposable
        {
            private readonly UndoContext _context;

            public BatchScope(UndoContext context, IUndoBatchBuilder batchBuilder)
            {
                _context = context;
                _context.BeginBatch(batchBuilder);
            }

            public void Dispose()
            {
                _context.EndBatch();
            }
        }
    }
}
