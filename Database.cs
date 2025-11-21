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
                ParsingHelper.TrimQuotations(vals[0]), 
                ParsingHelper.getRideableType(vals[1]),
                ParsingHelper.getDateTime(vals[2]), 
                ParsingHelper.getDateTime(vals[3]),
                ParsingHelper.TrimQuotationsOrNull(vals[5]),
                ParsingHelper.TrimQuotationsOrNull(vals[7]),
                ParsingHelper.getTripType(vals[12])
                );
            
            Trips.Add(tr);

            st = new Station(
                ParsingHelper.TrimQuotationsOrNull(vals[5]),
                ParsingHelper.TrimQuotationsOrNull(vals[4]),
                ParsingHelper.getDoubleOrNull(vals[8]),
                ParsingHelper.getDoubleOrNull(vals[9])
            );
            
            Stations.Add(st);
            
            st = new Station(
                ParsingHelper.TrimQuotationsOrNull(vals[7]),
                ParsingHelper.TrimQuotationsOrNull(vals[6]),
                ParsingHelper.getDoubleOrNull(vals[10]),
                ParsingHelper.getDoubleOrNull(vals[11])
            );
            
            Stations.Add(st);

        }
        Stations = Stations.Distinct().ToList();
        
    }
    
}