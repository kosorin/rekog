using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public abstract class FormViewModel<TModel> : ViewModelBase, IForm<TModel>
        where TModel : ModelBase
    {
        private IReadOnlyCollection<TModel> _models = Array.Empty<TModel>();

        public abstract IReadOnlyCollection<FormProperty<TModel>> Properties { get; }

        public IReadOnlyCollection<TModel> Models
        {
            get => _models;
            private set
            {
                foreach (var model in Models)
                {
                    model.PropertyChanged -= ModelOnPropertyChanged;
                }

                if (Set(ref _models, value))
                {
                    foreach (var model in Models)
                    {
                        model.PropertyChanged -= ModelOnPropertyChanged;
                        model.PropertyChanged += ModelOnPropertyChanged;
                    }

                    UpdateProperties();
                    OnPropertyChanged(nameof(IsSet));
                }
            }
        }

        public bool IsSet => Models.Any();

        public void Set(TModel model)
        {
            Set(new[] { model, });
        }

        public void Set(IEnumerable<TModel> models)
        {
            Models = models.ToArray();
        }

        public void Clear()
        {
            Models = Array.Empty<TModel>();
        }

        private void UpdateProperties()
        {
            foreach (var property in Properties)
            {
                property.Update();
            }
        }

        private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            foreach (var property in Properties.Where(x => args.PropertyName == null || args.PropertyName == x.Name))
            {
                property.Update();
            }
        }
    }
}
