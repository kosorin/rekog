using System.ComponentModel;
using System.Windows.Input;
using Rekog.App.Forms;
using Rekog.App.Model;

namespace Rekog.App.ViewModel.Forms
{
    public class LayerFormTab : FormTab
    {
        private ICommand? _deleteCommand;

        public LayerFormTab(LayerModel model, string icon, Form form) : base(model.Name, icon, form)
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
