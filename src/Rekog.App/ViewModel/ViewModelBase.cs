using System.ComponentModel;
using Rekog.App.Model;
using Rekog.App.ObjectModel;

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
            Model.PropertyChanging += OnModelPropertyChanging;
            Model.PropertyChanged += OnModelPropertyChanged;
        }

        public TModel Model { get; }

        protected virtual void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
        }

        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
        }
    }
}
