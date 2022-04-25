using System.ComponentModel;
using Rekog.App.Model;

namespace Rekog.App.ViewModel.Forms.Tabs
{
    public class LayerFormTabViewModel : FormTabViewModel
    {
        public LayerFormTabViewModel(LayerModel model, string icon, ViewModelBase form) : base(model.Name, icon, form)
        {
            Model = model;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        public LayerModel Model { get; }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(LayerModel.Name):
                    Header = Model.Name;
                    break;
            }
        }
    }
}
