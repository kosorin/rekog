using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rekog.App.Kle
{
    public static class KleParser
    {
        private const int ValueCount = 12;

        private static readonly int?[,] ValueMap =
        {
            { 0, 6, 2, 8, 9, 11, 3, 5, 1, 4, 7, 10, },
            { 1, 7, null, null, 9, 11, 4, null, null, null, null, 10, },
            { 3, null, 5, null, 9, 11, null, null, 4, null, null, 10, },
            { 4, null, null, null, 9, 11, null, null, null, null, null, 10, },
            { 0, 6, 2, 8, 10, null, 3, 5, 1, 4, 7, null, },
            { 1, 7, null, null, 10, null, 4, null, null, null, null, null, },
            { 3, null, 5, null, 10, null, null, null, 4, null, null, null, },
            { 4, null, null, null, 10, null, null, null, null, null, null, null, },
        };

        public static KleBoard ParseJson(string json)
        {
            return ParseCore(json);
        }

        public static KleBoard ParseRawData(string rawData)
        {
            return ParseCore("[" + rawData + "]");
        }

        private static KleBoard ParseCore(string json)
        {
            var board = new KleBoard
            {
                Background = "#eeeeee",
            };

            var data = DeserializeJson(json);
            var currentKey = new KleKey
            {
                X = 0d,
                Y = 0d,
                Width = 1d,
                Height = 1d,

                X2 = 0d,
                Y2 = 0d,
                Width2 = 1d,
                Height2 = 1d,

                RotationAngle = 0d,
                RotationX = 0d,
                RotationY = 0d,

                Color = "#cccccc",
                IsHoming = false,
                IsDecal = false,
                IsGhosted = false,
                IsStepped = false,

                DefaultTextColor = "#000000",
                DefaultTextSize = 3,
            };
            var cluster = (x: 0d, y: 0d);
            var align = 4;

            var expectMetadata = true;

            foreach (var rowData in data)
            {
                if (rowData is JObject metadata)
                {
                    if (!expectMetadata)
                    {
                        throw new FormatException();
                    }

                    TryUse(metadata, "backcolor", (string value) => board.Background = value);
                    TryUse(metadata, "name", (string value) => board.Name = value);
                    TryUse(metadata, "author", (string value) => board.Author = value);
                    TryUse(metadata, "notes", (string value) => board.Notes = value);
                }
                else if (rowData is JArray)
                {
                    foreach (var keyData in rowData)
                    {
                        if (keyData is JObject properties)
                        {
                            TryUse(properties, "r", (double value) => currentKey.RotationAngle = value);
                            TryUse(properties, "rx", (double value) =>
                            {
                                cluster.x = value;
                                currentKey.RotationX = value;
                                (currentKey.X, currentKey.Y) = cluster;
                            });
                            TryUse(properties, "ry", (double value) =>
                            {
                                cluster.y = value;
                                currentKey.RotationY = value;
                                (currentKey.X, currentKey.Y) = cluster;
                            });

                            TryUse(properties, "a", (int value) => align = value);

                            TryUse(properties, "c", (string value) => currentKey.Color = value);
                            TryUse(properties, "t", (string value) =>
                            {
                                var colors = value.Split('\n');
                                currentKey.DefaultTextColor = colors[0];
                                currentKey.TextColors = GetValues(colors.Select(x => !string.IsNullOrEmpty(x) ? x : null).ToArray());
                            });

                            TryUse(properties, "f", (int value) =>
                            {
                                currentKey.DefaultTextSize = value;
                                currentKey.TextSizes = GetValues(Array.Empty<int?>());
                            });
                            TryUse(properties, "f2", (int value) =>
                            {
                                for (var i = 1; i < currentKey.TextSizes.Length; i++)
                                {
                                    currentKey.TextSizes[i] = value;
                                }
                            });
                            TryUse(properties, "fa", (int[] value) => currentKey.TextSizes = GetValues(value.Select(x => x > 0 ? x : (int?)null).ToArray()));

                            TryUse(properties, "x", (double value) => currentKey.X += value);
                            TryUse(properties, "y", (double value) => currentKey.Y += value);
                            TryUse(properties, "w", (double value) =>
                            {
                                currentKey.Width = value;
                                currentKey.Width2 = value;
                            });
                            TryUse(properties, "h", (double value) =>
                            {
                                currentKey.Height = value;
                                currentKey.Height2 = value;
                            });
                            TryUse(properties, "x2", (double value) => currentKey.X2 = value);
                            TryUse(properties, "y2", (double value) => currentKey.Y2 = value);
                            TryUse(properties, "w2", (double value) => currentKey.Width2 = value);
                            TryUse(properties, "h2", (double value) => currentKey.Height2 = value);

                            TryUse(properties, "n", (bool value) => currentKey.IsHoming = value);
                            TryUse(properties, "l", (bool value) => currentKey.IsStepped = value);
                            TryUse(properties, "d", (bool value) => currentKey.IsDecal = value);
                            TryUse(properties, "g", (bool value) => currentKey.IsGhosted = value);
                        }
                        else if (keyData is JValue legend)
                        {
                            var legends = GetValues(legend.Value<string>().Split('\n').Select(x => !string.IsNullOrEmpty(x) ? x : null).ToArray());
                            var key = new KleKey
                            {
                                X = currentKey.X,
                                Y = currentKey.Y,
                                Width = currentKey.Width,
                                Height = currentKey.Height,

                                X2 = currentKey.X2,
                                Y2 = currentKey.Y2,
                                Width2 = currentKey.Width2 == 0 ? currentKey.Width : currentKey.Width2,
                                Height2 = currentKey.Height2 == 0 ? currentKey.Height : currentKey.Height2,

                                RotationAngle = currentKey.RotationAngle,
                                RotationX = currentKey.RotationX,
                                RotationY = currentKey.RotationY,

                                Color = currentKey.Color,
                                IsHoming = currentKey.IsHoming,
                                IsDecal = currentKey.IsDecal,
                                IsGhosted = currentKey.IsGhosted,
                                IsStepped = currentKey.IsStepped,

                                DefaultTextColor = currentKey.DefaultTextColor,
                                DefaultTextSize = currentKey.DefaultTextSize,
                                TextColors = ReorderValues(currentKey.TextColors.ToArray(), align),
                                TextSizes = ReorderValues(currentKey.TextSizes.ToArray(), align),
                                Legends = ReorderValues(legends, align),
                            };
                            board.Keys.Add(key);

                            currentKey.X += currentKey.Width;
                            currentKey.Width = 1;
                            currentKey.Height = 1;

                            currentKey.X2 = 0;
                            currentKey.Y2 = 0;
                            currentKey.Width2 = 0;
                            currentKey.Height2 = 0;

                            currentKey.IsHoming = false;
                            currentKey.IsDecal = false;
                            currentKey.IsStepped = false;
                        }
                    }
                    currentKey.Y++;
                }
                currentKey.X = currentKey.RotationX;

                expectMetadata = false;
            }

            return board;
        }

        private static T[] GetValues<T>(T[] values)
        {
            var result = new T[ValueCount];
            Array.Copy(values, result, values.Length);
            return result;
        }

        private static T[] ReorderValues<T>(T[] values, int align)
        {
            var result = new T[ValueCount];
            for (var i = 0; i < values.Length; ++i)
            {
                if (ValueMap[align, i] is { } index)
                {
                    result[index] = values[i];
                }
            }
            return result;
        }

        private static void TryUse<T>(JObject properties, string propertyName, Action<T> action)
        {
            if (properties.TryGetValue(propertyName, out var token))
            {
                action.Invoke(token.Value<T>());
            }
        }

        private static void TryUse<T>(JObject properties, string propertyName, Action<T[]> action)
        {
            if (properties.TryGetValue(propertyName, out var token))
            {
                action.Invoke(token.Values<T>().ToArray());
            }
        }

        private static JArray DeserializeJson(string json)
        {
            if (JsonConvert.DeserializeObject(json) is not JArray rawData)
            {
                throw new JsonException();
            }

            return rawData;
        }
    }
}
