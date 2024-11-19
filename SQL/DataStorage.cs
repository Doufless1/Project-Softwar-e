using Microsoft.IdentityModel.Tokens;

namespace sql_fetcher;

public class DataStorage
{
    public List<string> Name { get; private set; }
    public List<List<string>> Data { get; private set; }
    private List<string> Query { get; set; }
    private readonly DataFetcher _dataFetcher;
    
    public DataStorage(DataFetcher dataFetcher) //Initialize components
    {
        _dataFetcher = dataFetcher;
        Name = new List<string>();
        Query = new List<string>();
        Data = new List<List<string>>();
    }
    
    public void Add (string name, string query) //This function adds a query to the list of queries to be fetched with corresponding name and fetches the data once.
    {
        if (name.IsNullOrEmpty())
        {
            throw new ArgumentException("Name cannot be empty");
        } if (query.IsNullOrEmpty())
        {
            throw new ArgumentException("Query cannot be empty");
        }

        try
        {
            Name.Add(name);
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