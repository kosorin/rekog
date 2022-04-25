namespace Rekog.App.ViewModel.Forms.Tabs
{
    public class FormTabViewModel : ViewModelBase
    {
        private string _header;
        private string _icon;
        private ViewModelBase _form;
        private bool _isSelected;

        public FormTabViewModel(string header, string icon, ViewModelBase form)
        {
            _header = header;
            _icon = icon;
            _form = form;
        }

        public string Header
        {
            get => _header;
            set => Set(ref _header, value);
        }

        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        public ViewModelBase Form
        {
            get => _form;
            set => Set(ref _form, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
