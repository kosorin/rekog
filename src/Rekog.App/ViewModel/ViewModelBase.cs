using Rekog.App.Model;
using Rekog.App.ObjectModel;
using System.ComponentModel;

namespace Rekog.App.ViewModel
{
    public abstract class ViewModelBase : ObservableObject
    {
    }

    public abstract class ViewModelBase<TModel> : ViewModelBase
        where TModel : ModelBase
    {
        protected ViewModelBase(TModel model)
        {
            Model = model;
            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        public TModel Model { get; }

        protected virtual void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
        }

        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
        }

        private void Model_PropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            OnModelPropertyChanging(sender, args);
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            OnModelPropertyChanged(sender, args);
        }
    }
}
