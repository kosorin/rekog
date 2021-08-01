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
    public class KeyLabelAlignmentPicker : Control
    {
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register(nameof(AllowNull), typeof(bool), typeof(KeyLabelAlignmentPicker), new PropertyMetadata(false, OnAllowNullChanged));

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(nameof(Alignment), typeof(KeyLabelAlignment?), typeof(KeyLabelAlignmentPicker), new PropertyMetadata(null, OnAlignmentChanged));

        private bool _manual;
        private Dictionary<KeyLabelAlignment, ToggleButton>? _buttons;

        static KeyLabelAlignmentPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyLabelAlignmentPicker), new FrameworkPropertyMetadata(typeof(KeyLabelAlignmentPicker)));
        }

        public bool AllowNull
        {
            get => (bool)GetValue(AllowNullProperty);
            set => SetValue(AllowNullProperty, value);
        }

        public KeyLabelAlignment? Alignment
        {
            get => (KeyLabelAlignment?)GetValue(AlignmentProperty);
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

            _buttons = new Dictionary<KeyLabelAlignment, ToggleButton>
            {
                [KeyLabelAlignment.TopLeft] = (ToggleButton)Template.FindName("PART_TopLeft", this),
                [KeyLabelAlignment.Top] = (ToggleButton)Template.FindName("PART_TopCenter", this),
                [KeyLabelAlignment.TopRight] = (ToggleButton)Template.FindName("PART_TopRight", this),
                [KeyLabelAlignment.Left] = (ToggleButton)Template.FindName("PART_CenterLeft", this),
                [KeyLabelAlignment.Center] = (ToggleButton)Template.FindName("PART_CenterCenter", this),
                [KeyLabelAlignment.Right] = (ToggleButton)Template.FindName("PART_CenterRight", this),
                [KeyLabelAlignment.BottomLeft] = (ToggleButton)Template.FindName("PART_BottomLeft", this),
                [KeyLabelAlignment.Bottom] = (ToggleButton)Template.FindName("PART_BottomCenter", this),
                [KeyLabelAlignment.BottomRight] = (ToggleButton)Template.FindName("PART_BottomRight", this),
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
                    Alignment = KeyLabelAlignment.TopLeft;
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
            if (d is KeyLabelAlignmentPicker picker)
            {
                picker.UpdateData();
            }
        }

        private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is KeyLabelAlignmentPicker picker)
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
