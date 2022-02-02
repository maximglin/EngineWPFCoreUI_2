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
using System.Linq;

namespace EngineWPFCoreUI
{
    /// <summary>
    /// Логика взаимодействия для FunctionDrawer.xaml
    /// </summary>
    public partial class FunctionDrawer : UserControl
    {
        public FunctionDrawer()
        {
            InitializeComponent();

            this.DataContext = vm;
            vm.AddFunction();
            vm.AddFunction();
        }

        FunctionsVM vm = new();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.Functions[0].AddKeyframe();
            vm.Functions[1].AddKeyframe();
        }
    }


    public class FunctionsVM : BaseViewModel
    {
        public class FunctionVM : BaseViewModel
        {
            public FunctionVM(FunctionsVM parent)
            {
                Parent = parent;
            }
            public FunctionsVM Parent { get; }
            public class KeyFrame : BaseViewModel
            {
                public FunctionVM Parent { get; }
                public KeyFrame(Point L, Point P, Point R, FunctionVM parent)
                {
                    l = L;
                    p = P;
                    r = R;
                    Parent = parent;
                }
                Point l, p, r;
                public Point L
                {
                    get => l;
                    set
                    {
                        if (value.X > this.P.X)
                            value = new Point(this.P.X, value.Y);

                        int i = Parent.keyframes.FindIndex(kf => kf == this);
                        if (i > 0 && i != -1)
                        {
                            if (value.X < Parent.keyframes[i - 1].P.X + float.Epsilon)
                                value = new Point(Parent.keyframes[i - 1].P.X + float.Epsilon, value.Y);
                        }


                        l = value;
                        Parent.OnPropertyChanged(nameof(Parent.Segments));
                        OnPropertyChanged(nameof(L));
                    }
                }
                public Point P
                {
                    get => p;
                    set
                    {
                        int i = Parent.keyframes.FindIndex(kf => kf == this);
                        if (i > 0 && i != -1)
                        {
                            if (value.X < Parent.keyframes[i - 1].P.X)
                                value = new Point(Parent.keyframes[i - 1].P.X, value.Y);

                            if (Parent.keyframes[i - 1].R.X > value.X - float.Epsilon)
                                Parent.keyframes[i - 1].R = new Point(value.X - float.Epsilon, Parent.keyframes[i - 1].R.Y);
                        }
                        if (i < Parent.keyframes.Count - 1 && i != -1)
                        {
                            if (value.X > Parent.keyframes[i + 1].P.X)
                                value = new Point(Parent.keyframes[i + 1].P.X, value.Y);

                            if (Parent.keyframes[i + 1].L.X < value.X + float.Epsilon)
                                Parent.keyframes[i + 1].L = new Point(value.X + float.Epsilon, Parent.keyframes[i + 1].L.Y);
                        }

                        var dif = value - p;
                        p = value;
                        L += dif;
                        R += dif;
                        Parent.OnPropertyChanged(nameof(Parent.Segments));
                        OnPropertyChanged(nameof(P));
                    }
                }
                public Point R
                {
                    get => r;
                    set
                    {
                        if (value.X < this.P.X)
                            value = new Point(this.P.X, value.Y);

                        int i = Parent.keyframes.FindIndex(kf => kf == this);
                        if (i < Parent.keyframes.Count - 1 && i != -1)
                        {
                            if (value.X > Parent.keyframes[i + 1].P.X - float.Epsilon)
                                value = new Point(Parent.keyframes[i + 1].P.X - float.Epsilon, value.Y);
                        }


                        r = value;
                        Parent.OnPropertyChanged(nameof(Parent.Segments));
                        OnPropertyChanged(nameof(R));
                    }
                }

                public bool RVisible => this != Parent.keyframes.Last();
                public bool LVisible => this != Parent.keyframes.First();
            }

            public class Segment : BaseViewModel
            {
                Point p1, r1, l2, p2;

                public Point P1
                {
                    get => p1;
                    set
                    {
                        p1 = value;
                        OnPropertyChanged(nameof(P1));
                    }
                }
                public Point R1
                {
                    get => r1;
                    set
                    {
                        r1 = value;
                        OnPropertyChanged(nameof(R1));
                    }
                }
                public Point L2
                {
                    get => l2;
                    set
                    {
                        l2 = value;
                        OnPropertyChanged(nameof(L2));
                    }
                }
                public Point P2
                {
                    get => p2;
                    set
                    {
                        p2 = value;
                        OnPropertyChanged(nameof(P2));
                    }
                }
            }

            List<KeyFrame> keyframes = new();

            public IReadOnlyList<KeyFrame> KeyFrames { get => keyframes.ToList(); }

            public IEnumerable<Segment> Segments
            {
                get
                {
                    List<Segment> segments = new();
                    for (int i = 0; i < keyframes.Count - 1; i++)
                        segments.Add(new Segment
                        {
                            P1 = keyframes[i].P,
                            R1 = keyframes[i].R,
                            L2 = keyframes[i + 1].L,
                            P2 = keyframes[i + 1].P
                        });
                    return segments;
                }
            }
            public void AddKeyframe()
            {
                double t = keyframes.Count > 0 ? keyframes.Last().P.X : 0;
                t += 50;
                keyframes.Add(new KeyFrame
                (
                    new Point(t - 10, 50),
                    new Point(t, 50),
                    new Point(t + 10, 50),
                    this
                ));
                OnPropertyChanged(nameof(KeyFrames));
                OnPropertyChanged(nameof(Segments));
            }

            Brush color;
            public Brush Color { get => color; set { color = value; OnPropertyChanged(nameof(Color)); } }
        }

        List<FunctionVM> functions = new();
        public IReadOnlyList<FunctionVM> Functions { get => functions.ToList(); }


        Brush[] brushes = new[]
        {
            Brushes.DarkOliveGreen,
            Brushes.DarkRed,
            Brushes.DarkKhaki,
            Brushes.DarkBlue,
            Brushes.DarkGreen,
            Brushes.DarkSeaGreen
        };
        int brush_i = 0;
        public void AddFunction()
        {
            var fvm = new FunctionVM(this);
            functions.Add(fvm);
            fvm.Color = brushes[brush_i % brushes.Length];

            brush_i++;
        }
    }
    
}
