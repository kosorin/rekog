using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts
{
    public class Matrix<T> where T : notnull
    {
        private readonly T[,] _data;
        private readonly Dictionary<T, (int x, int y)> _positions = new();

        public Matrix(List<List<T>> rawData)
        {
            Width = rawData.Max(x => x.Count);
            Height = rawData.Count;

            _data = new T[Height, Width];
            for (var y = 0; y < rawData.Count; y++)
            {
                for (var x = 0; x < rawData[y].Count; x++)
                {
                    var value = rawData[y][x];
                    _data[y, x] = value;
                    _positions[value] = (x, y);
                }
            }
        }

        public int Width { get; }

        public int Height { get; }

        public (int x, int y) this[T value] => _positions[value];

        public T this[int x, int y] => GetValue(x, y);

        public T this[(int x, int y) position] => GetValue(position.x, position.y);

        private T GetValue(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
            return _data[x, y];
        }
    }
}
