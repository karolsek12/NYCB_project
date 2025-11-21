namespace NYCB_Project;

using System.Text.Json.Serialization;
public enum TripType
{
    Member,
    Casual
}

public enum RideableType
{
    Electric,
    Classic
    
}


public record Trip(
    string rideId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] 
    RideableType rideableType,
    DateTime startedAt,
    DateTime endedAt,
    string? startStationId,
    string? endStationId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] 
    TripType tripType
    
    );

public record Station(
    string? StationId,
    string? StationName,
    double? Longitude,
    double? Latitude
  
);
    
    
    