using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineWPFCoreUI
{
    interface IFunction
    {
        float GetValue(float t);
    }

    interface FunctionSection : IFunction
    {
        public float Tstart { get; }
        public float Tend { get; }

    }

    class BezierFunctionSection : FunctionSection
    {
        public BezierFunctionSection(IEnumerable<KeyFrame> keyFrames)
        {
            SetKeyframes(keyFrames);
        }

        public struct KeyFrame
        {
            public PointF L, R;
            public float Px, PLy, PRy;
            public PointF PL { get => new PointF(Px, PLy); set { Px = value.X; PLy = value.Y; } }
            public PointF PR { get => new PointF(Px, PRy); set { Px = value.X; PRy = value.Y; } }
        }
        class KeyFrameComparer : IComparer<KeyFrame>
        {
            public int Compare(KeyFrame a, KeyFrame b)
            {
                return a.PL.X.CompareTo(b.PL.X);
            }
        }

        List<KeyFrame> keyframes = new();



        struct CubicSegment : IFunction
        {
            public float tst, tend, a, b, c, d;

            public CubicSegment(PointF p1, PointF r1, PointF l2, PointF p2)
            {
                tst = p1.X; tend = p2.X;

                List<PointF> points = new();

                Func<float, float, float, float, float, float> bezier = (t, P1, P2, P3, P4) =>
                {
                    return (1 - t) * (1 - t) * (1 - t) * P1 +
                    3 * (1 - t) * (1 - t) * t * P2 +
                    3 * (1 - t) * t * t * P3 +
                    t * t * t * P4;
                };

                Func<float, float> bezierx = (t) =>
                {
                    return bezier(t, p1.X, r1.X, l2.X, p2.X);
                };
                Func<float, float> beziery = (t) =>
                {
                    return bezier(t, p1.Y, r1.Y, l2.Y, p2.Y);
                };

                const int pn = 4;
                float dt = 1f / pn;
                for (int i = 0; i < pn; i++)
                {
                    points.Add(new PointF(bezierx(0f + dt * i), beziery(0f + dt * i)));
                }

                var n = (float)points.Count;
                var pointsx = points.Select(p => p.X);
                var pointsy = points.Select(p => p.Y);


                var x0 = points[0].X; var x03 = x0 * x0 * x0; var x02 = x0 * x0;
                var x1 = points[1].X; var x13 = x1 * x1 * x1; var x12 = x1 * x1;
                var x2 = points[2].X; var x23 = x2 * x2 * x2; var x22 = x2 * x2;
                var x3 = points[3].X; var x33 = x3 * x3 * x3; var x32 = x3 * x3;

                var y0 = points[0].Y;
                var y1 = points[1].Y;
                var y2 = points[2].Y;
                var y3 = points[3].Y;


                a = (x1 * x22 * y0 - x12 * x2 * y0 - x1 * x32 * y0 + x2 * x32 * y0 + x12 * x3 * y0 - x22 * x3 * y0 - x0 * x12 * y3 + x02 * x1 * y3 + x0 * x22 * y3 - x1 * x22 * y3 - x02 * x2 * y3 + x12 * x2 * y3 - x0 * x22 * y1 + x02 * x2 * y1 + x0 * x32 * y1 - x2 * x32 * y1 - x02 * x3 * y1 + x22 * x3 * y1 + x0 * x12 * y2 - x02 * x1 * y2 - x0 * x32 * y2 + x1 * x32 * y2 + x02 * x3 * y2 - x12 * x3 * y2) / (x0 * x12 * x23 - x02 * x1 * x23 - x0 * x13 * x22 + x03 * x1 * x22 + x02 * x13 * x2 - x03 * x12 * x2 - x0 * x12 * x33 + x02 * x1 * x33 + x0 * x22 * x33 - x1 * x22 * x33 - x02 * x2 * x33 + x12 * x2 * x33 + x0 * x13 * x32 - x03 * x1 * x32 - x0 * x23 * x32 + x1 * x23 * x32 + x03 * x2 * x32 - x13 * x2 * x32 - x02 * x13 * x3 + x03 * x12 * x3 + x02 * x23 * x3 - x12 * x23 * x3 - x03 * x22 * x3 + x13 * x22 * x3);
                b = (-x1 * x23 * y0 + x13 * x2 * y0 + x1 * x33 * y0 - x2 * x33 * y0 - x13 * x3 * y0 + x23 * x3 * y0 + x0 * x13 * y3 - x03 * x1 * y3 - x0 * x23 * y3 + x1 * x23 * y3 + x03 * x2 * y3 - x13 * x2 * y3 + x0 * x23 * y1 - x03 * x2 * y1 - x0 * x33 * y1 + x2 * x33 * y1 + x03 * x3 * y1 - x23 * x3 * y1 - x0 * x13 * y2 + x03 * x1 * y2 + x0 * x33 * y2 - x1 * x33 * y2 - x03 * x3 * y2 + x13 * x3 * y2) / (x0 * x12 * x23 - x02 * x1 * x23 - x0 * x13 * x22 + x03 * x1 * x22 + x02 * x13 * x2 - x03 * x12 * x2 - x0 * x12 * x33 + x02 * x1 * x33 + x0 * x22 * x33 - x1 * x22 * x33 - x02 * x2 * x33 + x12 * x2 * x33 + x0 * x13 * x32 - x03 * x1 * x32 - x0 * x23 * x32 + x1 * x23 * x32 + x03 * x2 * x32 - x13 * x2 * x32 - x02 * x13 * x3 + x03 * x12 * x3 + x02 * x23 * x3 - x12 * x23 * x3 - x03 * x22 * x3 + x13 * x22 * x3);
                c = (x12 * x23 * y0 - x13 * x22 * y0 - x12 * x33 * y0 + x22 * x33 * y0 + x13 * x32 * y0 - x23 * x32 * y0 - x02 * x13 * y3 + x03 * x12 * y3 + x02 * x23 * y3 - x12 * x23 * y3 - x03 * x22 * y3 + x13 * x22 * y3 - x02 * x23 * y1 + x03 * x22 * y1 + x02 * x33 * y1 - x22 * x33 * y1 - x03 * x32 * y1 + x23 * x32 * y1 + x02 * x13 * y2 - x03 * x12 * y2 - x02 * x33 * y2 + x12 * x33 * y2 + x03 * x32 * y2 - x13 * x32 * y2) / (x0 * x12 * x23 - x02 * x1 * x23 - x0 * x13 * x22 + x03 * x1 * x22 + x02 * x13 * x2 - x03 * x12 * x2 - x0 * x12 * x33 + x02 * x1 * x33 + x0 * x22 * x33 - x1 * x22 * x33 - x02 * x2 * x33 + x12 * x2 * x33 + x0 * x13 * x32 - x03 * x1 * x32 - x0 * x23 * x32 + x1 * x23 * x32 + x03 * x2 * x32 - x13 * x2 * x32 - x02 * x13 * x3 + x03 * x12 * x3 + x02 * x23 * x3 - x12 * x23 * x3 - x03 * x22 * x3 + x13 * x22 * x3);
                d = (-x1 * x22 * x33 * y0 + x12 * x2 * x33 * y0 + x1 * x23 * x32 * y0 - x13 * x2 * x32 * y0 - x12 * x23 * x3 * y0 + x13 * x22 * x3 * y0 + x0 * x12 * x23 * y3 - x02 * x1 * x23 * y3 - x0 * x13 * x22 * y3 + x03 * x1 * x22 * y3 + x02 * x13 * x2 * y3 - x03 * x12 * x2 * y3 + x0 * x22 * x33 * y1 - x02 * x2 * x33 * y1 - x0 * x23 * x32 * y1 + x03 * x2 * x32 * y1 + x02 * x23 * x3 * y1 - x03 * x22 * x3 * y1 - x0 * x12 * x33 * y2 + x02 * x1 * x33 * y2 + x0 * x13 * x32 * y2 - x03 * x1 * x32 * y2 - x02 * x13 * x3 * y2 + x03 * x12 * x3 * y2) / (x0 * x12 * x23 - x02 * x1 * x23 - x0 * x13 * x22 + x03 * x1 * x22 + x02 * x13 * x2 - x03 * x12 * x2 - x0 * x12 * x33 + x02 * x1 * x33 + x0 * x22 * x33 - x1 * x22 * x33 - x02 * x2 * x33 + x12 * x2 * x33 + x0 * x13 * x32 - x03 * x1 * x32 - x0 * x23 * x32 + x1 * x23 * x32 + x03 * x2 * x32 - x13 * x2 * x32 - x02 * x13 * x3 + x03 * x12 * x3 + x02 * x23 * x3 - x12 * x23 * x3 - x03 * x22 * x3 + x13 * x22 * x3);
            }

            public float GetValue(float t)
            {
                return a * t * t * t + b * t * t + c * t + d;
            }
        }
        List<CubicSegment> cubicsegments = new();
        const float epsilon = 0.00001f;
        public void SetKeyframes(IEnumerable<KeyFrame> keyFrames)
        {
            keyframes = keyFrames.ToList();
            keyframes.Sort(new KeyFrameComparer());
            
            bool f = true;
            while(f) //move keyframes which are too close to each other
            {
                f = false;
                for (int i = keyframes.Count - 1; i > 0; i--)
                {
                    if (MathF.Abs(keyframes[i].Px - keyframes[i - 1].Px) > epsilon)
                    {
                        var tmp = keyframes[i];
                        tmp.Px += epsilon;
                        keyframes[i] = tmp;
                        f = true; 
                    }
                }
            }
            

            cubicsegments.Clear();
            for (int i = 0; i < keyframes.Count - 1; i++)
                cubicsegments.Add(new CubicSegment(
                    keyframes[i].PR, keyframes[i].R,
                    keyframes[i+1].L, keyframes[i+1].PL
                    ));
        }


        public float Tstart => keyframes.Count > 0 ? keyframes.First().Px : 0f;
        public float Tend => keyframes.Count > 0 ? keyframes.Last().Px : 0f;

        public float GetValue(float t)
        {
            if(keyframes.Count > 0)
            {
                if (t <= keyframes.First().Px)
                    return keyframes.First().PL.Y;
                if (t >= keyframes.Last().Px)
                    return keyframes.Last().PR.Y;


                for (int i = 0; i < cubicsegments.Count; i++)
                    if (t > cubicsegments[i].tst)
                        return cubicsegments[i].GetValue(t);
            }

            return 0f;
        }

    }

    class ConstantFunctionSection : FunctionSection
    {
        public float Tstart { get; set; }
        public float Tend { get; set; }
        public ConstantFunctionSection(float value, float tst, float tend)
        {
            Value = value;
            Tstart = tst;
            Tend = tend;
        }
        public float Value { get; set; }
        public float GetValue(float t)
        {
            return Value;
        }
    }

    class Function
    {

        List<FunctionSection> sections = new();




    }
}
