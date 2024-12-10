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
    
        
    private List<double>? DayPressure { get; set; }
    private List<double>? WeekPressure { get; set; }
    private List<double>? MonthPressure { get; set; }
    
    
    private List<double>? CurrentTemperature { get; set; }
    private List<double>? CurrentHumidity { get; set; }
    private List<double>? CurrentLight { get; set; }
    private List<double>? CurrentPressure { get; set; }

    
    private List<double>? BatteryVoltage { get; set; }
    private List<double>? BatteryPercentage { get; set; }
    private List<double>? SignalToNoiseRatio { get; set; }
    private List<double>? ModelID { get; set; }
    
    private DataAccess DataAccess { get; set; }
    
    public GraphData()
    {
        DataAccess = new DataAccess() ?? throw new ArgumentNullException("DataAccess cannot be null");
    }

    public Dictionary<FrontendReadyData, List<double>> FetchGraphData(ILocationEnum location)
    {
        CurrentTemperature = new List<double>();
        CurrentHumidity = new List<double>();
        CurrentLight = new List<double>();
        CurrentPressure = new List<double>();
        
        BatteryVoltage = new List<double>();
        BatteryPercentage = new List<double>();
        ModelID = new List<double>();
        SignalToNoiseRatio = new List<double>();
        
        
        WeekTemperature = new List<double>();
        WeekHumidity = new List<double>();
        WeekLight = new List<double>();
        WeekPressure = new List<double>();
        
        MonthTemperature = new List<double>();
        MonthHumidity = new List<double>();
        MonthLight = new List<double>();
        MonthPressure = new List<double>();
        
        
        DayTemperature = DataAccess.GetData(AccesableData.DayTemperature, 1, location) ?? new List<double>();
        DayHumidity = DataAccess.GetData(AccesableData.DayHumidity, 1, location) ?? new List<double>();
        DayLight = DataAccess.GetData(AccesableData.DayLight, 1, location) ?? new List<double>();
        DayPressure = DataAccess.GetData(AccesableData.DayPressure, 1, location) ?? new List<double>();
        
        for (int i = 1; i < 31; i++)
        {
            List<double> currentTemperatureList = new List<double>();
            List<double> currentHumidityList = new List<double>();
            List<double> currentLightList = new List<double>();
            List<double> currentPressureList = new List<double>();

            currentTemperatureList = DataAccess.GetData(AccesableData.DayTemperature, i, location);
            currentHumidityList = DataAccess.GetData(AccesableData.DayHumidity, i, location);
            currentLightList = DataAccess.GetData(AccesableData.DayLight, i, location);
            currentPressureList = DataAccess.GetData(AccesableData.DayPressure, i, location);

            if (i < 8)
            {
                WeekTemperature.Add(currentTemperatureList.Any() ? currentTemperatureList.Average() : 0);
                WeekHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : 0);
                WeekLight.Add(currentLightList.Any() ? currentLightList.Average() : 0);
                WeekPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : 0);
            }

            MonthTemperature.Add(currentTemperatureList.Any() ? currentTemperatureList.Average() : 0);
            MonthHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : 0);
            MonthLight.Add(currentLightList.Any() ? currentLightList.Average() : 0);
            MonthPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : 0);
        }
        
        CurrentTemperature.Add(DataAccess.GetData(AccesableData.CurrentTemperature, 0, location)?.FirstOrDefault() ?? 0);
        CurrentHumidity.Add(DataAccess.GetData(AccesableData.CurrentHumidity, 0, location)?.FirstOrDefault() ?? 0);
        CurrentLight.Add(DataAccess.GetData(AccesableData.CurrentLight, 0, location)?.FirstOrDefault() ?? 0);
        CurrentPressure.Add(DataAccess.GetData(AccesableData.CurrentPressure, 0, location)?.FirstOrDefault() ?? 0);
        
        BatteryVoltage.Add(DataAccess.GetData(AccesableData.BatteryVoltage, 0, location)?.FirstOrDefault() ?? 0);
        SignalToNoiseRatio.Add(DataAccess.GetData(AccesableData.SignalToNoiseRatio, 0, location)?.FirstOrDefault() ?? 0);
        ModelID.Add(DataAccess.GetData(AccesableData.ModelId, 0, location)?.FirstOrDefault() ?? 0);
        BatteryPercentage.Add(DataAccess.GetData(AccesableData.BatteryPercentage, 0, location)?.FirstOrDefault() ?? 0);
        
    // Calculate averages safely`
    List<double>? hourlyDayTemperatureAverage = CalculateAverages(DayTemperature, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekTemperatureAverage = WeekTemperature?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthTemperatureAverage = MonthTemperature?.AsEnumerable().Reverse().ToList();

    List<double>? hourlyDayHumidityAverage = CalculateAverages(DayHumidity, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekHumidityAverage = WeekHumidity?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthHumidityAverage = MonthHumidity?.AsEnumerable().Reverse().ToList();

    List<double>? hourlyDayLightAverage = CalculateAverages(DayLight, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekLightAverage = WeekLight?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthLightAverage = MonthLight?.AsEnumerable().Reverse().ToList();

    List<double>? hourlyDayPressureAverage = CalculateAverages(DayPressure, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekPressureAverage = WeekPressure?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthPressureAverage = MonthPressure?.AsEnumerable().Reverse().ToList();
        
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
            {FrontendReadyData.HourlyDayPressureAverage, hourlyDayPressureAverage},
            {FrontendReadyData.DailyWeekPressureAverage, dailyWeekPressureAverage},
            {FrontendReadyData.DailyMonthPressureAverage, dailyMonthPressureAverage},
            {FrontendReadyData.CurrentTemperature, CurrentTemperature},
            {FrontendReadyData.CurrentHumidity, CurrentHumidity},
            { FrontendReadyData.CurrentPressure, CurrentPressure},
            {FrontendReadyData.CurrentLight, CurrentLight},
            {FrontendReadyData.BatteryVoltage, BatteryVoltage},
            {FrontendReadyData.BatteryPercentage, BatteryPercentage},
            {FrontendReadyData.ModelId, ModelID},
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