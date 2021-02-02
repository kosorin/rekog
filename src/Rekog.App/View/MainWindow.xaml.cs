using System.Windows;
using System.Windows.Controls;

namespace Rekog.App.View
{
    public partial class MainWindow : Window
    {
        private bool _changingKleRawData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KleRawData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (KleRawDataList == null)
            {
                return;
            }

            if (_changingKleRawData)
            {
                return;
            }

            try
            {
                _changingKleRawData = true;

                KleRawDataList.SelectedIndex = 0;
            }
            finally
            {
                _changingKleRawData = false;
            }
        }

        private void KleRawDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KleRawData == null)
            {
                return;
            }

            if (_changingKleRawData)
            {
                return;
            }

            try
            {
                _changingKleRawData = true;

                KleRawData.Text = KleRawDataList.SelectedItem is ComboBoxItem item && item.Tag is string text
                    ? text
                    : null;
                if (KleParseButton.Command.CanExecute(KleParseButton.CommandParameter))
                {
                    KleParseButton.Command.Execute(KleParseButton.CommandParameter);
                }
            }
            finally
            {
                _changingKleRawData = false;
            }
        }
    }
}
