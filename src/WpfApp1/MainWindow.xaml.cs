using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            MainCanvas.PreviewMouseMove += MainCanvas_PreviewMouseMove;

            AddKey(new KleKey
            {
                X = 50,
                X2 = 30,
                Y = 50,
                Y2 = -30,

                Width = 200,
                Width2 = 100,
                Height = 100,
                Height2 = 200,

                IsStepped = true,
            });

            AddKey(new KleKey
            {
                X = 50,
                X2 = 0,
                Y = 300,
                Y2 = 0,

                Width = 150,
                Width2 = 225,
                Height = 100,
                Height2 = 100,

                IsStepped = true,
            });

            AddKey(new KleKey
            {
                X = 50,
                X2 = 40,
                Y = 500,
                Y2 = 50,

                Width = 50,
                Width2 = 50,
                Height = 50,
                Height2 = 50,

                IsStepped = true,
            });

            var p = new Geometry.Polygon(new List<Geometry.Point> {
                new Geometry.Point(300, 50),
                new Geometry.Point(350, 75),
                new Geometry.Point(400, 50),
                new Geometry.Point(400, 150),
                new Geometry.Point(320, 100),
            });
            var q = new Geometry.Polygon(new List<Geometry.Point> {
                new Geometry.Point(280, 30),
                new Geometry.Point(420, 30),
                new Geometry.Point(420, 65),
                new Geometry.Point(270, 35),
            });
            //DrawPolygon(p);
            //DrawPolygon(q);


            var ug = p.Union(q);
            foreach (var u in ug)
            {
                DrawPolygon(u, Brushes.Blue);
            }
        }

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(MainCanvas);
            Canvas.SetLeft(CoordBorder, position.X + SystemParameters.CursorWidth);
            Canvas.SetTop(CoordBorder, position.Y + SystemParameters.CursorHeight);

            Coord.Text = $"{position.X:N0} ; {position.Y:N0}";

            CoordBorder.Visibility = Visibility.Visible;
        }

        private void AddKey(KleKey key)
        {
            DrawPolygon(key.GetPrimaryPolygon(), Brushes.Lime, Brushes.Green);
            DrawPolygon(key.GetSecondaryPolygon(), Brushes.Pink, Brushes.Red);
        }

        private void DrawPolygon(Geometry.Polygon polygon, Brush? fill = null, Brush? stroke = null, double strokeThickness = 2, double opacity = 0.5)
        {
            var primary = new KeyControl
            {
                Fill = fill ?? Brushes.Gray,
                Stroke = stroke ?? Brushes.Black,
                StrokeThickness = strokeThickness,
                Opacity = opacity,
            };
            primary.Shape = new PointCollection(polygon.Vertices.Select(vertex => new Point(vertex.X, vertex.Y)));
            //foreach (var (x, y) in polygon.Vertices)
            //{
            //    primary.Shape.Add(new Point(x, y));
            //}
            MainCanvas.Children.Add(primary);
        }
    }
}
