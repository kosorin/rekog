using System.Windows.Media;

namespace Rekog.App.Extensions
{
    public static class GeometryExtensions
    {
        public static PathGeometry GetEnlargedPathGeometry(this Geometry geometry, double size)
        {
            return GetEnlargedPathGeometry(geometry, size, false);
        }

        public static PathGeometry GetEnlargedPathGeometry(this Geometry geometry, double size, bool round)
        {
            var pen = new Pen(Brushes.Black, size * 2)
            {
                LineJoin = round ? PenLineJoin.Round : PenLineJoin.Miter,
            };
            var widenedPathGeometry = geometry.GetWidenedPathGeometry(pen).GetOutlinedPathGeometry();
            if (widenedPathGeometry.Figures.Count == 2)
            {
                widenedPathGeometry.Figures.RemoveAt(size > 0 ? 0 : 1);
            }

            return widenedPathGeometry.GetFlattenedPathGeometry();
        }
    }
}
