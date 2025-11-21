using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCB_Project
{
    static class ParsingHelper
    {
        public static string TrimQuotations(string s)
        {
            return s.Trim('"');
        }

        public static string? TrimQuotationsOrNull(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            return s.Trim('"');
        }

        public static RideableType getRideableType(string s)
        {
            switch (s)
            {
                case "\"electric_bike\"":
                    return RideableType.Electric;

                case "\"classic_bike\"":
                    return RideableType.Classic;

                default:
                    throw new Exception("Unknown rideable type\n");
            }
        }

        public static DateTime getDateTime(string s)
        {
            DateTime dt;
            char[] toSplit = { '"', ' ', '-', ':', '.' };
            var vals = s.Split(toSplit, StringSplitOptions.RemoveEmptyEntries);

            dt = new DateTime(
                Convert.ToInt32(vals[0]),
                Convert.ToInt32(vals[1]),
                Convert.ToInt32(vals[2]),
                Convert.ToInt32(vals[3]),
                Convert.ToInt32(vals[4]),
                Convert.ToInt32(vals[5]),
                Convert.ToInt32(vals[6])
            );

            return dt;
        }

        public static TripType getTripType(string s)
        {
            switch (s)
            {
                case "\"member\"":
                    return TripType.Member;

                case "\"casual\"":
                    return TripType.Casual;

                default:
                    throw new Exception("Unknown trip type\n");
            }
        }
        public static double? getDoubleOrNull(string a)
        {

            if (string.IsNullOrEmpty(a))
                return null;

            return double.Parse(a, CultureInfo.InvariantCulture);
        }

    }
}
