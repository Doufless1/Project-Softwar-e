using Microsoft.IdentityModel.Tokens;

namespace Project_Software_API.Properties.Backend.SQL
{

    namespace sql_fetcher
    {
        using Models;

        public class DataStorage
        {
            public List<AccesableData> Name { get; private set; }
            public List<List<double>> Data { get; private set; }
            private List<string> Query { get; set; }
            public List<string> Location { get; private set; }
            public List<int> DayFromNow { get; private set; }
            private readonly DataFetcher _dataFetcher;

            public DataStorage(DataFetcher dataFetcher) //Initialize components
            {
                _dataFetcher = dataFetcher;
                Name = new List<AccesableData>();
                Location = new List<string>();
                Query = new List<string>();
                Data = new List<List<double>>();
                DayFromNow = new List<int>();
            }

            public void
                Add(AccesableData name, int dayFromNow, string location,
                    string query) //This function adds a query to the list of queries to be fetched with corresponding name and fetches the data once.
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
                    DayFromNow.Add(dayFromNow);
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
    }
} 