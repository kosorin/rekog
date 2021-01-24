using Rekog.App.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rekog.App.Views
{
    public partial class BoardView : UserControl
    {
        public BoardView()
        {
            InitializeComponent();

            DataContextChanged += UserControl_DataContextChanged;
        }

        public BoardViewModel? Board { get; private set; }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Board = DataContext as BoardViewModel;
        }

        private void Root_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Board == null)
            {
                return;
            }

            Board.Scale += Math.Sign(e.Delta);
        }
    }
}
