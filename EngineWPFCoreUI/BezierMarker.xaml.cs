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

            P2Property = DependencyProperty.Register(nameof(P2), typeof(Point), typeof(BezierMarker), new PropertyMetadata(new Point(), P2Changed));


            SplitProperty = DependencyProperty.Register(nameof(Split), typeof(bool), typeof(BezierMarker), new PropertyMetadata(false, SplitChanged));
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

            input.rect21.Fill = newval;
            input.rect22.Fill = newval;
            input.connector.Stroke = newval;
        }

        private static void PChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (BezierMarker)d;

            input.Ppos.X = newval.X;
            input.Ppos.Y = newval.Y;

            input.P21pos.X = newval.X;
            input.P21pos.Y = newval.Y;
            if (input.Split == false)
            {
                input.P22pos.X = newval.X;
                input.P22pos.Y = newval.Y;
            }
        }

        public static readonly DependencyProperty PProperty;
        public Point P
        {
            get => (Point)base.GetValue(PProperty);
            set => base.SetValue(PProperty, value);
        }

        
        private static void P2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (Point)e.NewValue;
            var input = (BezierMarker)d;

            {
                input.P22pos.X = newval.X;
                input.P22pos.Y = newval.Y;
            }
        }

        public static readonly DependencyProperty P2Property;
        public Point P2
        {
            get => (Point)base.GetValue(P2Property);
            set => base.SetValue(P2Property, value);
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
            input.l2.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
        }
        private static void LVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = (BezierMarker)d;

            input.elL.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
            input.l1.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
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


        private static void SplitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newval = (bool)e.NewValue;
            var input = (BezierMarker)d;


            input.rect21.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
            input.rect22.Visibility = newval ? Visibility.Visible : Visibility.Collapsed;
            input.rect.Visibility =  !newval ? Visibility.Visible : Visibility.Collapsed;
            if(newval == false)
                input.P2 = input.P;
        }

        public static readonly DependencyProperty SplitProperty;
        public bool Split
        {
            get => (bool)base.GetValue(SplitProperty);
            set => base.SetValue(SplitProperty, value);
        }

        enum Selected
        { L, P, R, None, P2}
        Selected sel = Selected.None;

        bool linear = false;
        bool linear2 = false;
        private void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if(Split)
                {
                    if(linear == false)
                    {
                        linear = true;
                        L = P;
                    }
                    else
                    {
                        linear = false;
                        L = new Point(P.X - 10, P.Y);
                    }
                }
                else
                {
                    if (linear == false)
                    {
                        linear = true;
                        L = P;
                        R = P;
                    }
                    else
                    {
                        linear = false;
                        L = new Point(P.X - 10, P.Y);
                        R = new Point(P.X + 10, P.Y);
                    }
                }
               
                return;
            }

            if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                P2 = P;
                Split = !Split;
                return;
            }

            Mouse.Capture(this);
            sel = Selected.P;
        }


        private void rect2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Split == false)
                return;
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (linear2 == false)
                {
                    linear2 = true;
                    R = P2;
                }
                else
                {
                    linear2 = false;
                    R = new Point(P2.X + 10, P2.Y);
                }

                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                P = P2;
                Split = !Split;
                return;
            }

            Mouse.Capture(this);
            sel = Selected.P2;
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch(sel)
            {
                case Selected.P:
                    var point = Mouse.GetPosition(canvas);
                    var dif = point - P;
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        P = new Point(P.X, point.Y);
                    else
                        P = point;
                    //if (Split == false)
                    //    P2 = P;
                    //else
                    //    P2 = new Point(P.X, P2.Y);
                    //L += dif;
                    //R += dif;
                    break;
                case Selected.L:
                    L = Mouse.GetPosition(canvas);
                    break;
                case Selected.R:
                    R = Mouse.GetPosition(canvas);
                    break;
                case Selected.P2:
                    var point2 = Mouse.GetPosition(canvas);
                    var dif2 = point2 - P;
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        P2 = new Point(P2.X, point2.Y);
                    else
                        P2 = point2;
                    //if(Split)
                    //    P = new Point(P2.X, P.Y);
                    //L += dif;
                    //R += dif;
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
