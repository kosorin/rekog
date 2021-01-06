using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rekog.Kle
{
    public class KleKey
    {
        public string?[] Labels { get; set; } = Array.Empty<string?>();


        public double RotationAngle { get; set; }

        public double RotationX { get; set; }

        public double RotationY { get; set; }


        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        public double Width2 { get; set; }

        public double Height2 { get; set; }


        public string BackgroundColor { get; set; } = string.Empty;

        public string DefaultTextColor { get; set; } = string.Empty;

        public int DefaultTextSize { get; set; }

        public string?[] TextColors { get; set; } = Array.Empty<string?>();

        public int?[] TextSizes { get; set; } = Array.Empty<int?>();


        public bool IsHoming { get; set; }

        public bool IsStepped { get; set; }

        public bool IsDecal { get; set; }

        public bool IsGhosted { get; set; }


        public (double x, double y)[] GetInnerPolygon()
        {
            var paddingValue = 0.1;
            var padding = new Thickness(0.04, 0.04, 0.04, 0.1);
            if (IsStepped)
            {
                var rectangle = new Rectangle(
                    new(0, 0),
                    new(0 + Width, 0 + Height),
                    padding);
                return new[]
                {
                    (rectangle.TopLeft.X, rectangle.TopLeft.Y),
                    (rectangle.TopRight.X, rectangle.TopRight.Y),
                    (rectangle.BottomRight.X, rectangle.BottomRight.Y),
                    (rectangle.BottomLeft.X, rectangle.BottomLeft.Y),
                };
            }
            else
            {
                return GetPolygon(padding);
            }
        }

        public (double x, double y)[] GetPolygon()
        {
            var paddingValue = 0.02;
            return GetPolygon(new(paddingValue, paddingValue, paddingValue, paddingValue));
        }

        private (double x, double y)[] GetPolygon(Thickness padding)
        {
            var firstRectangle = new Rectangle(
                new(0, 0),
                new(0 + Width, 0 + Height),
                padding);
            if (X2 == 0 && Y2 == 0 && Width == Width2 && Height == Height2)
            {
                return new[]
                {
                    (firstRectangle.TopLeft.X, firstRectangle.TopLeft.Y),
                    (firstRectangle.TopRight.X, firstRectangle.TopRight.Y),
                    (firstRectangle.BottomRight.X, firstRectangle.BottomRight.Y),
                    (firstRectangle.BottomLeft.X, firstRectangle.BottomLeft.Y),
                };
            }

            var secondRectangle = new Rectangle(
                new(0 + X2, 0 + Y2),
                new(0 + X2 + Width2, 0 + Y2 + Height2),
                padding);

            var horizontalLines = new Queue<Line>(new Line[]
            {
                new(firstRectangle.TopLeft, firstRectangle.TopRight),
                new(firstRectangle.BottomLeft, firstRectangle.BottomRight),
                new(secondRectangle.TopLeft, secondRectangle.TopRight),
                new(secondRectangle.BottomLeft, secondRectangle.BottomRight),
            });
            var verticalLines = new List<Line>
            {
                new(firstRectangle.TopLeft, firstRectangle.BottomLeft),
                new(firstRectangle.TopRight, firstRectangle.BottomRight),
                new(secondRectangle.TopLeft, secondRectangle.BottomLeft),
                new(secondRectangle.TopRight, secondRectangle.BottomRight),
            };

            var lines = new List<Line>();
            while (horizontalLines.TryDequeue(out var horizontalLine) && verticalLines.Any())
            {
                var split = false;
                for (var i = 0; i < verticalLines.Count; i++)
                {
                    var verticalLine = verticalLines[i];
                    if (true
                        && horizontalLine.First.X < verticalLine.First.X
                        && horizontalLine.Second.X > verticalLine.First.X
                        && verticalLine.First.Y < horizontalLine.First.Y
                        && verticalLine.Second.Y > horizontalLine.First.Y)
                    {
                        var intersection = new Point(verticalLine.First.X, horizontalLine.First.Y);

                        horizontalLines.Enqueue(new(horizontalLine.First, intersection));
                        horizontalLines.Enqueue(new(intersection, horizontalLine.Second));

                        verticalLines.RemoveAt(i);
                        verticalLines.Add(new(verticalLine.First, intersection));
                        verticalLines.Add(new(intersection, verticalLine.Second));

                        split = true;
                        break;
                    }
                }

                if (!split)
                {
                    lines.Add(horizontalLine);
                }
            }
            lines.AddRange(horizontalLines);
            lines.AddRange(verticalLines);
            lines = lines.Distinct().ToList();
            lines.RemoveAll(x => firstRectangle.Contains(x) && secondRectangle.Contains(x));

            var useA = false;
            var currentLine = lines.First();
            var points = new List<Point>();
            points.Add(currentLine.First);
            do
            {
                lines.Remove(currentLine);

                var point = useA ? currentLine.First : currentLine.Second;
                points.Add(point);

                currentLine = null;
                foreach (var line in lines)
                {
                    if (point == line.First)
                    {
                        useA = false;
                        currentLine = line;
                        break;
                    }
                    if (point == line.Second)
                    {
                        useA = true;
                        currentLine = line;
                        break;
                    }
                }
            } while (currentLine != null);

            if (points.Last() == points.First())
            {
                points.Remove(points.Last());
            }

            return points.Select(x => (x.X, x.Y)).ToArray();
        }


        private record Thickness(double Left, double Top, double Right, double Bottom);

        /// <summary>
        /// [X,Y]
        /// </summary>
        private record Point(double X, double Y);

        /// <summary>
        /// A ---- B
        /// </summary>
        private record Line(Point First, Point Second)
        {
            public Point? Intersect(Line line)
            {
                return null;
            }
        }

        private class Rectangle
        {
            public Rectangle(Point topLeft, Point bottomRight)
                : this(topLeft, bottomRight, new(0, 0, 0, 0))
            {
            }

            public Rectangle(Point topLeft, Point bottomRight, Thickness padding)
            {
                TopLeft = new Point(topLeft.X + padding.Left, topLeft.Y + padding.Top);
                BottomRight = new Point(bottomRight.X - padding.Right, bottomRight.Y - padding.Bottom);
                TopRight = new Point(BottomRight.X, TopLeft.Y);
                BottomLeft = new Point(TopLeft.X, BottomRight.Y);
            }

            public Point TopLeft { get; }

            public Point TopRight { get; }

            public Point BottomLeft { get; }

            public Point BottomRight { get; }

            public bool Contains(Line line)
            {
                if (line.First.X == line.Second.X)
                {
                    return line.First.X >= TopLeft.X && line.First.X <= BottomRight.X
                        && line.First.Y >= TopLeft.Y && line.Second.Y <= BottomRight.Y;
                }
                else
                {
                    return line.First.Y >= TopLeft.Y && line.First.Y <= BottomRight.Y
                        && line.First.X >= TopLeft.X && line.Second.X <= BottomRight.X;
                }
            }
        }
    }
}
