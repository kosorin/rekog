using System.Windows;
using Rekog.App.ViewModel;

namespace Rekog.App.View
{
    public partial class KeyView
    {
        public static readonly DependencyProperty IsPreviewSelectedProperty =
            DependencyProperty.Register(nameof(IsPreviewSelected), typeof(bool), typeof(KeyView), new PropertyMetadata(false));

        public KeyView()
        {
            InitializeComponent();
        }

        public KeyViewModel ViewModel => (KeyViewModel)DataContext;

        public bool IsPreviewSelected
        {
            get => (bool)GetValue(IsPreviewSelectedProperty);
            set => SetValue(IsPreviewSelectedProperty, value);
        }
    }
}
