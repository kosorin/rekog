using System.Runtime.CompilerServices;
using System.Windows;

namespace Rekog.App.ViewModel.Values
{
    public class PointValueSource : ValueSource<Point>
    {
        public PointValueSource(Point value, [CallerMemberName] string? key = null) : base(value, key)
        {
        }
    }
}
