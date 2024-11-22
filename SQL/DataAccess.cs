namespace sql_fetcher;
using System.Configuration;

public class DataAccess
{
    //TODO: Input correct ConnectionString linking to the SQL server
    private static readonly string ConnectionString = 
        "Server=project-software-3.database.microsoft.net;" +
        "Database=YourDatabaseName;" +
        "User ID=cloud-admin;" +
        "Password=very-good-password1;" +
        "Encrypt=True;" +
        "TrustServerCertificate=False;" +
        "Connection Timeout=30;";
    private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
    private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);

    public DataAccess() //Initializes queries in DataStorage.cs with the queries to be fetched
    {
        //TODO: Add all the queries for all the available options in AccesableData.cs
        foreach (Locations location in Enum.GetValues(typeof(Locations)))
        {
            DataStorage.Add(AccesableData.CurrentTemperature, location,
                $"SELECT TOP 1 temperature FROM table WHERE location = ${location} ORDER BY time DESC");  //placeholder query
            DataStorage.Add(AccesableData.CurrentHumidity, location,
                $"SELECT TOP 1 humidity FROM table WHERE location = ${location} ORDER BY time DESC");     //placeholder query
            DataStorage.Add(AccesableData.CurrentLight, location,
                $"SELECT TOP 1 light FROM table  WHERE location = ${location} ORDER BY time DESC");                                         //placeholder query
            DataStorage.Add(AccesableData.DayHumidity, location,
                $"SELECT humidity FROM  WHERE location = ${location} AND time > DATEADD(hour, DATEDIFF(hour, -24, GETDATE()), 0) ORDER BY time DESC"); //placeholder query
        }
    }

    public List<double> GetData(AccesableData name, Locations location) //Gets returns data (List<string>) of the name requested (i.e. current_temperature, current_humidity)
    {
        DataStorage.Reload();
        foreach(int i in DataStorage.Name)
        {
            if (DataStorage.Name[i] == name && DataStorage.Location[i] == location)
            {
                return DataStorage.Data[i];
            }
        }
        throw new ArgumentException($"Data for {name} at {location} not found");
    }
}