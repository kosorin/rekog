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
            return new BoardModel
            {
                Name = kleBoard.Name,
                Author = kleBoard.Author,
                Notes = kleBoard.Notes,
                Background = kleBoard.Background,
                Keys = ConvertKeys(kleBoard.Keys),
                Layers = ConvertLayers(),
                Legends = ConvertLegends(kleBoard.Keys),
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

                return new KeyModel(keyId)
                {
                    X = x,
                    Y = y,

                    RotationAngle = kleKey.RotationAngle,
                    RotationOriginX = kleKey.RotationX,
                    RotationOriginY = kleKey.RotationY,

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

        private static ObservableDictionary<LayerId, LayerModel> ConvertLayers()
        {
            var horizontalNames = new[] { "Left", "Center", "Right", };
            var verticalNames = new[] { "Top", "Middle", "Bottom", "Front", };

            var layers = new ObservableDictionary<LayerId, LayerModel>();
            for (var i = 0; i < 12; i++)
            {
                var layer = new LayerModel(i)
                {
                    Name = verticalNames[i / horizontalNames.Length] + " " + horizontalNames[i % horizontalNames.Length],
                    Order = i,
                };
                layers[i] = layer;
            }

            return layers;
        }

        private static ObservableDictionary<LegendId, LegendModel> ConvertLegends(List<KleKey> kleKeys)
        {
            return new ObservableDictionary<LegendId, LegendModel>(Enumerable.Range(0, 12)
                .Join(kleKeys.Select((x, i) => (keyId: i, kleKey: x)), _ => true, _ => true, (layerId, x) => (id: new LegendId(x.keyId, layerId), x.kleKey))
                .Select(x => x.id.LayerId.Value < 9 ? ConvertLegend(x.id, x.kleKey) : ConvertFrontLegend(x.id, x.kleKey))
                .ToDictionary(x => x.Id, x => x));

            static LegendModel ConvertLegend(LegendId id, KleKey kleKey)
            {
                return new LegendModel(id)
                {
                    Value = kleKey.Legends[id.LayerId.Value] ?? string.Empty,

                    Alignment = (LegendAlignment)id.LayerId.Value,

                    Size = GetLegendSize(kleKey.TextSizes[id.LayerId.Value] ?? kleKey.DefaultTextSize),
                    Color = kleKey.TextColors[id.LayerId.Value] ?? kleKey.DefaultTextColor,
                };
            }

            static LegendModel ConvertFrontLegend(LegendId id, KleKey kleKey)
            {
                return new LegendModel(id)
                {
                    Value = kleKey.Legends[id.LayerId.Value] ?? string.Empty,

                    Alignment = (LegendAlignment)(id.LayerId.Value - 3),
                    Bottom = -0.22,

                    Size = GetLegendSize(kleKey.FrontLegendTextSize),
                    Color = kleKey.TextColors[id.LayerId.Value] ?? kleKey.DefaultTextColor,
                };
            }

            static double GetLegendSize(double kleTextSize)
            {
                return 8 + 4 * kleTextSize;
            }
        }
    }
}
