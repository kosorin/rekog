using System;
using System.Collections.Generic;
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
                if (Set(ref _models, value))
                {
                    Update();
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

        public void Update()
        {
            foreach (var property in Properties)
            {
                property.Update();
            }
        }
    }
}
