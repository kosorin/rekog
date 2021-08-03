using System.Collections.Generic;
using System.Linq;
using Rekog.App.Model;

namespace Rekog.App.ViewModel.Forms
{
    public abstract class FormViewModel : ViewModelBase, IForm
    {
        private bool _isSet;

        protected FormViewModel(ICollection<ModelBase> models)
        {
            IsSet = models.Any();
        }

        public bool IsSet
        {
            get => _isSet;
            private set => Set(ref _isSet, value);
        }
    }
}
