using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EngineWPFCoreUI
{
    /// <summary>
    /// Логика взаимодействия для ValueInput.xaml
    /// </summary>
    public partial class ValueInput : UserControl
    {
        static ValueInput()
        {
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ValueInput), new PropertyMetadata(0.0, ValueChangedCallback, _CoerceValueCallback));
            EditAllowedProperty = DependencyProperty.Register(nameof(EditAllowed), typeof(bool), typeof(ValueInput), new PropertyMetadata(true, EditAllowedChangedCallback));
            ValueChangeAllowedProperty = DependencyProperty.Register(nameof(ValueChangeAllowed), typeof(bool), typeof(ValueInput), new PropertyMetadata(true, ValueChangeAllowedChangedCallback));
            SensitivityProperty = DependencyProperty.Register(nameof(Sensitivity), typeof(double), typeof(ValueInput), new PropertyMetadata(0.1, SensitivityChangedCallback));
            FastMultiplierProperty = DependencyProperty.Register(nameof(FastMultiplier), typeof(double), typeof(ValueInput), new PropertyMetadata(10.0, FastMultiplierChangedCallback));
            SlowMultiplierProperty = DependencyProperty.Register(nameof(SlowMultiplier), typeof(double), typeof(ValueInput), new PropertyMetadata(0.1, SlowMultiplierChangedCallback));
        }

        

        public ValueInput()
        {
            InitializeComponent();
        }

        public event EventHandler<double> ValueChanged;
        public static readonly DependencyProperty ValueProperty;
        public double Value 
        {
            get
            {
                return (double)base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            } 
        }

        public event EventHandler<bool> EditAllowedChanged;
        public static readonly DependencyProperty EditAllowedProperty;
        public bool EditAllowed
        {
            get
            {
                return (bool)base.GetValue(EditAllowedProperty);
            }
            set
            {
                base.SetValue(EditAllowedProperty, value);
            }
        }
        public event EventHandler<bool> ValueChangeAllowedChanged;
        public static readonly DependencyProperty ValueChangeAllowedProperty;
        public bool ValueChangeAllowed
        {
            get
            {
                return (bool)base.GetValue(ValueChangeAllowedProperty);
            }
            set
            {
                base.SetValue(ValueChangeAllowedProperty, value);
            }
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueInput)d).textBox.Text = ((double)e.NewValue).ToString("0.00######");
            ((ValueInput)d).ValueChanged?.Invoke(d, (double)e.NewValue);
        }
        private static object _CoerceValueCallback(DependencyObject d, object baseValue)
        {
            var input = ((ValueInput)d);
            double val;
            //if (!input.EditAllowed)
            if (!input.ValueChangeAllowed)
                val = input.Value;
            else
                val = (double)baseValue;
            input.textBox.Text = val.ToString("0.00######");
            return val;
        }
        private static void EditAllowedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = ((ValueInput)d);
            if (newval)
                input.textBlock.FontWeight = FontWeights.Bold;
            else
                input.textBlock.FontWeight = FontWeights.Normal;
            input.EditAllowedChanged?.Invoke(d, newval);
        }
        private static void ValueChangeAllowedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = ((ValueInput)d);
            if (newval)
                input.textBlock.TextDecorations = TextDecorations.Underline;
            else
                input.textBlock.TextDecorations = null;
            input.ValueChangeAllowedChanged?.Invoke(d, newval);
        }

        public event EventHandler<double> SensitivityChanged;
        public static readonly DependencyProperty SensitivityProperty;
        public double Sensitivity
        {
            get
            {
                return (double)base.GetValue(SensitivityProperty);
            }
            set
            {
                base.SetValue(SensitivityProperty, value);
            }
        }
        private static void SensitivityChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueInput)d).sens = (double)e.NewValue;
            ((ValueInput)d).SensitivityChanged?.Invoke(d, (double)e.NewValue);
        }

        public event EventHandler<double> FastMultiplierChanged;
        public static readonly DependencyProperty FastMultiplierProperty;
        public double FastMultiplier
        {
            get
            {
                return (double)base.GetValue(FastMultiplierProperty);
            }
            set
            {
                base.SetValue(FastMultiplierProperty, value);
            }
        }
        private static void FastMultiplierChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueInput)d).fast = (double)e.NewValue;
            ((ValueInput)d).FastMultiplierChanged?.Invoke(d, (double)e.NewValue);
        }

        public event EventHandler<double> SlowMultiplierChanged;
        public static readonly DependencyProperty SlowMultiplierProperty;
        public double SlowMultiplier
        {
            get
            {
                return (double)base.GetValue(SlowMultiplierProperty);
            }
            set
            {
                base.SetValue(SlowMultiplierProperty, value);
            }
        }
        private static void SlowMultiplierChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueInput)d).slow = (double)e.NewValue;
            ((ValueInput)d).SlowMultiplierChanged?.Invoke(d, (double)e.NewValue);
        }

        bool inTBmode;
        void TBMode()
        {
            if (!inTBmode)
            {
                label.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;
                textBox.Focus();
                textBox.SelectAll();
                inTBmode = true;
            }
        }
        void LBMode()
        {
            if (inTBmode)
            {
                label.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Collapsed;
                inTBmode = false;
                //label.Content = Value.ToString("f2");
                textBox.Text = Value.ToString("0.00######");
            }
        }

        private void WrapPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (!textBox.IsFocused)
            //    LBMode();
        }

        bool value_edit_down = false;
        bool value_edit = false;
        double vlx, vly;
        double sens = 0.1;
        double fast = 10.0;
        double slow = 0.1;

        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditAllowed)
                if (!inTBmode && e.ChangedButton == MouseButton.Left)
                {
                    value_edit_down = true;
                    CaptureMouse();
                    vlx = e.GetPosition(this).X;
                    vly = e.GetPosition(this).Y;
                }
        }
        private void WrapPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EditAllowed)
            {
                if (!value_edit && e.ChangedButton == MouseButton.Left)
                    TBMode();
                value_edit_down = false;
                value_edit = false;
            }
            ReleaseMouseCapture();
        }
        private void WrapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (value_edit_down && EditAllowed)
            {
                double dy = e.GetPosition(this).Y - vly;
                double dx = e.GetPosition(this).X - vlx;
                if (value_edit)
                {
                    double m = 1.0;
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        m = fast;
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        m = slow;


                    double d = 0.0;

                    d = -dx * sens * m;
                    d += dy * sens * m;

                    Value = Value - d;
                }
                if (!value_edit && (Math.Abs(dy) > 25.0 || Math.Abs(dx) > 25.0))
                {
                    value_edit = true;

                    vly = e.GetPosition(this).Y;
                    vlx = e.GetPosition(this).X;
                }
                if (value_edit)
                {
                    vly = e.GetPosition(this).Y;
                    vlx = e.GetPosition(this).X;
                }
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double new_val;
            if (double.TryParse(textBox.Text, out new_val))
            {
                if (new_val != Value)
                {
                    Value = new_val;
                }

            }
            LBMode();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double new_val;
                if (double.TryParse(textBox.Text, out new_val))
                {
                    Value = new_val;
                }
                LBMode();
            }
            else if (e.Key == Key.Escape)
            {
                LBMode();
            }

        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    


    public class StringToStringFloat2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (double.TryParse((string)value, out val))
                return val.ToString("f2");
            else
                return 0.0.ToString("f2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class DoubleToStringConverterF6 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (double.TryParse((string)value, out val))
                return val;
            else
                return 0.0;
        }
    }
    public class DoubleToStringConverterF2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("f2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (double.TryParse((string)value, out val))
                return val;
            else
                return 0.0;
        }
    }
}
