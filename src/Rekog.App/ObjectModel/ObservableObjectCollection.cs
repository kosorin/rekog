using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Rekog.App.ObjectModel
{
    public class ObservableObjectCollection<T> : ObservableCollection<T>
        where T : ObservableObject
    {
        public ObservableObjectCollection()
        {
        }

        public ObservableObjectCollection(IEnumerable<T> collection) : base(collection)
        {
            Subscribe(Items);
        }

        public event PropertyChangedEventHandler? ItemPropertyChanged;

        public event CollectionItemChangedEventHandler? CollectionItemChanged;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Subscribe(item);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            if (ReferenceEquals(oldItem, item))
            {
                return;
            }

            Unsubscribe(oldItem);
            base.SetItem(index, item);
            Subscribe(item);
        }

        protected override void RemoveItem(int index)
        {
            Unsubscribe(Items[index]);
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            Unsubscribe(Items);
            base.ClearItems();
        }

        protected virtual void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            ItemPropertyChanged?.Invoke(sender, args);
        }

        protected virtual void OnCollectionItemChanged(CollectionItemChangedEventArgs args)
        {
            CollectionItemChanged?.Invoke(this, args);
        }

        private void Subscribe(T item)
        {
            Subscribe(new[] { item });
        }

        private void Unsubscribe(T item)
        {
            Unsubscribe(new[] { item });
        }

        private void Subscribe(IEnumerable<T> items)
        {
            OnCollectionItemChanged(new CollectionItemChangedEventArgs(new List<T>(), items.ToList()));
            foreach (var item in items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Unsubscribe(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            OnCollectionItemChanged(new CollectionItemChangedEventArgs(items.ToList(), new List<T>()));
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            OnItemPropertyChanged(sender, args);
        }
    }
}
