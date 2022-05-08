using System.ComponentModel;
using System.Windows.Input;
using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms.Tabs
{
    public class LayerFormTabViewModel : FormTabViewModel
    {
        private ICommand? _deleteCommand;

        public LayerFormTabViewModel(LayerModel model, string icon, ModelForm form) : base(model.Name, icon, form)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
        }

        public ICommand? DeleteCommand
        {
            get => _deleteCommand;
            set => Set(ref _deleteCommand, value);
        }

        public LayerModel Model { get; }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
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
