using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineWPFCoreUI
{
    public static class Foo
    {
        static public double AtoB(double APoint, double Ast, double Aend, double Bst, double Bend)
        {
            APoint -= Ast;
            APoint /= (Aend - Ast);
            APoint *= (Bend - Bst);
            APoint += Bst;
            return APoint;
        }
    }
}
