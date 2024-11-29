﻿namespace sql_fetcher;
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
        //TODO: Add all the queries for all the available options in AccesableData.cs
        foreach (Locations location in Enum.GetValues(typeof(Locations)))
        {
            DataStorage.Add(AccesableData.CurrentTemperature, location,
                $"SELECT TOP 1 temperature FROM weather ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentHumidity, location,
                $"SELECT TOP 1 humidity FROM weather ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.CurrentLight, location,
                $"SELECT TOP 1 luminosity FROM weather ORDER BY weather.date DESC");
            DataStorage.Add(AccesableData.DayTemperature, location,
                $"SELECT temperature FROM weather WHERE date >= DATEADD(DAY, -1, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.WeekTemperature, location,
                 $"SELECT temperature FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.MonthTemperature, location,
                 $"SELECT temperature FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.DayHumidity, location,
                $"SELECT humidity FROM weather WHERE date >= DATEADD(DAY, -1, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC"); 
            DataStorage.Add(AccesableData.WeekHumidity, location,
                $"SELECT humidity FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.MonthHumidity, location,
                $"SELECT humidity FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.DayLight, location,
                $"SELECT luminosity FROM weather WHERE date >= DATEADD(DAY, -1, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC"); 
            DataStorage.Add(AccesableData.WeekLight, location,
                $"SELECT luminosity FROM weather WHERE date >= DATEADD(DAY, -7, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");
            DataStorage.Add(AccesableData.MonthLight, location,
                $"SELECT luminosity FROM weather WHERE date >= DATEADD(DAY, -30, GETDATE()) AND deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY date DESC");

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