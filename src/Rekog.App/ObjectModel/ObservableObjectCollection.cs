using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            foreach (var item in Items)
            {
                Subscribe(item);
            }
        }

        public event PropertyChangedEventHandler? ItemPropertyChanged;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Subscribe(item);
        }

        protected override void SetItem(int index, T item)
        {
            Unsubscribe(Items[index]);
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
            foreach (var item in Items)
            {
                Unsubscribe(item);
            }
            base.ClearItems();
        }

        private void Subscribe(T item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void Unsubscribe(T item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            ItemPropertyChanged?.Invoke(sender, args);
        }
    }
}
