using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using Microsoft.IdentityModel.Tokens;

namespace Backend;
using sql_fetcher;
using enums;

public class GraphData
{
    private List<double>? DayTemperature { get; set; } 
    private List<double>? WeekTemperature { get; set; }
    private List<double>? MonthTemperature { get; set; }
    
    private List<double>? DayHumidity { get; set; }
    private List<double>? WeekHumidity { get; set; }
    private List<double>? MonthHumidity { get; set; }
    
    private List<double>? DayLight { get; set; }
    private List<double>? WeekLight { get; set; }
    private List<double>? MonthLight { get; set; }
    
    private List<double>? CurrentTemperature { get; set; }
    private List<double>? CurrentHumidity { get; set; }
    private List<double>? CurrentLight { get; set; }
    
    private List<double>? BatteryStatus { get; set; }
    private List<double>? SignalToNoiseRatio { get; set; }
    
    private DataAccess DataAccess { get; set; }
    
    public GraphData()
    {
        DataAccess = new DataAccess() ?? throw new ArgumentNullException("DataAccess cannot be null");
    }

    public Dictionary<FrontendReadyData, List<double>> FetchGraphData(Locations location)
    {
        CurrentTemperature = new List<double>();
        CurrentHumidity = new List<double>();
        CurrentLight = new List<double>();
        BatteryStatus = new List<double>();
        SignalToNoiseRatio = new List<double>();
        
        DayTemperature = DataAccess.GetData(AccesableData.DayTemperature, location) ?? new List<double>();
        WeekTemperature = DataAccess.GetData(AccesableData.WeekTemperature, location) ?? new List<double>();
        MonthTemperature = DataAccess.GetData(AccesableData.MonthTemperature, location) ?? new List<double>();

        DayHumidity = DataAccess.GetData(AccesableData.DayHumidity, location) ?? new List<double>();
        WeekHumidity = DataAccess.GetData(AccesableData.WeekHumidity, location) ?? new List<double>();
        MonthHumidity = DataAccess.GetData(AccesableData.MonthHumidity, location) ?? new List<double>();
        
        DayLight = DataAccess.GetData(AccesableData.DayLight, location) ?? new List<double>();
        WeekLight = DataAccess.GetData(AccesableData.WeekLight, location) ?? new List<double>();
        MonthLight = DataAccess.GetData(AccesableData.MonthLight, location) ?? new List<double>();
        
        CurrentTemperature.Add(DataAccess.GetData(AccesableData.CurrentTemperature, location)?.FirstOrDefault() ?? 0);
        CurrentHumidity.Add(DataAccess.GetData(AccesableData.CurrentHumidity, location)?.FirstOrDefault() ?? 0);
        CurrentLight.Add(DataAccess.GetData(AccesableData.CurrentLight, location)?.FirstOrDefault() ?? 0);
        
        BatteryStatus.Add(DataAccess.GetData(AccesableData.BatteryStatus, location)?.FirstOrDefault() ?? 0);
        SignalToNoiseRatio.Add(DataAccess.GetData(AccesableData.SignalToNoiseRatio, location)?.FirstOrDefault() ?? 0);
        
        // Calculate averages safely
        List<double>? hourlyDayTemperatureAverage = CalculateAverages(DayTemperature, 24);
        List<double>? dailyWeekTemperatureAverage = CalculateAverages(WeekTemperature, 7);
        List<double>? dailyMonthTemperatureAverage = CalculateAverages(MonthTemperature, 30);

        List<double>? hourlyDayHumidityAverage = CalculateAverages(DayTemperature, 24);
        List<double>? dailyWeekHumidityAverage = CalculateAverages(WeekHumidity, 7);
        List<double>? dailyMonthHumidityAverage = CalculateAverages(MonthHumidity, 30);
        
        List<double>? hourlyDayLightAverage = CalculateAverages(DayLight, 24);
        List<double>? dailyWeekLightAverage = CalculateAverages(WeekLight, 7);
        List<double>? dailyMonthLightAverage = CalculateAverages(MonthLight, 30);
        
        return new Dictionary<FrontendReadyData, List<double>>
        {
            {FrontendReadyData.HourlyDayTemperatureAverage, hourlyDayTemperatureAverage},
            {FrontendReadyData.DailyWeekTemperatureAverage, dailyWeekTemperatureAverage},
            {FrontendReadyData.DailyMonthTemperatureAverage, dailyMonthTemperatureAverage},
            {FrontendReadyData.HourlyDayHumidityAverage, hourlyDayHumidityAverage},
            {FrontendReadyData.DailyWeekHumidityAverage, dailyWeekHumidityAverage},
            {FrontendReadyData.DailyMonthHumidityAverage, dailyMonthHumidityAverage},
            {FrontendReadyData.HourlyDayLightAverage, hourlyDayLightAverage},
            {FrontendReadyData.DailyWeekLightAverage, dailyWeekLightAverage},
            {FrontendReadyData.DailyMonthLightAverage, dailyMonthLightAverage},
            {FrontendReadyData.CurrentTemperature, CurrentTemperature},
            {FrontendReadyData.CurrentHumidity, CurrentHumidity},
            {FrontendReadyData.CurrentLight, CurrentLight},
            {FrontendReadyData.BatteryStatus, BatteryStatus},
            {FrontendReadyData.SignalToNoiseRatio, SignalToNoiseRatio}
        };
    } 
    

    private static List<double> CalculateAverages(List<double> data, int datapoints)
    {
        if (datapoints <= 0)
        {
            throw new ArgumentException("Datapoints must be greater than 0", nameof(datapoints));
        }
        if (datapoints > data.Count)
        {
            return Enumerable.Repeat(0.0, datapoints).ToList();
        }
        var averages = new List<double>();
        for (int i = 0; i < datapoints; i++)
        {
            if (data.Count > i)
            {
                averages.Add(data.GetRange(i * (data.Count / datapoints), data.Count / datapoints).Average());
            }
            else
            {
                averages.Add(0);
            }
        }
        return averages;
    }
    
}