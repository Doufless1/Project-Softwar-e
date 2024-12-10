﻿﻿namespace sql_fetcher;
 using enums;

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
    private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
    private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);

    public DataAccess() //Initializes queries in DataStorage.cs with the queries to be fetched
    {
        foreach (Locations location in Enum.GetValues(typeof(Locations)))
        {
            ILocationEnum locationEnum = new LocationEnum(location);
            //Current datapoints
            DataStorage.Add(AccesableData.CurrentTemperature, 0, locationEnum,
                $"SELECT TOP 1 temperature FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentHumidity, 0, locationEnum,
                $"SELECT TOP 1 humidity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentLight, 0, locationEnum,
                $"SELECT TOP 1 luminosity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentLight, 0, locationEnum,
                $"SELECT TOP 1 luminosity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentPressure, 0, locationEnum,
                $"SELECT TOP 1 pressure FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");

            DataStorage.Add(AccesableData.BatteryVoltage, 0, locationEnum,
                $"SELECT battery_voltage FROM dbo.device WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND battery_voltage IS NOT NULL");   
            DataStorage.Add(AccesableData.BatteryPercentage, 0, locationEnum,
                $"SELECT battery_percentage FROM dbo.device WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND battery_percentage IS NOT NULL");   

            
            //Past datapoints
            for (int i = 1; i < 31; i++)
            {
                // Generate date range for the query
                string dateRangeQuery = $"BETWEEN DATEADD(DAY, -{i}, GETDATE()) AND DATEADD(DAY, -{i - 1}, GETDATE())";

                DataStorage.Add(AccesableData.DayTemperature, i, locationEnum,
                    $"SELECT temperature FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}' AND temperature IS NOT NULL");
                DataStorage.Add(AccesableData.DayHumidity, i, locationEnum,
                    $"SELECT humidity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}' AND humidity IS NOT NULL");
                DataStorage.Add(AccesableData.DayLight, i, locationEnum,
                    $"SELECT luminosity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}' AND luminosity IS NOT NULL");
                DataStorage.Add(AccesableData.DayPressure, i, locationEnum,
                    $"SELECT pressure FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}' AND pressure IS NOT NULL");
            }
        }
    }

    public List<double> GetData(AccesableData name, int dayFromNow, ILocationEnum location) //Gets returns data (List<string>) of the name requested (i.e. current_temperature, current_humidity)
    {
        for(int i = 0; i < DataStorage.Name.Count; i++)
        {
            if (DataStorage.Name[i] == name && DataStorage.Location[i] == location && DataStorage.DayFromNow[i] == dayFromNow)
            {
                return DataStorage.Data[i];
            }
        }
        return new List<double>();
    }
}