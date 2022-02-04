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

        FunctionVM vm = new();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (vm.Sections[0] as FunctionVM.BezierVM).AddKeyframe();
            (vm.Sections[1] as FunctionVM.BezierVM).AddKeyframe();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.Tend = 10;
        }
    }


    public class FunctionVM : BaseViewModel
    {
        public interface ISectionVM
        {
            IEnumerable<BezierVM.KeyFrame> KeyFrames { get; }
            public IEnumerable<BezierVM.Segment> Segments { get; }
            void OnScaleChange((double Tst, double Tend, double Ymin, double Ymax) oldscale, (double Tst, double Tend, double Ymin, double Ymax) newscale);
        }
        public class BezierVM : BaseViewModel, ISectionVM
        {
            public BezierVM(FunctionVM parent)
            {
                Parent = parent;
            }
            public FunctionVM Parent { get; }
            public class KeyFrame : BaseViewModel
            {
                public BezierVM Parent { get; }
                public KeyFrame(Point L, Point P, Point R, BezierVM parent)
                {
                    split = false;
                    l = L;
                    p = P;
                    p2 = P;
                    r = R;
                    Parent = parent;
                }
                public KeyFrame(Point L, Point P, Point P2, Point R, BezierVM parent)
                {
                    split = true;
                    l = L;
                    p = P;
                    p2 = P2;
                    r = R;
                    Parent = parent;
                }
                Point l, p, r, p2;
                bool split = false;

                public bool Split { get => split; 
                    set
                    {
                        split = value;
                        OnPropertyChanged(nameof(Split));
                    } 
                }

                public Point P2
                {
                    get => p2;
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

                        var dif = value - p2;
                        p2 = value;
                        R += dif;

                        if(Split)
                        {
                            var old = p;
                            p = new Point(p2.X, p.Y);
                            var dif2 = p - old;
                            L += dif2;
                        }
                        else
                        {
                            p = p2;
                            L += dif;
                        }

                        
                        Parent.OnPropertyChanged(nameof(Parent.Segments));
                        OnPropertyChanged(nameof(P2));
                        OnPropertyChanged(nameof(P));
                    }
                }


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

                        if(Split)
                        {
                            var old = p2;
                            p2 = new Point(p.X, p2.Y);
                            var dif2 = p2 - old;
                            R += dif2;
                        }
                        else
                        {
                            p2 = p;
                            R += dif;
                        }
                        
                            
                        Parent.OnPropertyChanged(nameof(Parent.Segments));
                        OnPropertyChanged(nameof(P));
                        OnPropertyChanged(nameof(P2));
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

            public IEnumerable<KeyFrame> KeyFrames { get => keyframes.ToList(); }

            public IEnumerable<Segment> Segments
            {
                get
                {
                    List<Segment> segments = new();
                    for (int i = 0; i < keyframes.Count - 1; i++)
                        segments.Add(new Segment
                        {
                            P1 = keyframes[i].P2,
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

            
            public void OnScaleChange((double Tst, double Tend, double Ymin, double Ymax) oldscale, (double Tst, double Tend, double Ymin, double Ymax) newscale)
            {
                List<KeyFrame> newkeyframes = new();
                foreach(var kf in keyframes)
                {
                    var P = new Point(
                        Foo.AtoB(kf.P.X, newscale.Tst,  newscale.Tend, oldscale.Tst,  oldscale.Tend),
                        Foo.AtoB(kf.P.Y, newscale.Ymin, newscale.Ymax, oldscale.Ymin, oldscale.Ymax)
                        );
                    var R = new Point(
                        Foo.AtoB(kf.R.X, newscale.Tst,  newscale.Tend, oldscale.Tst,  oldscale.Tend),
                        Foo.AtoB(kf.R.Y, newscale.Ymin, newscale.Ymax, oldscale.Ymin, oldscale.Ymax)
                        );
                    var L = new Point(
                        Foo.AtoB(kf.L.X, newscale.Tst,  newscale.Tend, oldscale.Tst,  oldscale.Tend),
                        Foo.AtoB(kf.L.Y, newscale.Ymin, newscale.Ymax, oldscale.Ymin, oldscale.Ymax)
                        );
                    var P2 = new Point(
                        Foo.AtoB(kf.P2.X, newscale.Tst,  newscale.Tend, oldscale.Tst,  oldscale.Tend),
                        Foo.AtoB(kf.P2.Y, newscale.Ymin, newscale.Ymax, oldscale.Ymin, oldscale.Ymax)
                        );

                    KeyFrame nkf;
                    if (kf.Split)
                        nkf = new KeyFrame(L, P, P2, R, this);
                    else
                        nkf = new KeyFrame(L, P, R, this);

                    newkeyframes.Add(nkf);
                }
                keyframes = newkeyframes;
                OnPropertyChanged(nameof(KeyFrames));
                OnPropertyChanged(nameof(Segments));
            }
        }

        public class ConstVM : BaseViewModel, ISectionVM
        {
            public IEnumerable<BezierVM.KeyFrame> KeyFrames => Array.Empty<BezierVM.KeyFrame>();

            public IEnumerable<BezierVM.Segment> Segments => Array.Empty<BezierVM.Segment>();

            public void OnScaleChange((double Tst, double Tend, double Ymin, double Ymax) oldscale, (double Tst, double Tend, double Ymin, double Ymax) newscale)
            {
                throw new NotImplementedException();
            }
        }

        void UpdateScale(double Tst, double Tend, double Ymin, double Ymax)
        {
            foreach (var f in sections)
                f.OnScaleChange((this.Tstart, this.Tend, this.Ymin, this.Ymax), (Tst, Tend, Ymin, Ymax));
        }
        double tstart = 0, tend = 20, ymin = -50, ymax = 50;
        public double Tstart { get => tstart; set { UpdateScale(value, Tend, Ymin, Ymax); tstart = value; OnPropertyChanged(nameof(Tstart)); } }
        public double Tend { get => tend; set { UpdateScale(Tstart, value, Ymin, Ymax); tend = value; OnPropertyChanged(nameof(Tend)); } }

        public double Ymin { get => ymin; set { UpdateScale(Tstart, Tend, value, Ymax); ymin = value; OnPropertyChanged(nameof(Ymin)); } }
        public double Ymax { get => ymax; set { UpdateScale(Tstart, Tend, Ymin, value); ymax = value; OnPropertyChanged(nameof(Ymax)); } }


        List<ISectionVM> sections = new();
        public IReadOnlyList<ISectionVM> Sections { get => sections.ToList(); }









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
            var fvm = new BezierVM(this);
            sections.Add(fvm);
            fvm.Color = brushes[brush_i % brushes.Length];

            brush_i++;
        }
    }
    
}
