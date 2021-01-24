using System;

namespace Rekog.App.Geometry
{
    public record LineSegment
    {
        public LineSegment(Point first, Point second)
        {
            First = first;
            Second = second;
        }

        public Point First { get; }

        public Point Second { get; }

        public void Deconstruct(out Point first, out Point second)
        {
            first = First;
            second = Second;
        }

        public override string ToString()
        {
            return $"({First}<=>{Second})";
        }

        public Point[] GetIntersections(LineSegment lineSegment)
        {
            if (!IntersectHelper.Intersect(this, lineSegment))
            {
                return Array.Empty<Point>();
            }

            return IntersectHelper.GetIntersections(this, lineSegment);
        }

        private class IntersectHelper
        {
            public static bool Intersect(LineSegment lineSegment1, LineSegment lineSegment2)
            {
                return IntersectBoundingBox(lineSegment1, lineSegment2)
                    && TouchOrIntersect(lineSegment1, lineSegment2)
                    && TouchOrIntersect(lineSegment2, lineSegment1);
            }

            public static Point[] GetIntersections(LineSegment lineSegment1, LineSegment lineSegment2)
            {
                // TODO: Refactor GetIntersections
                // We already know that there is intersection
                // So now just get all intersection points

                double x1, y1, x2, y2;

                if (lineSegment1.First.X == lineSegment1.Second.X)
                {
                    // Case (A)
                    // As a is a perfect vertical line, it cannot be represented
                    // nicely in a mathematical way. But we directly know that
                    //
                    x1 = lineSegment1.First.X;
                    x2 = x1;
                    if (lineSegment2.First.X == lineSegment2.Second.X)
                    {
                        // Case (AA): all x are the same!
                        // Normalize
                        if (lineSegment1.First.Y > lineSegment1.Second.Y)
                        {
                            lineSegment1 = new(lineSegment1.Second, lineSegment1.First);
                        }
                        if (lineSegment2.First.Y > lineSegment2.Second.Y)
                        {
                            lineSegment2 = new(lineSegment2.Second, lineSegment2.First);
                        }
                        if (lineSegment1.First.Y > lineSegment2.First.Y)
                        {
                            (lineSegment1, lineSegment2) = (lineSegment2, lineSegment1);
                        }

                        // Now we know that the y-value of a.Start is the
                        // lowest of all 4 y values
                        // this means, we are either in case (AAA):
                        //   a: x--------------x
                        //   b:    x---------------x
                        // or in case (AAB)
                        //   a: x--------------x
                        //   b:    x-------x
                        // in both cases:
                        // get the relavant y intervall
                        y1 = lineSegment2.First.Y;
                        y2 = Math.Min(lineSegment1.Second.Y, lineSegment2.Second.Y);
                    }
                    else
                    {
                        // Case (AB)
                        // we can mathematically represent line b as
                        //     y = m*x + t <=> t = y - m*x
                        // m = (y1-y2)/(x1-x2)
                        double m, t;
                        m = (lineSegment2.First.Y - lineSegment2.Second.Y) /
                            (lineSegment2.First.X - lineSegment2.Second.X);
                        t = lineSegment2.First.Y - m * lineSegment2.First.X;
                        y1 = m * x1 + t;
                        y2 = y1;
                    }
                }
                else if (lineSegment2.First.X == lineSegment2.Second.X)
                {
                    // Case (B)
                    // essentially the same as Case (AB), but with
                    // a and b switched
                    x1 = lineSegment2.First.X;
                    x2 = x1;

                    (lineSegment1, lineSegment2) = (lineSegment2, lineSegment1);

                    double m, t;
                    m = (lineSegment2.First.Y - lineSegment2.Second.Y) /
                        (lineSegment2.First.X - lineSegment2.Second.X);
                    t = lineSegment2.First.Y - m * lineSegment2.First.X;
                    y1 = m * x1 + t;
                    y2 = y1;
                }
                else
                {
                    // Case (C)
                    // Both lines can be represented mathematically
                    double ma, mb, ta, tb;
                    ma = (lineSegment1.First.Y - lineSegment1.Second.Y) /
                         (lineSegment1.First.X - lineSegment1.Second.X);
                    mb = (lineSegment2.First.Y - lineSegment2.Second.Y) /
                         (lineSegment2.First.X - lineSegment2.Second.X);
                    ta = lineSegment1.First.Y - ma * lineSegment1.First.X;
                    tb = lineSegment2.First.Y - mb * lineSegment2.First.X;
                    if (ma == mb)
                    {
                        // Case (CA)
                        // both lines are in parallel. As we know that they
                        // intersect, the intersection could be a line
                        // when we rotated this, it would be the same situation
                        // as in case (AA)

                        // Normalize
                        if (lineSegment1.First.X > lineSegment1.Second.X)
                        {
                            lineSegment1 = new(lineSegment1.Second, lineSegment1.First);
                        }
                        if (lineSegment2.First.X > lineSegment2.Second.X)
                        {
                            lineSegment2 = new(lineSegment2.Second, lineSegment2.First);
                        }
                        if (lineSegment1.First.X > lineSegment2.First.X)
                        {
                            (lineSegment1, lineSegment2) = (lineSegment2, lineSegment1);
                        }

                        // get the relavant x intervall
                        x1 = lineSegment2.First.X;
                        x2 = Math.Min(lineSegment1.Second.X, lineSegment2.Second.X);
                        y1 = ma * x1 + ta;
                        y2 = ma * x2 + ta;
                    }
                    else
                    {
                        // Case (CB): only a point as intersection:
                        // y = ma*x+ta
                        // y = mb*x+tb
                        // ma*x + ta = mb*x + tb
                        // (ma-mb)*x = tb - ta
                        // x = (tb - ta)/(ma-mb)
                        x1 = (tb - ta) / (ma - mb);
                        y1 = ma * x1 + ta;
                        x2 = x1;
                        y2 = y1;
                    }
                }

                if (x1 == x2 && y1 == y2)
                {
                    return new[] { new Point(x1, y1) };
                }
                else
                {
                    return new[] { new Point(x1, y1), new Point(x2, y2) };
                }
            }

            private static bool TouchOrIntersect(LineSegment line, LineSegment lineSegment)
            {
                return (GetCrossProduct(line, lineSegment.First), GetCrossProduct(line, lineSegment.Second))
                    is (0, _)
                    or (_, 0)
                    or ( < 0, > 0)
                    or ( > 0, < 0);
            }

            private static double GetCrossProduct(LineSegment line, Point point)
            {
                var a = line.Second - line.First;
                var b = point - line.First;
                return (a.X * b.Y) - (b.X * a.Y);
            }

            private static bool IntersectBoundingBox(LineSegment lineSegment1, LineSegment lineSegment2)
            {
                var a = GetBoundingBox(lineSegment1);
                var b = GetBoundingBox(lineSegment2);

                return a.topLeft.X <= b.bottomRight.X
                    && a.bottomRight.X >= b.topLeft.X
                    && a.topLeft.Y <= b.bottomRight.Y
                    && a.bottomRight.Y >= b.topLeft.Y;
            }

            private static (Point topLeft, Point bottomRight) GetBoundingBox(LineSegment lineSegment)
            {
                return (
                    new(Math.Min(lineSegment.First.X, lineSegment.Second.X), Math.Min(lineSegment.First.Y, lineSegment.Second.Y)),
                    new(Math.Max(lineSegment.First.X, lineSegment.Second.X), Math.Max(lineSegment.First.Y, lineSegment.Second.Y)));
            }
        }
    }
}
