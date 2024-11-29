using System.Security.Cryptography.X509Certificates;

namespace Backend;
using sql_fetcher;
using enums;

public class GraphData
{
    public List<double> DayTemperature { get; set; }
    public List<double> WeekTemperature { get; set; }
    public List<double> MonthTemperature { get; set; }
    public List<double> DayHumidity { get; set; }
    public List<double> WeekHumidity { get; set; }
    public List<double> MonthHumidity { get; set; }
    
    public DataAccess DataAccess { get; set; }

    public Dictionary<GraphDataEnum, List<double>> FetchGraphData(Locations location)
    {
        DataAccess = new DataAccess();
        
        var dayTemperature = DataAccess.GetData(AccesableData.DayTemperature, location) ?? new List<double>();
        var weekTemperature = DataAccess.GetData(AccesableData.WeekTemperature, location) ?? new List<double>();
        var monthTemperature = DataAccess.GetData(AccesableData.MonthTemperature, location) ?? new List<double>();

        var dayHumidity = DataAccess.GetData(AccesableData.DayHumidity, location) ?? new List<double>();
        var weekHumidity = DataAccess.GetData(AccesableData.WeekHumidity, location) ?? new List<double>();
        var monthHumidity = DataAccess.GetData(AccesableData.MonthHumidity, location) ?? new List<double>();
        
        // Calculate averages safely
        var hourlyDayTemperatureAverage = CalculateAverages(dayTemperature, 24);
        var dailyWeekTemperatureAverage = CalculateAverages(weekTemperature, 7);
        var dailyMonthTemperatureAverage = CalculateAverages(monthTemperature, 30);

        var hourlyDayHumidityAverage = CalculateAverages(dayHumidity, 24);
        var dailyWeekHumidityAverage = CalculateAverages(weekHumidity, 7);
        var dailyMonthHumidityAverage = CalculateAverages(monthHumidity, 30);
        
        return new Dictionary<GraphDataEnum, List<double>>
        {
            {GraphDataEnum.HourlyDayTemperatureAverage, hourlyDayTemperatureAverage},
            {GraphDataEnum.DailyWeekTemperatureAverage, dailyWeekTemperatureAverage},
            {GraphDataEnum.DailyMonthTemperatureAverage, dailyMonthTemperatureAverage},
            {GraphDataEnum.HourlyDayHumidityAverage, hourlyDayHumidityAverage},
            {GraphDataEnum.DailyWeekHumidityAverage, dailyWeekHumidityAverage},
            {GraphDataEnum.DailyMonthHumidityAverage, dailyMonthHumidityAverage}
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
            throw new ArgumentException("Datapoints must at least be greater then 29", nameof(datapoints));
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