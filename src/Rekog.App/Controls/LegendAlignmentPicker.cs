using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Rekog.App.Model;

namespace Rekog.App.Controls
{
    [TemplatePart(Name = "PART_TopLeft", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_TopCenter", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_TopRight", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_CenterLeft", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_CenterCenter", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_CenterRight", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_BottomLeft", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_BottomCenter", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_BottomRight", Type = typeof(ToggleButton))]
    public class LegendAlignmentPicker : Control
    {
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register(nameof(AllowNull), typeof(bool), typeof(LegendAlignmentPicker), new PropertyMetadata(false, OnAllowNullChanged));

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(nameof(Alignment), typeof(LegendAlignment?), typeof(LegendAlignmentPicker), new PropertyMetadata(null, OnAlignmentChanged));

        private bool _manual;
        private Dictionary<LegendAlignment, ToggleButton>? _buttons;

        static LegendAlignmentPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendAlignmentPicker), new FrameworkPropertyMetadata(typeof(LegendAlignmentPicker)));
        }

        public bool AllowNull
        {
            get => (bool)GetValue(AllowNullProperty);
            set => SetValue(AllowNullProperty, value);
        }

        public LegendAlignment? Alignment
        {
            get => (LegendAlignment?)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_buttons != null)
            {
                foreach (var button in _buttons.Values)
                {
                    button.Checked -= OnButtonChecked;
                    button.Unchecked -= OnButtonUnchecked;
                }
            }

            _buttons = new Dictionary<LegendAlignment, ToggleButton>
            {
                [LegendAlignment.TopLeft] = (ToggleButton)Template.FindName("PART_TopLeft", this),
                [LegendAlignment.Top] = (ToggleButton)Template.FindName("PART_TopCenter", this),
                [LegendAlignment.TopRight] = (ToggleButton)Template.FindName("PART_TopRight", this),
                [LegendAlignment.Left] = (ToggleButton)Template.FindName("PART_CenterLeft", this),
                [LegendAlignment.Center] = (ToggleButton)Template.FindName("PART_CenterCenter", this),
                [LegendAlignment.Right] = (ToggleButton)Template.FindName("PART_CenterRight", this),
                [LegendAlignment.BottomLeft] = (ToggleButton)Template.FindName("PART_BottomLeft", this),
                [LegendAlignment.Bottom] = (ToggleButton)Template.FindName("PART_BottomCenter", this),
                [LegendAlignment.BottomRight] = (ToggleButton)Template.FindName("PART_BottomRight", this),
            };

            foreach (var button in _buttons.Values)
            {
                button.Checked += OnButtonChecked;
                button.Unchecked += OnButtonUnchecked;
            }

            UpdateData();
        }

        private void UpdateData()
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            _manual = true;
            try
            {
                foreach (var (alignment, button) in _buttons)
                {
                    button.IsChecked = alignment == Alignment;
                }
            }
            finally
            {
                _manual = false;
            }
        }

        private static void OnAllowNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is LegendAlignmentPicker picker)
            {
                picker.UpdateData();
            }
        }

        private static void OnAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is LegendAlignmentPicker picker)
            {
                picker.UpdateData();
            }
        }

        private void OnButtonChecked(object sender, RoutedEventArgs args)
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            Alignment = _buttons.First(x => x.Value == sender).Key;
        }

        private void OnButtonUnchecked(object sender, RoutedEventArgs args)
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            if (AllowNull)
            {
                Alignment = null;
            }
            else
            {
                if (sender is ToggleButton button)
                {
                    _manual = true;
                    try
                    {
                        button.IsChecked = true;
                    }
                    finally
                    {
                        _manual = false;
                    }
                }
            }
        }
    }
}
