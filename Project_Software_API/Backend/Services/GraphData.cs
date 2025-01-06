using Microsoft.IdentityModel.Tokens;

namespace Project_Software_API.Properties.Backend.Services;

using Models;
using System.Linq;
using SQL.sql_fetcher;
/// <summary>
/// Provides methods to fetch and process graph data for various metrics.
/// </summary>

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

        private readonly DataAccess DataAccess;

        public GraphData()
        {
            DataAccess = new DataAccess() ?? throw new ArgumentNullException("DataAccess cannot be null");
        }

        /// <summary>
        /// Fetch graph data for a given location (asynchronously).
        /// </summary>
        public async Task<Dictionary<FrontendReadyData, List<double>>> FetchGraphData(string location)
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
        
        
        DayInsideTemperature = await DataAccess.GetWeatherData(AccesableData.DayInsideTemperature, 1, location) ?? new List<double>();
        DayOutsideTemperature = await DataAccess.GetWeatherData(AccesableData.DayOutsideTemperature, 1, location) ?? new List<double>();
        DayHumidity = await DataAccess.GetWeatherData(AccesableData.DayHumidity, 1, location) ?? new List<double>();
        DayLight = await DataAccess.GetWeatherData(AccesableData.DayLight, 1, location) ?? new List<double>();
        DayPressure = await DataAccess.GetWeatherData(AccesableData.DayPressure, 1, location) ?? new List<double>();
        
        for (int i = 1; i < 31; i++)
        {
            List<double> currentInsideTemperatureList = new List<double>();
            List<double> currentOutsideTemperatureList = new List<double>();
            List<double> currentHumidityList = new List<double>();
            List<double> currentLightList = new List<double>();
            List<double> currentPressureList = new List<double>();

            currentInsideTemperatureList = await DataAccess.GetWeatherData(AccesableData.DayInsideTemperature, i, location);
            currentOutsideTemperatureList = await DataAccess.GetWeatherData(AccesableData.DayOutsideTemperature, i, location);
            currentHumidityList = await DataAccess.GetWeatherData(AccesableData.DayHumidity, i, location);
            currentLightList = await DataAccess.GetWeatherData(AccesableData.DayLight, i, location);
            currentPressureList = await DataAccess.GetWeatherData(AccesableData.DayPressure, i, location);

            if (currentInsideTemperatureList.Any()) 
                if (currentInsideTemperatureList.Average() > 40)
                    currentInsideTemperatureList.Clear();
            if (currentOutsideTemperatureList.Any()) 
                if (currentOutsideTemperatureList.Average() > 40)
                    currentOutsideTemperatureList.Clear();
                
            
            
            if (i < 8)
            {
                WeekInsideTemperature.Add(currentInsideTemperatureList.Any() ? currentInsideTemperatureList.Average() : -100);
                WeekOutsideTemperature.Add(currentOutsideTemperatureList.Any() ? currentOutsideTemperatureList.Average() : -100);
                WeekHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : -100);
                WeekLight.Add(currentLightList.Any() ? currentLightList.Average() : -100);
                WeekPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : -100);
            }

            MonthInsideTemperature.Add(currentInsideTemperatureList.Any() ? currentInsideTemperatureList.Average() : -100);
            MonthOutsideTemperature.Add(currentOutsideTemperatureList.Any() ? currentOutsideTemperatureList.Average() : -100);
            MonthHumidity.Add(currentHumidityList.Any() ? currentHumidityList.Average() : -100);
            MonthLight.Add(currentLightList.Any() ? currentLightList.Average() : -100);
            MonthPressure.Add(currentPressureList.Any() ? currentPressureList.Average() : -100);
        }
        
        CurrentInsideTemperature.Add(
            (await DataAccess.GetWeatherData(AccesableData.CurrentInsideTemperature, 0, location))
                .FirstOrDefault(value => value != null, -100)
        );
        CurrentOutsideTemperature.Add(
            (await DataAccess.GetWeatherData(AccesableData.CurrentOutsideTemperature, 0, location))
            .FirstOrDefault(value => value != null, -100));
        CurrentHumidity.Add(
            (await DataAccess.GetWeatherData(AccesableData.CurrentHumidity, 0, location))
            .FirstOrDefault(value => value != null, -100));
        CurrentLight.Add(
            (await DataAccess.GetWeatherData(AccesableData.CurrentLight, 0, location))
                .FirstOrDefault(value => value != null, -100));
        CurrentPressure.Add(
            (await DataAccess.GetWeatherData(AccesableData.CurrentPressure, 0, location))
            .FirstOrDefault(value => value != null, -100));
        
        BatteryVoltage.Add((await DataAccess.GetWeatherData(AccesableData.BatteryVoltage, 0, location))?.FirstOrDefault() ?? -100);
        ModelID.Add((await DataAccess.GetWeatherData(AccesableData.ModelId, 0, location))?.FirstOrDefault() ?? -100);
        BatteryPercentage.Add((await DataAccess.GetWeatherData(AccesableData.BatteryPercentage, 0, location))?.FirstOrDefault() ?? -100);
        
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
            {FrontendReadyData.CurrentPressure, CurrentPressure},
            {FrontendReadyData.CurrentLight, CurrentLight},
            {FrontendReadyData.BatteryVoltage, BatteryVoltage},
            {FrontendReadyData.BatteryPercentage, BatteryPercentage},
            {FrontendReadyData.ModelId, ModelID}
        };
        }

        /// <summary>
        /// Fetch gateway data. (Still synchronous if your DataAccess is sync.)
        /// </summary>
       public async Task<Dictionary<string, Dictionary<AccesableData, double>>> FetchGatewayData(string location)
        {
            // 1) If 'FetchGateways' is async and returns Task<List<string>>, do:
            var gatewayList = await DataAccess.FetchGateways(location);

            // 2) Build your result dictionary
            var result = new Dictionary<string, Dictionary<AccesableData, double>>();

            // 3) For each gateway in the real List<string>:
            foreach (var gateway in gatewayList)
            {
                // 4) Await each call that returns a Task<double>
                double maxRssi   = await DataAccess.GetGatewayData(AccesableData.MaxRssi, gateway);
                double minRssi   = await DataAccess.GetGatewayData(AccesableData.MinRssi, gateway);
                double maxSnr    = await DataAccess.GetGatewayData(AccesableData.MaxSnr, gateway);
                double minSnr    = await DataAccess.GetGatewayData(AccesableData.MinSnr, gateway);
                double avgRssi   = await DataAccess.GetGatewayData(AccesableData.AvgRssi, gateway);
                double avgSnr    = await DataAccess.GetGatewayData(AccesableData.AvgSnr, gateway);
                double longitude = await DataAccess.GetGatewayData(AccesableData.Longitude, gateway);
                double latitude  = await DataAccess.GetGatewayData(AccesableData.Latitude, gateway);
                double altitude  = await DataAccess.GetGatewayData(AccesableData.Altitude, gateway);

                // 5) Insert the awaited double values into the dictionary
                result.Add(gateway, new Dictionary<AccesableData, double>
                {
                    { AccesableData.MaxRssi,   maxRssi },
                    { AccesableData.MinRssi,   minRssi },
                    { AccesableData.MaxSnr,    maxSnr },
                    { AccesableData.MinSnr,    minSnr },
                    { AccesableData.AvgRssi,   avgRssi },
                    { AccesableData.AvgSnr,    avgSnr },
                    { AccesableData.Longitude, longitude },
                    { AccesableData.Latitude,  latitude },
                    { AccesableData.Altitude,  altitude }
                });
            }

            return result;
        }


        /// <summary>
        /// Example: fetch weather data in range (still synchronous).
        /// </summary>
        public async Task<Dictionary<List<string>, List<double>>> FetchWeatherDataInRange(int start_date, int end_date, AccesableData data, string location)
        {
            var result = new Dictionary<List<string>, List<double>>();
            List<double>? weatherValues = new List<double>();
        
            for (int i = start_date; i >= end_date; i--)
            {
                List<double> input = await DataAccess.GetWeatherData(data, i, location);
                if(input.IsNullOrEmpty())
                {
                    input.Add(-100);
                }
                weatherValues.Add(input.Average());
            }

            List<string> xAxisLabels = new List<string>();
            for (int i = start_date; i >= end_date; i--)
            {
                xAxisLabels.Add(DateTime.Now.AddDays(-i).ToString("dd/MM"));
            }

            result.Add(xAxisLabels, weatherValues);
            return result;
        }

        /// <summary>
        /// Calculate chunked averages for 'data' in 'datapoints' segments.
        /// </summary>
        private static List<double> CalculateAverages(List<double>? data, int datapoints)
        {
            // Defensive checks
            if (data == null || data.Count == 0)
                return new List<double>();

            if (datapoints <= 0)
                throw new ArgumentException("Datapoints must be > 0.", nameof(datapoints));

            if (datapoints > data.Count)
                return Enumerable.Repeat(-100.0, datapoints).ToList();

            var averages = new List<double>();
            for (int i = 0; i < datapoints; i++)
            {
                int sliceSize = data.Count / datapoints;
                int startIndex = i * sliceSize;

                if (startIndex + sliceSize <= data.Count)
                {
                    var slice = data.GetRange(startIndex, sliceSize);
                    averages.Add(slice.Average());
                }
                else
                {
                    averages.Add(-100.0);
                }
            }
            return averages;
        }
    }