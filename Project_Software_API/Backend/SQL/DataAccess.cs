using System.Runtime.InteropServices.ComTypes;

namespace Project_Software_API.Properties.Backend.SQL
{
    using Models;

    namespace sql_fetcher
    {

        public class DataAccess
        {
            
            
            private static readonly string ConnectionString =
                "Server=tcp:group13.database.windows.net,1433;" +
                "Database=weather_state;" +
                "User ID=cloudadmin;" +
                "Password=Group13pass;" +
                "Encrypt=True;" +
                "TrustServerCertificate=False;" +
                "Connection Timeout=30;";

            private readonly DataFetcher DataFetcher;
            private readonly DataStorage DataStorage;
            private readonly GatewayDataStorage GatewayDataStorage;
            public DataAccess()
            {
                DataFetcher = new DataFetcher(ConnectionString);
                DataStorage = new DataStorage(DataFetcher);
                GatewayDataStorage = new GatewayDataStorage(DataFetcher);
                Initialize();
            }



            public void Initialize()
            {
                try
                {
                    // Heavy initialization logic
                    foreach (string location in Devices.GetDevices())
                    {
                        try
                        {
                            GatewayDataStorage.Add(AccesableData.Longitude, location,
                                $"SELECT Longitude FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.Latitude, location,
                                $"SELECT Latitude FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.Altitude, location,
                                $"SELECT Altitude FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.AvgRssi, location,
                                $"SELECT avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.AvgSnr, location,
                                $"SELECT avg_snr FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.MaxRssi, location,
                                $"SELECT max_rssi FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.MinRssi, location,
                                $"SELECT min_rssi FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.MaxSnr, location,
                                $"SELECT max_snr FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");
                            GatewayDataStorage.Add(AccesableData.MinSnr, location,
                                $"SELECT min_snr FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND gatewayID = 'gateway_input'");

                            DataStorage.Add(AccesableData.GatewayAmount, 0, location,
                                $"SELECT COUNT(*) FROM gateway WHERE deviceID LIKE '%{location.ToLower()}'");


                            // Current datapoints
                            DataStorage.Add(AccesableData.CurrentInsideTemperature, 0, location,
                                $"SELECT TOP 1 inside_temperature FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                            DataStorage.Add(AccesableData.CurrentOutsideTemperature, 0, location,
                                $"SELECT TOP 1 external_temperature FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                            DataStorage.Add(AccesableData.CurrentHumidity, 0, location,
                                $"SELECT TOP 1 humidity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                            DataStorage.Add(AccesableData.CurrentLight, 0, location,
                                $"SELECT TOP 1 luminosity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                            DataStorage.Add(AccesableData.BatteryPercentage, 0, location,
                                $@"SELECT d.battery_percentage
                       FROM dbo.device d
                       where d.deviceID LIKE '%{location.ToString().ToLower()}'");
                            DataStorage.Add(AccesableData.SignalToNoiseRatio, 0, location,
                                $"SELECT TOP 1 avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND avg_rssi IS NOT NULL ORDER BY deviceID ASC");
                            // Past data points
                            for (int i = 1; i < 31; i++)
                            {
                                string dateRangeQuery =
                                    $"BETWEEN DATEADD(DAY, -{i}, GETDATE()) AND DATEADD(DAY, -{i - 1}, GETDATE())";

                                DataStorage.Add(AccesableData.DayInsideTemperature, i, location,
                                    $"SELECT inside_temperature FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayOutsideTemperature, i, location,
                                    $"SELECT external_temperature FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayHumidity, i, location,
                                    $"SELECT humidity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayLight, i, location,
                                    $"SELECT luminosity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayPressure, i, location,
                                    $"SELECT pressure FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.SignalToNoiseRatio, 0, location,
                                    $"SELECT TOP 1 avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToLower()}' AND avg_rssi IS NOT NULL ORDER BY deviceID ASC");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException($"Error while adding data for location {location}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException("Error during DataAccess initialization", ex);
                    throw new InvalidOperationException("Failed to initialize data access.", ex);
                }
            }



            public async Task<List<double>> GetWeatherData(AccesableData name, int dayFromNow, string location)
            {
                try
                {
                    for (int i = 0; i < DataStorage.Name.Count; i++)
                    {
                        if (DataStorage.Name[i] == name && 
                            DataStorage.Location[i] == location &&
                            DataStorage.DayFromNow[i] == dayFromNow)
                        {
                            // Return right away as a Task
                            return await Task.FromResult(DataStorage.Data[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(
                        $"Error while fetching data for {name}, dayFromNow: {dayFromNow}, location: {location}", ex);
                }

                // Return empty if no data found or error
                return await Task.FromResult(new List<double>());
            }


            public async Task<double> GetGatewayData(AccesableData name, string gateway)
            {
                try
                {
                    for (int i = 0; i < GatewayDataStorage.Name.Count; i++)
                    {
                        if (GatewayDataStorage.Name[i] == name && 
                            GatewayDataStorage.Gateways[i] == gateway)
                        {
                            double value = GatewayDataStorage.Data[i][0];
                            return await Task.FromResult(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException($"Error while fetching data for {name}, gateway: {gateway}", ex);
                }

                // Return -1 if not found/error
                return await Task.FromResult(-1.0);
            }


            public async Task<List<string>> FetchGateways(string location)
            {
                List<string> gateways = new List<string>();
                try
                {
                    for (int i = 0; i < GatewayDataStorage.Gateways.Count; i++)
                    {
                        if (GatewayDataStorage.Location[i] == location &&
                            !gateways.Contains(GatewayDataStorage.Gateways[i]))
                        {
                            gateways.Add(GatewayDataStorage.Gateways[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException($"Error while fetching gateways for location {location}", ex);
                }

                return await Task.FromResult(gateways);
            }


            private void LogException(string context, Exception ex)
            {
                // Log exception with context
                Console.WriteLine($"{context}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}