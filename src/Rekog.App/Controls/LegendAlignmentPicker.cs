using System.Collections.Generic;
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
                    button.Checked -= Button_Checked;
                    button.Unchecked -= Button_Unchecked;
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
                button.Checked += Button_Checked;
                button.Unchecked += Button_Unchecked;
            }

            UpdateData();
        }

        private void UpdateData()
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            try
            {
                _manual = true;

                if (!Alignment.HasValue && !AllowNull)
                {
                    Alignment = LegendAlignment.TopLeft;
                }

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

        private static void OnAllowNullChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LegendAlignmentPicker picker)
            {
                picker.UpdateData();
            }
        }

        private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LegendAlignmentPicker picker)
            {
                picker.UpdateData();
            }
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            try
            {
                _manual = true;

                foreach (var (alignment, button) in _buttons)
                {
                    if (button == sender)
                    {
                        Alignment = alignment;
                    }
                    else
                    {
                        button.IsChecked = false;
                    }
                }
            }
            finally
            {
                _manual = false;
            }
        }

        private void Button_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_manual || _buttons == null)
            {
                return;
            }

            try
            {
                _manual = true;

                if (sender is ToggleButton button)
                {
                    if (AllowNull)
                    {
                        Alignment = null;
                    }
                    else
                    {
                        button.IsChecked = true;
                    }
                }
            }
            finally
            {
                _manual = false;
            }
        }
    }
}
