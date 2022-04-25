using System.ComponentModel;
using System.Windows.Input;
using Rekog.App.Model;

namespace Rekog.App.ViewModel.Forms.Tabs
{
    public class LayerFormTabViewModel : FormTabViewModel
    {
        private ICommand? _deleteCommand;

        public LayerFormTabViewModel(LayerModel model, string icon, ViewModelBase form) : base(model.Name, icon, form)
        {
            Model = model;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        public ICommand? DeleteCommand
        {
            get => _deleteCommand;
            set => Set(ref _deleteCommand, value);
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
