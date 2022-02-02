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
    /// Логика взаимодействия для BezierMarker.xaml
    /// </summary>
    public partial class BezierMarker : UserControl
    {
        public BezierMarker()
        {
            InitializeComponent();
        }
        static BezierMarker()
        {
            PProperty = DependencyProperty.Register(nameof(P), typeof(Point), typeof(BezierMarker), new PropertyMetadata(new Point(), PChanged));
            LProperty = DependencyProperty.Register(nameof(L), typeof(Point), typeof(BezierMarker), new PropertyMetadata(new Point(), LChanged));
            RProperty = DependencyProperty.Register(nameof(R), typeof(Point), typeof(BezierMarker), new PropertyMetadata(new Point(), RChanged));
            ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(BezierMarker), new PropertyMetadata(Brushes.DarkGray, ColorChanged));
        
            RVisibleProperty = DependencyProperty.Register(nameof(RVisible), typeof(bool), typeof(BezierMarker), new PropertyMetadata(true, RVisibleChanged));
            LVisibleProperty = DependencyProperty.Register(nameof(LVisible), typeof(bool), typeof(BezierMarker), new PropertyMetadata(true, LVisibleChanged));
        }

        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Brush)e.NewValue;
            var input = (BezierMarker)d;

            input.rect.Fill = newval;
            input.elL.Fill = newval;
            input.elR.Fill = newval;
            input.l1.Stroke = newval;
            input.l2.Stroke = newval;
        }

        private static void PChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (BezierMarker)d;

            input.Ppos.X = newval.X;
            input.Ppos.Y = newval.Y;
        }

        public static readonly DependencyProperty PProperty;
        public Point P
        {
            get => (Point)base.GetValue(PProperty);
            set => base.SetValue(PProperty, value);
        }
        private static void LChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (BezierMarker)d;

            input.Lpos.X = newval.X;
            input.Lpos.Y = newval.Y;
        }

        public static readonly DependencyProperty LProperty;
        public Point L
        {
            get => (Point)base.GetValue(LProperty);
            set => base.SetValue(LProperty, value);
        }


        private static void RChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (BezierMarker)d;

            input.Rpos.X = newval.X;
            input.Rpos.Y = newval.Y;
        }

        public static readonly DependencyProperty RProperty;
        public Point R
        {
            get => (Point)base.GetValue(RProperty);
            set => base.SetValue(RProperty, value);
        }


        private static void RVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = (BezierMarker)d;

            input.elR.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
        }
        private static void LVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = (BezierMarker)d;

            input.elL.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
        }



        public static readonly DependencyProperty ColorProperty;
        public Brush Color
        {
            get => (Brush)base.GetValue(ColorProperty);
            set => base.SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty RVisibleProperty;
        public bool RVisible
        {
            get => (bool)base.GetValue(RVisibleProperty);
            set => base.SetValue(RVisibleProperty, value);
        }
        public static readonly DependencyProperty LVisibleProperty;
        public bool LVisible
        {
            get => (bool)base.GetValue(LVisibleProperty);
            set => base.SetValue(LVisibleProperty, value);
        }

        enum Selected
        { L, P, R, None}
        Selected sel = Selected.None;
        private void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            sel = Selected.P;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch(sel)
            {
                case Selected.P:
                    var point = Mouse.GetPosition(canvas);
                    var dif = point - P;
                    P = point;
                    //L += dif;
                    //R += dif;
                    break;
                case Selected.L:
                    L = Mouse.GetPosition(canvas);
                    break;
                case Selected.R:
                    R = Mouse.GetPosition(canvas);
                    break;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            sel = Selected.None;
            ReleaseMouseCapture();
        }

        private void elL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            sel = Selected.L;
        }

        private void elR_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            sel = Selected.R;
        }
    }
}
