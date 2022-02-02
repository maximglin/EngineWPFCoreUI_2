using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для MyBezierSegment.xaml
    /// </summary>
    public partial class MyBezierSegment : UserControl
    {

        static MyBezierSegment()
        {
            P1Property = DependencyProperty.Register(nameof(P1), typeof(Point), typeof(MyBezierSegment), new PropertyMetadata(new Point(0.0, 0.0), P1ChangedCallback, CoerceP1Callback));
            R1Property = DependencyProperty.Register(nameof(R1), typeof(Point), typeof(MyBezierSegment), new PropertyMetadata(new Point(1.0, 0.0), R1ChangedCallback, CoerceR1Callback));
            L2Property = DependencyProperty.Register(nameof(L2), typeof(Point), typeof(MyBezierSegment), new PropertyMetadata(new Point(2.0, 0.0), L2ChangedCallback, CoerceL2Callback));
            P2Property = DependencyProperty.Register(nameof(P2), typeof(Point), typeof(MyBezierSegment), new PropertyMetadata(new Point(3.0, 0.0), P2ChangedCallback, CoerceP2Callback));

            ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(MyBezierSegment), new PropertyMetadata(Brushes.DarkGray, ColorChangedCallback));
        }

        #region callbacks
        private static object CoerceP1Callback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void P1ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (MyBezierSegment)d;
            input.Start.StartPoint = newval;
        }
        private static object CoerceR1Callback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void R1ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (MyBezierSegment)d;
            input.End.Point1 = newval;
        }

        private static object CoerceL2Callback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void L2ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (MyBezierSegment)d;
            input.End.Point2 = newval;
        }

        private static object CoerceP2Callback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void P2ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (MyBezierSegment)d;
            input.End.Point3 = newval;
        }


        private static void ColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Brush)e.NewValue;
            var input = (MyBezierSegment)d;
            input._Path.Stroke = newval;
        }
        #endregion

        public MyBezierSegment()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty P1Property;
        public static readonly DependencyProperty R1Property;
        public static readonly DependencyProperty L2Property;
        public static readonly DependencyProperty P2Property;

        public static readonly DependencyProperty ColorProperty;
        
        public Point P1
        {
            get => (Point)base.GetValue(P1Property);
            set => base.SetValue(P1Property, value);
        }
        public Point R1
        {
            get => (Point)base.GetValue(R1Property);
            set => base.SetValue(R1Property, value);
        }
        public Point L2
        {
            get => (Point)base.GetValue(L2Property);
            set => base.SetValue(L2Property, value);
        }
        public Point P2
        {
            get => (Point)base.GetValue(P2Property);
            set => base.SetValue(P2Property, value);
        }

        public Brush Color
        {
            get => (Brush)base.GetValue(ColorProperty);
            set => base.SetValue(ColorProperty, value);
        }
    }

    public class TwoDoublesToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var X = (double)values[0];
            var Y = (double)values[1];
            return new Point(X, Y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
