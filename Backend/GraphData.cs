namespace Backend;
using sql_fetcher;
using enums;

public class GraphData
{
    private List<double>? DayInsideTemperature { get; set; } 
    private List<double>? WeekInsideTemperature { get; set; }
    private List<double>? MonthInsideTemperature { get; set; }
    
    private List<double>? DayOutsideTemperature { get; set; }
    private List<double>? WeekOutsideTemperature { get; set; }
    private List<double>? MonthOutsideTemperature { get; set; }
    
    private List<double>? DayHumidity { get; set; }
    private List<double>? WeekHumidity { get; set; }
    private List<double>? MonthHumidity { get; set; }
    
    private List<double>? DayLight { get; set; }
    private List<double>? WeekLight { get; set; }
    private List<double>? MonthLight { get; set; }
    
        
    private List<double>? DayPressure { get; set; }
    private List<double>? WeekPressure { get; set; }
    private List<double>? MonthPressure { get; set; }
    
    
    private List<double>? CurrentInsideTemperature { get; set; }
    private List<double>? CurrentOutsideTemperature { get; set; }
    private List<double>? CurrentHumidity { get; set; }
    private List<double>? CurrentLight { get; set; }
    private List<double>? CurrentPressure { get; set; }

    
    private List<double>? BatteryVoltage { get; set; }
    private List<double>? BatteryPercentage { get; set; }
    private List<double>? ModelID { get; set; }
    
    private DataAccess DataAccess { get; set; }

    public GraphData()
    {
        DataAccess = new DataAccess() ?? throw new ArgumentNullException("DataAccess cannot be null");
    }

    public Dictionary<FrontendReadyData, List<double>> FetchGraphData(string location)
    {
        CurrentInsideTemperature = new List<double>();
        CurrentOutsideTemperature = new List<double>();
        CurrentHumidity = new List<double>();
        CurrentLight = new List<double>();
        CurrentPressure = new List<double>();
        
        BatteryVoltage = new List<double>();
        BatteryPercentage = new List<double>();
        ModelID = new List<double>();
        
        WeekInsideTemperature = new List<double>();
        WeekOutsideTemperature = new List<double>();
        WeekHumidity = new List<double>();
        WeekLight = new List<double>();
        WeekPressure = new List<double>();
        
        MonthInsideTemperature = new List<double>();
        MonthOutsideTemperature = new List<double>();
        MonthHumidity = new List<double>();
        MonthLight = new List<double>();
        MonthPressure = new List<double>();
        
        
        DayInsideTemperature = DataAccess.GetWeatherData(AccesableData.DayInsideTemperature, 1, location) ?? new List<double>();
        DayOutsideTemperature = DataAccess.GetWeatherData(AccesableData.DayOutsideTemperature, 1, location) ?? new List<double>();
        DayHumidity = DataAccess.GetWeatherData(AccesableData.DayHumidity, 1, location) ?? new List<double>();
        DayLight = DataAccess.GetWeatherData(AccesableData.DayLight, 1, location) ?? new List<double>();
        DayPressure = DataAccess.GetWeatherData(AccesableData.DayPressure, 1, location) ?? new List<double>();
        
        for (int i = 1; i < 31; i++)
        {
            List<double> currentInsideTemperatureList = new List<double>();
            List<double> currentOutsideTemperatureList = new List<double>();
            List<double> currentHumidityList = new List<double>();
            List<double> currentLightList = new List<double>();
            List<double> currentPressureList = new List<double>();

            currentInsideTemperatureList = DataAccess.GetWeatherData(AccesableData.DayInsideTemperature, i, location);
            currentOutsideTemperatureList = DataAccess.GetWeatherData(AccesableData.DayOutsideTemperature, i, location);
            currentHumidityList = DataAccess.GetWeatherData(AccesableData.DayHumidity, i, location);
            currentLightList = DataAccess.GetWeatherData(AccesableData.DayLight, i, location);
            currentPressureList = DataAccess.GetWeatherData(AccesableData.DayPressure, i, location);

            if (i < 8)
            {
                WeekInsideTemperature.Add(currentInsideTemperatureList.Any() ? currentInsideTemperatureList.Average() : 0);
                WeekOutsideTemperature.Add(currentOutsideTemperatureList.Any() ? currentOutsideTemperatureList.Average() : 0);
                WeekHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : 0);
                WeekLight.Add(currentLightList.Any() ? currentLightList.Average() : 0);
                WeekPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : 0);
            }

