namespace sql_fetcher;
using System.Configuration;

public class DataAccess
{
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //Placeholder SQL connection string
    private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
    private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);

    public void InitializeQueries() //Initializes queries in DataStorage.cs with the queries to be fetched
    {
        DataStorage.Add("current_temperature", "SELECT TOP 1 temperature FROM temperature ORDER BY time DESC"); //placeholder query
        DataStorage.Add("current_humidity", "SELECT TOP 1 humidity FROM humidity ORDER BY time DESC"); //placeholder query
    }

    public List<string> GetData(string name) //Gets returns data (List<string>) of the name requested (i.e. current_temperature, current_humidity)
    {
        DataStorage.Reload();
        return DataStorage.Data[DataStorage.Name.IndexOf(name)];
    }
}