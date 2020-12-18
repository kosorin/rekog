using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rekog.Kle
{
    public partial class MainWindow : Window
    {
        private record Key(string Value, double X, double Y, double Width, double Height, string BackgroundColor, string ForegroundColor, bool IsHoming);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCanvas.Children.Clear();

            if (string.IsNullOrWhiteSpace(RawData.Text))
            {
                return;
            }

            List<Key> keys;
            try
            {
                keys = ParseRawData(DeserializeRawData(RawData.Text));
            }
            catch
            {
                MessageBox.Show("Raw data parse error!", "Error");
                return;
            }

            var scale = 64d;
            var thickness = 1;
            foreach (var key in keys)
            {
                var text = new TextBlock
                {
                    Text = key.Value,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, thickness, 0),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(key.ForegroundColor)),
                };
                var homingEllipse = new Ellipse
                {
                    Fill = new SolidColorBrush(new Color() { A = 32, R = 0, G = 0, B = 0 }),
                    Margin = new Thickness(0.15 * scale),
                };
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(new Color() { A = 192, R = 0, G = 0, B = 0 }),
                    BorderThickness = new Thickness(thickness),
                };
                var containerGrid = new Grid
                {
                    Width = (key.Width * scale) + thickness,
                    Height = (key.Height * scale) + thickness,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(key.BackgroundColor)),
                    RenderTransformOrigin = new Point(0, 0),
                };
                containerGrid.Children.Add(border);
                if (key.IsHoming)
                {
                    containerGrid.Children.Add(homingEllipse);
                }
                containerGrid.Children.Add(text);
                MainCanvas.Children.Add(containerGrid);
                Canvas.SetLeft(containerGrid, key.X * scale);
                Canvas.SetTop(containerGrid, key.Y * scale);
            }

            MainCanvas.Width = (keys.Max(x => x.X + x.Width) * scale) + thickness;
            MainCanvas.Height = (keys.Max(x => x.Y + x.Height) * scale) + thickness;
            MainCanvas.Margin = new Thickness(0.5 * scale);
        }

        private List<Key> ParseRawData(dynamic rawData)
        {
            var keys = new List<Key>();
            var x = 0d;
            var y = 0d;
            var width = 1d;
            var height = 1d;
            var backgroundColor = "#cccccc";
            var foregroundColor = "#000000";
            var isHoming = false;
            foreach (var row in rawData)
            {
                if (row is not JArray)
                {
                    continue;
                }
                foreach (var item in row)
                {
                    if (item is JObject properties)
                    {
                        JToken token;
                        if (properties.TryGetValue("x", out token))
                        {
                            x += token.Value<double>();
                        }
                        if (properties.TryGetValue("y", out token))
                        {
                            y += token.Value<double>();
                        }
                        if (properties.TryGetValue("w", out token))
                        {
                            width = token.Value<double>();
                        }
                        if (properties.TryGetValue("h", out token))
                        {
                            height = token.Value<double>();
                        }
                        if (properties.TryGetValue("c", out token))
                        {
                            backgroundColor = token.Value<string>();
                        }
                        if (properties.TryGetValue("t", out token))
                        {
                            foregroundColor = token.Value<string>();
                        }
                        if (properties.TryGetValue("n", out token))
                        {
                            isHoming = token.Value<bool>();
                        }
                    }
                    else if (item is JValue value)
                    {
                        keys.Add(new(value.Value<string>(), x, y, width, height, backgroundColor, foregroundColor, isHoming));
                        x += width - 1;
                        width = 1d;
                        height = 1d;
                        isHoming = false;
                        x++;
                    }
                }
                y++;
                x = 0;
            }

            return keys;
        }

        private dynamic DeserializeRawData(string rawDataText)
        {
            dynamic rawData;

            try
            {
                rawData = JsonConvert.DeserializeObject(rawDataText);
            }
            catch
            {
                rawData = JsonConvert.DeserializeObject("[" + rawDataText + "]");
            }

            return rawData;
        }
    }
}
