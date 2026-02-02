﻿using System.Globalization;

namespace NYCB_Project;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Database
{
    public List<Trip> Trips;
    public List<Station> Stations;
    
    public List<Trip> TripsProp // property set up for json
    {
        get => Trips;
        set => Trips = value;
    }
    public List<Station> StationsProp // property set up for json
    {
        get => Stations;
        set => Stations = value;
    }
    

    public Database(string a) : this() // a - path of csv file to read
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
            if (!firstRead) // first line of a csv file includes names of the fields, this is not needed
            {
                firstRead = true;
                continue;
            }
            var vals = line.Split(toSplit); // getting all the values as string
            
            tr = new Trip( // parsing all the values related to the trip
                ParsingHelper.TrimQuotations(vals[0]), // id of the ride
                ParsingHelper.getRideableType(vals[1]), // type of the ride either classic or electric
                ParsingHelper.getDateTime(vals[2]), // trip start time
                ParsingHelper.getDateTime(vals[3]), // trip end time
                ParsingHelper.TrimQuotationsOrNull(vals[5]), // id of the start station
                ParsingHelper.TrimQuotationsOrNull(vals[7]), // id of the end station
                ParsingHelper.getTripType(vals[12]) // whether the ride was "casual" or "member"
                );
            
            Trips.Add(tr);

            st = new Station( // parsing the values related to the start station
                ParsingHelper.TrimQuotationsOrNull(vals[5]), //id of the station
                ParsingHelper.TrimQuotationsOrNull(vals[4]), //name of the station
                ParsingHelper.getDoubleOrNull(vals[8]), // latitude of the station
                ParsingHelper.getDoubleOrNull(vals[9]) // longitude of the station
            );
            
            Stations.Add(st);
            
            st = new Station( // parsing the values related to the end station
                ParsingHelper.TrimQuotationsOrNull(vals[7]), // id of the station
                ParsingHelper.TrimQuotationsOrNull(vals[6]), // name of the station
                ParsingHelper.getDoubleOrNull(vals[10]), // latitude of the station
                ParsingHelper.getDoubleOrNull(vals[11]) // longitude of the station
            );
            
            Stations.Add(st);

        }
        Stations = Stations.Distinct().ToList(); // of course some stations will repeat, distinct is used
        
    }
    
}