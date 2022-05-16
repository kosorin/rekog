using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Rekog.App.Model;
using Rekog.App.ObjectModel;

namespace Rekog.App.Kle
{
    public static class KleConverter
    {
        public static BoardModel ConvertJson(string json)
        {
            return Convert(KleParser.ParseJson(json));
        }

        public static BoardModel ConvertRawData(string rawData)
        {
            return Convert(KleParser.ParseRawData(rawData));
        }

        private static BoardModel Convert(KleBoard kleBoard)
        {
            var layerData = Enumerable.Range(0, 12)
                .Where(layer => kleBoard.Keys.Any(key => !string.IsNullOrWhiteSpace(key.Legends[layer])))
                .Select((x, i) => (id: i, index: x))
                .ToList();
            return new BoardModel
            {
                Name = kleBoard.Name,
                Author = kleBoard.Author,
                Notes = kleBoard.Notes,
                Background = kleBoard.Background,
                Keys = ConvertKeys(kleBoard.Keys),
                Layers = ConvertLayers(layerData),
                Legends = ConvertLegends(layerData, kleBoard.Keys),
            };
        }

        private static ObservableDictionary<KeyId, KeyModel> ConvertKeys(List<KleKey> kleKeys)
        {
            return new ObservableDictionary<KeyId, KeyModel>(kleKeys.Select((kleKey, keyId) => new KeyValuePair<KeyId, KeyModel>(keyId, ConvertKey(keyId, kleKey))));

            static KeyModel ConvertKey(KeyId keyId, KleKey kleKey)
            {
                var x = kleKey.X;
                var y = kleKey.Y;
                var width = kleKey.Width;
                var height = kleKey.Height;
                var steppedOffsetX = 0d;
                var steppedOffsetY = 0d;
                var steppedWidth = kleKey.Width;
                var steppedHeight = kleKey.Height;
                string? shape = null;

                if (!kleKey.IsSimple)
                {
                    if (kleKey.IsSimpleInverted)
                    {
                        x = kleKey.X + kleKey.X2;
                        y = kleKey.Y + kleKey.Y2;
                        width = kleKey.Width2;
                        height = kleKey.Height2;
                        steppedOffsetX = -kleKey.X2;
                        steppedOffsetY = -kleKey.Y2;
                        steppedWidth = kleKey.Width;
                        steppedHeight = kleKey.Height;
                    }
                    else
                    {
                        var geometry1 = new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height));
                        var geometry2 = new RectangleGeometry(new Rect(kleKey.X2, kleKey.Y2, kleKey.Width2, kleKey.Height2));
                        shape = new CombinedGeometry(GeometryCombineMode.Union, geometry1, geometry2).GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture);

                        // Delete "fill rule"
                        if (shape[0] == 'F')
                        {
                            shape = shape[2..];
                        }
                    }
                }

                var hasRotation = kleKey.RotationAngle != 0 || kleKey.RotationX != 0 || kleKey.RotationY != 0;

                return new KeyModel(keyId)
                {
                    X = x,
                    Y = y,

                    RotationAngle = kleKey.RotationAngle,
                    RotationOriginX = hasRotation ? kleKey.RotationX : null,
                    RotationOriginY = hasRotation ? kleKey.RotationY : null,

                    Width = width,
                    Height = height,
                    UseShape = shape != null,
                    Shape = shape,

                    IsStepped = kleKey.IsStepped,
                    SteppedOffsetX = steppedOffsetX,
                    SteppedOffsetY = steppedOffsetY,
                    SteppedWidth = steppedWidth,
                    SteppedHeight = steppedHeight,
                    UseSteppedShape = false,
                    SteppedShape = null,

                    Color = kleKey.Color,

                    IsHoming = kleKey.IsHoming,
                    IsGhosted = kleKey.IsGhosted,
                    IsDecal = kleKey.IsDecal,
                };
            }
        }

        private static ObservableDictionary<LayerId, LayerModel> ConvertLayers(List<(int id, int index)> layerData)
        {
            var horizontalNames = new[] { "Left", "Center", "Right", };
            var verticalNames = new[] { "Top", "Middle", "Bottom", "Front", };

            var layers = new ObservableDictionary<LayerId, LayerModel>();

            foreach (var (id, index) in layerData)
            {
                var layerId = new LayerId(id);
                var layer = new LayerModel(layerId)
                {
                    Name = verticalNames[index / horizontalNames.Length] + " " + horizontalNames[index % horizontalNames.Length],
                    Order = id,
                };
                layers[layerId] = layer;
            }

            return layers;
        }

        private static ObservableDictionary<LegendId, LegendModel> ConvertLegends(List<(int id, int index)> layerData, IEnumerable<KleKey> kleKeys)
        {
            return new ObservableDictionary<LegendId, LegendModel>(layerData
                .Join(kleKeys.Select((x, i) => (keyId: i, kleKey: x)), _ => true, _ => true, (lx, kx) => (id: new LegendId(kx.keyId, lx.id), lx.index, kx.kleKey))
                .Select(x => x.index < 9 ? ConvertLegend(x.id, x.index, x.kleKey) : ConvertFrontLegend(x.id, x.index, x.kleKey))
                .ToDictionary(x => x.Id, x => x));

            static LegendModel ConvertLegend(LegendId id, int index, KleKey kleKey)
            {
                return new LegendModel(id)
                {
                    Value = kleKey.Legends[index] ?? string.Empty,

                    Alignment = (LegendAlignment)index,

                    Size = GetLegendSize(kleKey.TextSizes[index] ?? kleKey.DefaultTextSize),
                    Color = kleKey.TextColors[index] ?? kleKey.DefaultTextColor,
                };
            }

            static LegendModel ConvertFrontLegend(LegendId id, int index, KleKey kleKey)
            {
                return new LegendModel(id)
                {
                    Value = kleKey.Legends[index] ?? string.Empty,

                    Alignment = (LegendAlignment)(index - 3),
                    Bottom = -0.22,

                    Size = GetLegendSize(kleKey.FrontLegendTextSize),
                    Color = kleKey.TextColors[index] ?? kleKey.DefaultTextColor,
                };
            }

            static double GetLegendSize(double kleTextSize)
            {
                return 8 + 4 * kleTextSize;
            }
        }
    }
}
