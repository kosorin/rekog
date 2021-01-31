﻿using System;

namespace Rekog.App.Model.Kle
{
    public class KleKey
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }


        public double X2 { get; set; }

        public double Y2 { get; set; }

        public double Width2 { get; set; }

        public double Height2 { get; set; }

        public bool IsSimple => X2 == 0 && Y2 == 0 && Width == Width2 && Height == Height2;


        public double RotationAngle { get; set; }

        public double RotationX { get; set; }

        public double RotationY { get; set; }


        public string Color { get; set; } = string.Empty;

        public bool IsHoming { get; set; }

        public bool IsDecal { get; set; }

        public bool IsGhosted { get; set; }

        public bool IsStepped { get; set; }


        public string DefaultTextColor { get; set; } = string.Empty;

        public int DefaultTextSize { get; set; }

        public string?[] TextColors { get; set; } = Array.Empty<string?>();

        public int?[] TextSizes { get; set; } = Array.Empty<int?>();

        public string?[] Labels { get; set; } = Array.Empty<string?>();
    }
}