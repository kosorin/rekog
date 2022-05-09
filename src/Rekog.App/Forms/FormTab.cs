using Rekog.App.ViewModel;

namespace Rekog.App.Forms
{
    public class FormTab : ViewModelBase
    {
        private string _header;
        private string _icon;
        private Form _form;
        private bool _isSelected;

        public FormTab(string header, string icon, Form form)
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

        public Form Form
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
