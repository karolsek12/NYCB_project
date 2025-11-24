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
        public static void RunQueries(this Database db)
        {
            List<string> queries = new List<string>{ "Stations with no ID",
                "Top 5 Longest Trips"
            };
            string c;

            queries.Add("Exit");


            while (true)
            {

                Console.WriteLine("Select Query:");

                for (int i = 0; i < queries.Count; i++)
                {
                    Console.WriteLine("${0}: {1}",i+1, queries[i]);
                }
                c = Console.ReadLine()!;

                switch (c)
                {
                    case "1":
                        db.StationsWithNoId();
                        PressEnterToContinue();
                    break;

                    case "2":
                        db.Top5LongestTrips();
                        PressEnterToContinue();
                    break;

                    default:
                        if(c == Convert.ToString(queries.Count))
                        {
                            Console.Clear();
                            return;
                        }
                        break;
                }
                
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

            Console.WriteLine("StationsWithNoId");
            DisplayQueryResults(queryResult);
            Console.WriteLine();
        }

        public static void Top5LongestTrips(this Database db)
        {
            var trips = db.Trips;
            var stations = db.Stations;

            var queryResult = trips
                .Where(t => t.startStationId != null && t.endStationId != null)
                .Join(stations, t => t.startStationId, s => s.StationId, (t, s) => new
                {
                    t.rideId,
                    t.startStationId,
                    s.StationName,
                    start_position = (s.Latitude, s.Longitude),
                    t.endStationId,
                })
                .Join(stations,t=>t.endStationId, s=>s.StationId, (t,s) => new
                {
                    t.rideId,
                    t.startStationId,
                    start_station_name = t.StationName,
                    t.start_position,
                    t.endStationId,
                    ene_station_name = s.StationName,
                    end_position = (s.Latitude, s.Longitude)
                })
                .Where(v=> v.start_position.Latitude!= null && v.start_position.Longitude!=null &&
                v.end_position.Latitude != null && v.end_position.Longitude != null)
                .Select(v => new
                {
                    trip_data = v,
                    trip_length = QueryHelper.getEuclideanDist(v.start_position, v.end_position)
                })
                .OrderByDescending(v=>v.trip_length)
                .Select(v => new
                {
                    v.trip_data.rideId,
                    v.trip_data.startStationId,
                    v.trip_data.start_station_name,
                    start_position = new { v.trip_data.start_position.Latitude, v.trip_data.start_position.Longitude},
                    v.trip_data.endStationId,
                    v.trip_data.ene_station_name,
                    end_position = new { v.trip_data.end_position.Latitude, v.trip_data.end_position.Longitude },
                    v.trip_length
                })
                .Take(5)
                .ToList();

            Console.WriteLine("Top5LongestTrips");
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
