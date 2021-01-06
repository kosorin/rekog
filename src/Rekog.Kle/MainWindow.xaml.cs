using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Rekog.Kle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RawData.Text = @"
[{p:""FLAT"",a:7,f:9,n:true},""A"",{c:""#ffffff"",t:""#0ca800"",f:1},""B"",{c:""#cccccc"",t:""#000000"",f:3,fa:[1]},""C"",{f:3,n:true},""D"",{t:""#000000\n\n\n#d91c1c\n\n\n\n\n#201cd9"",a:4,fa:[1,0,0,9,0,0,0,0,4]},""\n\n\ne\n\n\n\n\ně\nE"",{t:""#000000"",g:true,a:3,f:3,n:true},""FX\n\n\n\nuxx2""],
[{t:""#000000\n\n\n#aa07f5\n\n#a824d6\n#ffd600\n\n\n\n\n#a624d4"",g:false,a:0,f:7,w:1.5},""0\n1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11""],
[{y:-0.75,x:4.75,t:""#000000"",p:"""",f:3,w:1.25,w2:1.75,x2:-0.25,l:true},""+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+\n+""],
[{y:-0.5,x:2,p:""FLAT"",a:5,d:true},""WUT""],
[{y:-0.75,a:0,f:6,n:true},""A\nB\nC\nD\n\nok""],
[{y:-0.5,x:7.25,p:""CHICKLET"",a:7,f:3,w:1.25,w2:1.75,l:true},""""],
[{x:0.5,p:"""",h:0.5},""""],
[{rx:2.5,ry:2.75,y:-0.25,c:""#a5a5a5"",a:0,w:1.5,h:2,w2:2.25,h2:1,x2:-0.5,y2:0.75,n:true,l:true},""+\n+\n+\n+\n+\n+\n+\n+\n+\n@""],
[{r:15,rx:5,ry:3,y:-0.5,c:""#cccccc"",a:7,h:1.5},"""",""""],
[{x:1,w:1.25},""""],
[{r:-15,rx:8,ry:2,y:-1.5,x:0.75,c:""#a5a5a5"",p:""CHICKLET"",a:0,w:2.25,h:1.25,w2:1.5,h2:2.5,x2:0.25,y2:-0.5},""+\n+\n+\n+\n+\n+\n+\n+\n+\n@\n+\n+""]
";
            CommandBinding_Executed(null!, null!);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCanvas.Children.Clear();

            if (string.IsNullOrWhiteSpace(RawData.Text))
            {
                return;
            }

            List<KleKey> keys;
            try
            {
                keys = new KleParser().ParseRawData(RawData.Text);
            }
            catch
            {
                keys = new List<KleKey>();
            }

            var keySize = 48d;
            var keyBorderSize = 1d;
            double Translate(double value) => value * keySize;

            var allPoints = new List<(double x, double y)>();

            foreach (var key in keys)
            {
                var points = key.GetPolygon();
                allPoints.AddRange(points.Select(point =>
                {
                    var x = point.x + key.X;
                    var y = point.y + key.Y;

                    var dx = x - key.RotationX;
                    var dy = y - key.RotationY;
                    var p = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                    var angle = key.RotationAngle * (Math.PI / 180);
                    if (p != 0)
                    {
                        angle += Math.Asin(dy / p);
                    }

                    var bx = p * Math.Cos(angle);
                    var by = p * Math.Sin(angle);

                    return (x: bx + key.RotationX, y: by + key.RotationY);
                }));

                var offset = (x: Math.Min(0, key.X2), y: Math.Min(0, key.Y2));
                var width = points.Max(p => p.x) - points.Min(p => p.x);
                var height = points.Max(p => p.y) - points.Min(p => p.y);

                var container = new Grid
                {
                    Width = Translate(width) + keyBorderSize,
                    Height = Translate(height) + keyBorderSize,
                    RenderTransformOrigin = new Point(
                        Translate(key.RotationX - (key.X + offset.x)) / (Translate(width) + keyBorderSize),
                        Translate(key.RotationY - (key.Y + offset.y)) / (Translate(height) + keyBorderSize)),
                    RenderTransform = new RotateTransform(key.RotationAngle),
                };

                if (!key.IsDecal)
                {
                    var radius = 4d;
                    var backgroundColor = ParseColor(key.BackgroundColor, Colors.White);

                    var polygon = new RoundedCornersPolygon
                    {
                        ArcRoundness = radius,
                        Fill = new SolidColorBrush(backgroundColor),
                        Stroke = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                        StrokeThickness = keyBorderSize,
                        IsClosed = true,
                        Opacity = key.IsGhosted ? 0.4 : 1,
                    };
                    foreach (var (x, y) in points)
                    {
                        polygon.Points.Add(new Point(Translate(x - offset.x) - keyBorderSize, Translate(y - offset.y) - keyBorderSize));
                    }
                    container.Children.Add(polygon);

                    if (!key.IsGhosted)
                    {
                        var innerPoints = key.GetInnerPolygon();
                        var innerPolygon = new RoundedCornersPolygon
                        {
                            ArcRoundness = radius,
                            Fill = new SolidColorBrush(Color.Multiply(backgroundColor, 1.5f)),
                            Stroke = new SolidColorBrush(Color.Multiply(backgroundColor, 0.75f)),
                            StrokeThickness = 1,
                            IsClosed = true,
                        };
                        foreach (var (x, y) in innerPoints)
                        {
                            innerPolygon.Points.Add(new Point(Translate(x - offset.x) - keyBorderSize, Translate(y - offset.y) - keyBorderSize));
                        }
                        container.Children.Add(innerPolygon);
                    }
                }

                if (!key.IsGhosted)
                {
                    if (key.IsHoming && !key.IsDecal)
                    {
                        var homingEllipse = new Line
                        {
                            Stroke = new SolidColorBrush(Color.FromArgb(96, 0, 0, 0)),
                            StrokeThickness = 1.5,
                            X1 = Translate(-offset.x + key.Width * 0.5) - 4.5,
                            X2 = Translate(-offset.x + key.Width * 0.5) + 4.5 - 2 * keyBorderSize,
                            Y1 = Translate(-offset.y + key.Height) - keySize * 0.275,
                            Y2 = Translate(-offset.y + key.Height) - keySize * 0.275,
                        };
                        container.Children.Add(homingEllipse);
                    }

                    for (var index = 0; index < 9; index++)
                    {
                        if (key.Labels[index] == null)
                        {
                            continue;
                        }
                        var text = new TextBlock
                        {
                            Text = key.Labels[index],
                            HorizontalAlignment = (index % 3) switch
                            {
                                0 => HorizontalAlignment.Left,
                                2 => HorizontalAlignment.Right,
                                _ => HorizontalAlignment.Center,
                            },
                            VerticalAlignment = (index / 3) switch
                            {
                                0 => VerticalAlignment.Top,
                                2 => VerticalAlignment.Bottom,
                                _ => VerticalAlignment.Center,
                            },
                            Margin = new Thickness(
                                -Translate(offset.x) + 4.5 * keyBorderSize,
                                -Translate(offset.y) + 2 * keyBorderSize,
                                Translate(width) - Translate(key.Width - offset.x) + 7.5 * keyBorderSize,
                                Translate(height) - Translate(key.Height - offset.y) + 8.5 * keyBorderSize),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = new SolidColorBrush(ParseColor(key.TextColors[index] ?? key.DefaultTextColor, Colors.Black)),
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 4 + (key.TextSizes[index] ?? key.DefaultTextSize) * 2,
                        };
                        container.Children.Add(text);
                    }
                }

                Canvas.SetLeft(container, Translate(key.X + offset.x));
                Canvas.SetTop(container, Translate(key.Y + offset.y));
                MainCanvas.Children.Add(container);

                container.MouseEnter += (_, _) => { container.Opacity = 0.8; };
                container.MouseLeave += (_, _) => { container.Opacity = 1; };
            }

            MainCanvas.Width = keys.Count > 0 ? Translate(allPoints.Max(p => p.x)) : Translate(0.5);
            MainCanvas.Height = keys.Count > 0 ? Translate(allPoints.Max(p => p.y)) : Translate(0.5);
            MainCanvas.Margin = new Thickness(Translate(0.25));
        }

        private static Color ParseColor(string color, Color fallbackColor)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(color);
            }
            catch
            {
                return fallbackColor;
            }
        }
    }
}