            MonthInsideTemperature.Add(currentInsideTemperatureList.Any() ? currentInsideTemperatureList.Average() : 0);
            MonthOutsideTemperature.Add(currentOutsideTemperatureList.Any() ? currentOutsideTemperatureList.Average() : 0);
            MonthHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : 0);
            MonthLight.Add(currentLightList.Any() ? currentLightList.Average() : 0);
            MonthPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : 0);
        }
        
        CurrentInsideTemperature.Add(DataAccess.GetWeatherData(AccesableData.CurrentInsideTemperature, 0, location)?.FirstOrDefault() ?? -100);
        CurrentOutsideTemperature.Add(DataAccess.GetWeatherData(AccesableData.CurrentOutsideTemperature, 0, location)?.FirstOrDefault() ?? -100);
        CurrentHumidity.Add(DataAccess.GetWeatherData(AccesableData.CurrentHumidity, 0, location)?.FirstOrDefault() ?? -100);
        CurrentLight.Add(DataAccess.GetWeatherData(AccesableData.CurrentLight, 0, location)?.FirstOrDefault() ?? -100);
        CurrentPressure.Add(DataAccess.GetWeatherData(AccesableData.CurrentPressure, 0, location)?.FirstOrDefault() ?? -100);
        
        BatteryVoltage.Add(DataAccess.GetWeatherData(AccesableData.BatteryVoltage, 0, location)?.FirstOrDefault() ?? -100);
        ModelID.Add(DataAccess.GetWeatherData(AccesableData.ModelId, 0, location)?.FirstOrDefault() ?? -100);
        BatteryPercentage.Add(DataAccess.GetWeatherData(AccesableData.BatteryPercentage, 0, location)?.FirstOrDefault() ?? -100);
        
    // Calculate averages safely`
    List<double>? hourlyDayInsideTemperatureAverage = CalculateAverages(DayInsideTemperature, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekInsideTemperatureAverage = WeekInsideTemperature?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthInsideTemperatureAverage = MonthInsideTemperature?.AsEnumerable().Reverse().ToList();
    
    List<double>? hourlyDayOutsideTemperatureAverage = CalculateAverages(DayOutsideTemperature, 24)?.AsEnumerable().Reverse().ToList();
    List<double>? dailyWeekOutsideTemperatureAverage = WeekOutsideTemperature?.AsEnumerable().Reverse().ToList();
    List<double>? dailyMonthOutsideTemperatureAverage = MonthOutsideTemperature?.AsEnumerable().Reverse().ToList();
    
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
            {FrontendReadyData.HourlyDayInsideTemperatureAverage, hourlyDayInsideTemperatureAverage},
            {FrontendReadyData.DailyWeekInsideTemperatureAverage, dailyWeekInsideTemperatureAverage},
            {FrontendReadyData.DailyMonthInsideTemperatureAverage, dailyMonthInsideTemperatureAverage},
            {FrontendReadyData.HourlyDayOutsideTemperatureAverage, hourlyDayOutsideTemperatureAverage},
            {FrontendReadyData.DailyWeekOutsideTemperatureAverage, dailyWeekOutsideTemperatureAverage},
            {FrontendReadyData.DailyMonthOutsideTemperatureAverage, dailyMonthOutsideTemperatureAverage},
            {FrontendReadyData.HourlyDayHumidityAverage, hourlyDayHumidityAverage},
            {FrontendReadyData.DailyWeekHumidityAverage, dailyWeekHumidityAverage},
            {FrontendReadyData.DailyMonthHumidityAverage, dailyMonthHumidityAverage},
            {FrontendReadyData.HourlyDayLightAverage, hourlyDayLightAverage},
            {FrontendReadyData.DailyWeekLightAverage, dailyWeekLightAverage},
            {FrontendReadyData.DailyMonthLightAverage, dailyMonthLightAverage},
            {FrontendReadyData.HourlyDayPressureAverage, hourlyDayPressureAverage},
            {FrontendReadyData.DailyWeekPressureAverage, dailyWeekPressureAverage},
            {FrontendReadyData.DailyMonthPressureAverage, dailyMonthPressureAverage},
            {FrontendReadyData.CurrentInsideTemperature, CurrentInsideTemperature},
            {FrontendReadyData.CurrentOutsideTemperature, CurrentOutsideTemperature},
            {FrontendReadyData.CurrentHumidity, CurrentHumidity},
            { FrontendReadyData.CurrentPressure, CurrentPressure},
            {FrontendReadyData.CurrentLight, CurrentLight},
            {FrontendReadyData.BatteryVoltage, BatteryVoltage},
            {FrontendReadyData.BatteryPercentage, BatteryPercentage},
            {FrontendReadyData.ModelId, ModelID}
        };
    }
    
    public Dictionary<string, Dictionary<AccesableData, double>> FetchGatewayData(string location)
    {
        DataAccess.FetchGateways(location);
        Dictionary<string, Dictionary<AccesableData, double>> result = new Dictionary<string, Dictionary<AccesableData, double>>();
        foreach (string gateway in DataAccess.FetchGateways(location))
        {
            result.Add(gateway, new Dictionary<AccesableData, double>
            {
                {AccesableData.MaxRssi, DataAccess.GetGatewayData(AccesableData.MaxRssi, gateway)},
                {AccesableData.MinRssi, DataAccess.GetGatewayData(AccesableData.MinRssi, gateway)},
                {AccesableData.MaxSnr, DataAccess.GetGatewayData(AccesableData.MaxSnr, gateway)},
                {AccesableData.MinSnr, DataAccess.GetGatewayData(AccesableData.MinSnr, gateway)},
                {AccesableData.AvgRssi, DataAccess.GetGatewayData(AccesableData.AvgRssi, gateway)},
                {AccesableData.AvgSnr, DataAccess.GetGatewayData(AccesableData.AvgSnr, gateway)},
                {AccesableData.Longitude, DataAccess.GetGatewayData(AccesableData.Longitude, gateway)},
                {AccesableData.Latitude, DataAccess.GetGatewayData(AccesableData.Latitude, gateway)},
                {AccesableData.Altitude, DataAccess.GetGatewayData(AccesableData.Altitude, gateway)}
            });
        }
        return result;
    }
    
    public Dictionary<List<string>, List<double>> FetchWeatherDataInRange(int days, AccesableData data, string location)
    {
        Dictionary<List<double>, List<double>> result;
        List<double> values = new List<double>();
        for(int i = 0; i < days; i++)
        {
            values.AddRange(DataAccess.GetWeatherData(data, i, location));
        }
        
        List<string> XAxis = new List<string>();
        for (int i = 0; i < days; i++)
        {
            XAxis.Add(DateTime.Now.AddDays(-i).Day.ToString("dd,MM"));
        }
        return new Dictionary<List<string>, List<double>> {{XAxis, values}};
    }
    
    
    private static List<double> CalculateAverages(List<double> data, int datapoints)
    {
        if (datapoints <= 0)
        {
            throw new ArgumentException("Datapoints must be greater than 0", nameof(datapoints));
        }
        if (datapoints > data.Count)
        {
            return Enumerable.Repeat(-100.0, datapoints).ToList();
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
                averages.Add(-100.0);
            }
        }
        return averages;
    }
    
}