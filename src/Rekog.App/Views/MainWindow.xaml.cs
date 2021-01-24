using Rekog.App.ViewModel;
using Rekog.App.Model.Kle;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Rekog.App.Model;
using System;

namespace Rekog.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            KleRawData.Text = @"
[{p:""FLAT"",a:7,f:9,n:true},""A"",{c:""#ffffff"",t:""#0ca800"",f:1},""B"",{c:""#cccccc"",t:""#000000"",f:3,fa:[1]},""C"",{f:3,n:true},""D"",{t:""#000000\n\n\n#d91c1c\n\n\n\n\n#201cd9"",a:4,fa:[1,0,0,9,0,0,0,0,4]},""\n\n\ne\n\n\n\n\ně\nE"",{t:""#000000"",g:true,a:3,f:3,n:true},""FX\n\n\n\nuxx2""],
[{t:""#000000\n\n\n#aa07f5\n\n#a824d6\n#ffd600\n\n\n\n\n#a624d4"",g:false,a:0,f:7,w:1.5},""0\n1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11""],
[{y:-0.75,x:4.75,t:""#000000"",f:3,w:1.25,w2:1.75,x2:-0.25,l:true},""+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+""],
[{y:-0.5,x:2,a:5,d:true},""WUT""],
[{y:-0.75,a:0,f:6,n:true},""A\nB\nC\nD\n\nok""],
[{y:-0.5,x:7.25,p:""CHICKLET"",a:7,f:3,w:1.25,w2:1.75,l:true},""""],
[{r:15,h:0.5},""""],
[{rx:5,ry:3,y:-0.5,h:1.5},"""",""""],
[{x:1,w:1.25},""""],
[{r:-15,rx:1,ry:2,y:0.5,x:1.5,c:""#a5a5a5"",a:0,w:1.5,h:2,w2:2.25,h2:1,x2:-0.5,y2:0.75,n:true,l:true},""+\n+\n+\n+\n+\n+\n+\n+\n+\n@""],
[{rx:8,y:-1.5,x:0.75,w:2.25,h:1.25,w2:1.5,h2:2.5,x2:0.25,y2:-0.5},""+\n+\n+\n+\n+\n+\n+\n+\n+\n@\n+\n+""]
";
            CommandBinding_Executed(null!, null!);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var kleKeys = ParseKleRawData(KleRawData.Text);
            var keys = new KeyViewModelCollection(kleKeys.Select(x => new KeyViewModel { Model = KeyModel.FromKle(x) }));
            var board = new BoardViewModel
            {
                Keys = keys,
            };

            BoardView.DataContext = board;
        }

        private void BoardContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (BoardView.DataContext is BoardViewModel board)
            {
                board.Scale *= 1 + (Math.Sign(e.Delta) * 0.2);
            }
        }

        private static List<KleKey> ParseKleRawData(string kleRawData)
        {
            if (string.IsNullOrWhiteSpace(kleRawData))
            {
                return new List<KleKey>();
            }
            else
            {
                try
                {
                    return KleParser.ParseRawData(kleRawData);
                }
                catch
                {
                    return new List<KleKey>();
                }
            }
        }
    }
}
