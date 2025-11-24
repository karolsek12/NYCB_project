using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCB_Project
{
    static class QueryHelper
    {
        public static double getEuclideanDist(dynamic x , dynamic y) 
        {
            //anonymous types used in linq complicate this a bit

            return Math.Sqrt(Math.Pow(x.Item1 - y.Item1,2)+ Math.Pow(x.Item2 - y.Item2,2));
        }
    }
}
