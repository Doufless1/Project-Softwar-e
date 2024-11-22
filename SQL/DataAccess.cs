namespace sql_fetcher;
using System.Configuration;

public class DataAccess
{
    //TODO: Input correct ConnectionString linking to the SQL server
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //Placeholder SQL connection string
    private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
    private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);

    public DataAccess() //Initializes queries in DataStorage.cs with the queries to be fetched
    {
        //TODO: Add all the queries for all the available options in AccesableData.cs
        foreach (Locations location in Enum.GetValues(typeof(Locations)))
        {
            DataStorage.Add(AccesableData.CurrentTemperature,
                $"SELECT TOP 1 temperature FROM table WHERE location = ${location} ORDER BY time DESC");  //placeholder query
            DataStorage.Add(AccesableData.CurrentHumidity,
                $"SELECT TOP 1 humidity FROM table WHERE location = ${location} ORDER BY time DESC");     //placeholder query
            DataStorage.Add(AccesableData.CurrentLight,
                $"SELECT TOP 1 light FROM table  WHERE location = ${location} ORDER BY time DESC");                                         //placeholder query
            DataStorage.Add(AccesableData.DayHumidity,
                $"SELECT humidity FROM  WHERE location = ${location} AND time > DATEADD(hour, DATEDIFF(hour, -24, GETDATE()), 0) ORDER BY time DESC"); //placeholder query
        }
    }

    public List<double> GetData(AccesableData name) //Gets returns data (List<string>) of the name requested (i.e. current_temperature, current_humidity)
    {
        DataStorage.Reload();
        return DataStorage.Data[DataStorage.Name.IndexOf(name)];
    }
}