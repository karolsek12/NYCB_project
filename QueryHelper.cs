using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCB_Project
{
    static class QueryHelper
    {
        public static double getEuclideanDist((double?, double?) x, (double?, double?) y)
        {
            (double, double) xc = ((double, double))x;
            (double, double) yc = ((double, double))y;

            return Math.Sqrt(Math.Pow(xc.Item1 - yc.Item1,2)+ Math.Pow(xc.Item2 - yc.Item2,2));
        }
    }
}
