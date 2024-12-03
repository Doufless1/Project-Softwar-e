﻿namespace sql_fetcher;
 using enums;

public class DataAccess
{
    private static readonly string ConnectionString = 
        "Server=tcp:group13.database.windows.net,1433;" +
        "Database=weather_state_old;" +
        "User ID=cloudadmin;" +
        "Password=Group13pass;" +
        "Encrypt=True;" +
        "TrustServerCertificate=False;" +
        "Connection Timeout=30;";
    private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
    private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);

    public DataAccess() //Initializes queries in DataStorage.cs with the queries to be fetched
    {
        //TODO: Add all the queries for all the available options in AccesableData.cs
        //TODO: Clean this up to current_day, current_day - 1, current_day - 2, etc. Current version requires a lot of data to be fetched and is taking a long time. Also it's not precise.
        foreach (Locations location in Enum.GetValues(typeof(Locations)))
        {
            //Current datapoints
            DataStorage.Add(AccesableData.CurrentTemperature, 0, location,
                $"SELECT TOP 1 temperature FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentHumidity, 0, location,
                $"SELECT TOP 1 humidity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentLight, 0, location,
                $"SELECT TOP 1 luminosity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.BatteryStatus, 0, location,
                $"SELECT TOP 1 battery_voltage FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND battery_voltage IS NOT NULL ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.SignalToNoiseRatio, 0, location,
                $"SELECT TOP 1 SNR FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND SNR IS NOT NULL ORDER BY weather.date DESC");
            
            //Past datapoints
            for (int i = 1; i < 31; i++)
            {
                // Generate date range for the query
                string dateRangeQuery = $"BETWEEN DATEADD(DAY, -{i + 1}, GETDATE()) AND DATEADD(DAY, -{i}, GETDATE())";

                DataStorage.Add(AccesableData.DayTemperature, i, location,
                    $"SELECT temperature FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                DataStorage.Add(AccesableData.DayHumidity, i, location,
                    $"SELECT humidity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                DataStorage.Add(AccesableData.DayLight, i, location,
                    $"SELECT luminosity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
            }


            // DataStorage.Add(AccesableData.WeekTemperature, location,
            //      $"SELECT temperature FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            // DataStorage.Add(AccesableData.MonthTemperature, location,
            //      $"SELECT temperature FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            // DataStorage.Add(AccesableData.WeekHumidity, location,
            //     $"SELECT humidity FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            // DataStorage.Add(AccesableData.MonthHumidity, location,
            //     $"SELECT humidity FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            // DataStorage.Add(AccesableData.WeekLight, location,
            //     $"SELECT luminosity FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            // DataStorage.Add(AccesableData.MonthLight, location,
            //     $"SELECT luminosity FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            
        }
    }

    public List<double> GetData(AccesableData name, int dayFromNow, Locations location) //Gets returns data (List<string>) of the name requested (i.e. current_temperature, current_humidity)
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