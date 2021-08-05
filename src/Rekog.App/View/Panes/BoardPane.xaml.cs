using System.Windows;
using System.Windows.Controls;

namespace Rekog.App.View.Panes
{
    public partial class BoardPane
    {
        public BoardPane()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { Tag: string text, })
            {
                if (KleParseRawDataButton.Command.CanExecute(text))
                {
                    KleParseRawDataButton.Command.Execute(text);
                }
            }
        }
    }
}
