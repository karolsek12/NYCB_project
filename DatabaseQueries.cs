using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NYCB_Project
{
    static class DatabaseQueries
    {
        public static void RunQueries(this Database db) // elegant interface
        {
            List<string> queries = new List<string>{
                "Stations with no ID",
                "Top 5 Longest Trips",
                "Station where the ratio of trips started to trips ended was the biggest",
                "Trips which started and ended on a different day",
                "Average duration of a trip for a Classic and an Electric bike"
            };
            string c;

            queries.Add("Exit");


            while (true)
            {

                Console.WriteLine("Select Query:");

                for (int i = 0; i < queries.Count; i++)
                {
                    Console.WriteLine("${0}: {1}", i + 1, queries[i]);
                }
                c = Console.ReadLine()!;

                switch (c)
                {
                    case "1":
                        db.StationsWithNoId();
                        break;

                    case "2":
                        db.Top5LongestTrips();
                        break;

                    case "3":
                        db.BiggestRatioStartEnd();
                        break;

                    case "4":
                        db.TripsChangedDay();
                        break;

                    case "5":
                        db.AverageDurationByType();
                        break;

                    default:
                        if (c == Convert.ToString(queries.Count))
                        {
                            Console.Clear();
                            return;
                        }
                        break;
                }
                PressEnterToContinue();
                Console.Clear();
            }

        }

        public static void StationsWithNoId(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = stations
                .Where(s => s.StationId == null)
                .ToList();

            Console.WriteLine("Stations With No Id");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        public static void Top5LongestTrips(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = trips
                .Where(t => t.startStationId != null && t.endStationId != null) // removing trips with no station id
                .Join(stations, t => t.startStationId, s => s.StationId, (t, s) => new //joining with stations
                {
                    t.rideId,
                    t.startStationId,
                    s.StationName,
                    start_position = (s.Latitude, s.Longitude),
                    t.endStationId,
                })
                .Join(stations, t => t.endStationId, s => s.StationId, (t, s) => new //once again joining with stations to get end position
                {
                    t.rideId,
                    t.startStationId,
                    start_station_name = t.StationName,
                    t.start_position,
                    t.endStationId,
                    ene_station_name = s.StationName,
                    end_position = (s.Latitude, s.Longitude)
                })
                .Where(v => v.start_position.Latitude != null && v.start_position.Longitude != null &&
                v.end_position.Latitude != null && v.end_position.Longitude != null) // removing trips with unkown start/end positions
                .Select(v => new
                {
                    trip_data = v,
                    trip_length = QueryHelper.GetGreatCircleDist(v.start_position, v.end_position)
                })
                .OrderByDescending(v => v.trip_length)
                .Select(v => new //cleaning up for display
                {
                    v.trip_data.rideId,
                    v.trip_data.startStationId,
                    v.trip_data.start_station_name,
                    start_position = new { v.trip_data.start_position.Latitude, v.trip_data.start_position.Longitude },
                    v.trip_data.endStationId,
                    v.trip_data.ene_station_name,
                    end_position = new { v.trip_data.end_position.Latitude, v.trip_data.end_position.Longitude },
                    trip_length_km = v.trip_length
                })
                .Take(5)
                .ToList();

            Console.WriteLine("Top 5 Longest Trips");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        public static void BiggestRatioStartEnd(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = stations
                .Where(s => s.StationId != null) // removing stations with no id
                .GroupJoin(trips, s => s.StationId, t => t.startStationId, (s, t) => new // counting amount of trips starting in a station
                {
                    s,
                    count = t.Count()
                })
                .GroupJoin(trips, s => s.s.StationId, t => t.endStationId, (s, t) => new // counting amount of trips ending in a station
                {
                    s.s,
                    start_count = s.count,
                    end_count = t.Count()
                })
                .Where(v => v.end_count != 0) // making sure not to divide by 0
                .Select(v => new
                {
                    station = v.s,
                    ratio = (double)v.start_count / v.end_count,
                    v.start_count,
                    v.end_count

                })
                .OrderByDescending(v => v.ratio)
                .Take(1)
                .ToList();


            Console.WriteLine("Station where the ratio of trips started to trips ended was the biggest");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        public static void TripsChangedDay(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = trips
                .Where(t => t.startedAt.Day != t.endedAt.Day) // date is saved as the Datetime class, so isolating the day is easy
                .ToList();


            Console.WriteLine("Trips which started and ended on a different day");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        public static void AverageDurationByType(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = trips
                .Select(t => new
                {
                    t,
                    time = t.endedAt.Ticks - t.startedAt.Ticks, // measuring in ticks gives the best precision
                })
                .GroupBy(v => v.t.rideableType, v => v.time)
                .Select(v => new
                {
                    ride_type = v.Key,
                    average_trip_duration_h = (double)v.Average(x => x) / (10_000_000) / 60 // diving by 10000000 * 60 to get the duration in hours
                })
                .ToList();


            Console.WriteLine("Average duration of a trip for a Classic and an Electric bike");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        private static void PressEnterToContinue()
        {
            Console.WriteLine("Press ENTER key to continue...");
            Console.ReadLine();
        }

        public static void DisplayQueryResults<T>(T query)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new JsonStringEnumConverter());

            var json = JsonSerializer.Serialize(query, options);

            Console.WriteLine(json);
        }

    }
}