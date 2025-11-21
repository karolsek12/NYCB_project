using System.Globalization;

namespace NYCB_Project;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Database
{
    public List<Trip> Trips;
    public List<Station> Stations;
    
    public List<Trip> TripsProp
    {
        get => Trips;
        set => Trips = value;
    }
    public List<Station> StationsProp
    {
        get => Stations;
        set => Stations = value;
    }
    

    public Database(string a) : this()
    {
        InitDb(a);
    }

    public Database(List<Trip> t, List<Station> s)
    {
        Trips = t;
        Stations = s;
    }

    public Database()
    {
        Trips = new List<Trip>();
        Stations = new List<Station>();
    }

    private void InitDb(string a)
    {
        IEnumerable<string> lines = File.ReadLines(a);
        bool firstRead = false;
        Trip tr;
        Station st;
        char[] toSplit = {','};

        foreach (string line in lines)
        {
            if (!firstRead)
            {
                firstRead = true;
                continue;
            }
            var vals = line.Split(toSplit);
            
            tr = new Trip(
                getRideId(vals[0]), 
                getRideableType(vals[1]),
                getDateTime(vals[2]), 
                getDateTime(vals[3]),
                getStationId(vals[5]),
                getStationId(vals[7]),
                getTripType(vals[12])
                );
            
            Trips.Add(tr);

            st = new Station(
                getStationId(vals[5]),
                getStationName(vals[4]),
                getPos(vals[8]),
                getPos(vals[9])
            );
            
            Stations.Add(st);
            
            st = new Station(
                getStationId(vals[7]),
                getStationName(vals[6]),
                getPos(vals[10]),
                getPos(vals[11])
            );
            
            Stations.Add(st);

        }
        Stations = Stations.Distinct().ToList();
        
    }

    private string getRideId(string s)
    {
        return s.Trim('"');
    }
    
    private RideableType getRideableType(string s)
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

    private DateTime getDateTime(string s)
    {
        DateTime dt;
        char[] toSplit = {'"',' ','-',':','.'};
        var vals = s.Split(toSplit,StringSplitOptions.RemoveEmptyEntries);

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

    private string? getStationId(string s)
    {
        if (string.IsNullOrEmpty(s))
            return null;

        return s.Trim('"');
    }

    private TripType getTripType(string s)
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

    private string? getStationName(string s)
    {
        if (string.IsNullOrEmpty(s))
            return null;

        return s.Trim('"');
    }

    private double? getPos(string a)
    {
        
        if (string.IsNullOrEmpty(a))
            return null;
        
        return double.Parse(a,CultureInfo.InvariantCulture);
    }
    
    public void RunQueries()
    {
        throw new NotImplementedException();
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