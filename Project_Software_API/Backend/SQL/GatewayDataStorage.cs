namespace Project_Software_API.Properties.Backend.SQL
{

    namespace sql_fetcher
    {
        using Models;

        public class GatewayDataStorage
        {
            public List<AccesableData> Name { get; private set; }
            public List<List<double>> Data { get; private set; }
            public List<string> Query { get; private set; }
            public List<string> Location { get; private set; }
            public List<string> Gateways { get; private set; }

            private readonly DataFetcher _dataFetcher;

            public GatewayDataStorage(DataFetcher dataFetcher) //Initialize components
            {
                _dataFetcher = dataFetcher;
                Name = new List<AccesableData>();
                Location = new List<string>();
                Query = new List<string>();
                Data = new List<List<double>>();
                Gateways = new List<string>();
            }

            public void Add(AccesableData name, string location, string query)
            {
                try
                {
                    // Fetch all gateways and their IDs in one query
                    string gatewaysQuery = $@"
            SELECT gatewayID 
            FROM gateway 
            WHERE deviceID = '{location}'
            ORDER BY Latitude;";

                    var gatewayIDs = _dataFetcher.FetchStringList(gatewaysQuery);

                    // For each gateway, prepare the query and fetch data in bulk
                    foreach (var gatewayID in gatewayIDs)
                    {
                        Name.Add(name);
                        Location.Add(location);
                        Gateways.Add(gatewayID);

                        // Replace 'gateway_input' with the current gatewayID
                        string query_ = query.Replace("gateway_input", gatewayID);
                        Query.Add(query_);

                        // Fetch data for this query and store it
                        Data.Add(_dataFetcher.FetchData(query_));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch data for {query}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}