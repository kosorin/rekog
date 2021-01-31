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
        protected ViewModelBase()
        {
        }

        protected ViewModelBase(TModel? model)
        {
            Model = model;
        }

        private TModel? _model;
        public TModel? Model
        {
            get => _model;
            set
            {
                var oldModel = Model;
                
                if (Set(ref _model, value))
                {
                    var newModel = Model;

                    if (oldModel != null)
                    {
                        oldModel.PropertyChanged -= Model_PropertyChanged;
                    }
                    if (newModel != null)
                    {
                        newModel.PropertyChanged -= Model_PropertyChanged;
                        newModel.PropertyChanged += Model_PropertyChanged;
                    }

                    OnModelChanged(oldModel, newModel);
                }
            }
        }

        protected virtual void OnModelChanged(TModel? oldModel, TModel? newModel)
        {
        }

        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            OnModelPropertyChanged(sender, args);
        }
    }
}
