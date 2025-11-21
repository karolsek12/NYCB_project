using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NYCB_Project;

class Program
{
    static void Main()
    {
        Database db;
        
        if (File.Exists("Database.json")) //parsing json seems to be faster. I think???
        {
            db = JsonSerializer.Deserialize<Database>(File.ReadAllText("Database.json"))!;
        }
        else
        {
            // if json doesnt exist, open csv file, parse and create a database class
            // then serialise that class for future uses
            db = new Database("JC-202510-citibike-tripdata.csv");
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            File.WriteAllText("Database.json", JsonSerializer.Serialize(db,options));
            
        }
        
        
        db.RunQueries();
        
    }
    
}

