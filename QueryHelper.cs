
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCB_Project
{
    static class QueryHelper
    {

        public static double Haversine(double x) // calculating haversine(x)
        {

            return (1 - Math.Cos(x)) / 2;
        }

        public static double GetGreatCircleDist(dynamic x, dynamic y) //calculating great circle distance, used in a query
        {
            //anonymous types used in linq complicate this a bit
            double R = 6371; //earth radius in kilometers
            double lonDiff = x.Item1 - y.Item1;
            double latDiff = x.Item2 - y.Item2;

            lonDiff *= Math.PI / 180;
            latDiff *= Math.PI / 180;

            double havTheta = Haversine(latDiff) + Math.Cos(x.Item2) * Math.Cos(y.Item2) * Haversine(lonDiff);

            double theta = 2 * Math.Asin(Math.Sqrt(havTheta));

            return R * theta;
        }
    }
}
