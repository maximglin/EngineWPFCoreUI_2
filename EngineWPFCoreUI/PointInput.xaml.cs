using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для PointEditor.xaml
    /// </summary>
    public partial class PointInput : UserControl
    {
        public PointInput()
        {
            InitializeComponent();
            XValue.ValueChanged += XValue_ValueChanged;
            YValue.ValueChanged += YValue_ValueChanged;
        }

       
        private void XValue_ValueChanged(object sender, double e)
        {
            Value = new Point(e, Value.Y);
        }
        private void YValue_ValueChanged(object sender, double e)
        {
            Value = new Point(Value.X, e);
        }

        static PointInput()
        {
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Point), typeof(PointInput), new PropertyMetadata(default(Point), ValueChangedCallback));
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (PointInput)d;
            input.XValue.Value = newval.X;
            input.YValue.Value = newval.Y;
        }

        public static readonly DependencyProperty ValueProperty;
        public Point Value
        {
            get => (Point)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }

    }
}
