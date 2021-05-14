using System.Windows.Controls;

namespace Rekog.App.View
{
    public partial class MainWindow
    {
        private bool _changingKleInput;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (KleRawDataList == null)
            {
                return;
            }

            if (_changingKleInput)
            {
                return;
            }

            try
            {
                _changingKleInput = true;

                KleRawDataList.SelectedIndex = 0;
            }
            finally
            {
                _changingKleInput = false;
            }
        }

        private void KleRawDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KleInput == null)
            {
                return;
            }

            if (_changingKleInput)
            {
                return;
            }

            try
            {
                _changingKleInput = true;

                KleInput.Text = KleRawDataList.SelectedItem is ComboBoxItem { Tag: string text, }
                    ? text
                    : null;
                if (KleParseRawDataButton.Command.CanExecute(KleParseRawDataButton.CommandParameter))
                {
                    KleParseRawDataButton.Command.Execute(KleParseRawDataButton.CommandParameter);
                }
            }
            finally
            {
                _changingKleInput = false;
            }
        }
    }
}
