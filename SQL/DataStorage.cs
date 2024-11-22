using Microsoft.IdentityModel.Tokens;

namespace sql_fetcher;

public class DataStorage
{
    public List<AccesableData> Name { get; private set; }
    public List<List<double>> Data { get; private set; }
    private List<string> Query { get; set; }
    public List<Locations> Location { get; private set; }
    private readonly DataFetcher _dataFetcher;
    
    public DataStorage(DataFetcher dataFetcher) //Initialize components
    {
        _dataFetcher = dataFetcher;
        Name = new List<AccesableData>();
        Location = new List<Locations>();
        Query = new List<string>();
        Data = new List<List<double>>();
    }
    
    public void Add (AccesableData name, Locations location, string query) //This function adds a query to the list of queries to be fetched with corresponding name and fetches the data once.
    {
        if (query.IsNullOrEmpty())
        {
            throw new ArgumentException("Query cannot be empty");
        }

        try
        {
            Name.Add(name);
            Location.Add(location);
            Query.Add(query);
            Data.Add(_dataFetcher.FetchData(query));
        }
        catch
        {
            throw new Exception($"Failed to fetch data for {query}");
        }
    }

    public void Reload() //Reloads the data for all the queries in the list
    {
        for (int i = 0; i < Query.Count; i++)
        {
            Data[i] = _dataFetcher.FetchData(Query[i]);
        }
    }
}