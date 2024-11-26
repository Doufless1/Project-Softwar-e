using System.Windows;
using sql_fetcher;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Windows.Controls;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using Axis = LiveChartsCore.SkiaSharpView.Axis;
using Button = System.Windows.Controls.Button;


namespace Weather_App
{
    public partial class MainWindow : Window
    {
        //TODO: Fill in how much datapoints per hour are fetched into the database
        private readonly int DatapointsPerHour = 15;                         //Placeholder
        private static readonly sql_fetcher.DataAccess DataAccess = new DataAccess();
        
        //Graph stuff
        public ISeries[] TemperatureDaySeries { get; private set; }                    //Series for the graph
        public ISeries[] HumidityDaySeries { get; private set; }                       //Series for the graph
        public ISeries[] TemperatureWeekSeries { get; private set; }                    //Series for the graph
        public ISeries[] HumidityWeekSeries { get; private set; }                       //Series for the graph
        public ISeries[] TemperatureMonthSeries { get; private set; }                    //Series for the graph
        public ISeries[] HumidityMonthSeries { get; private set; }                       //Series for the graph
        public Axis[] XAxesDay { get; private set; }
        public Axis[] XAxesWeek { get; private set; }
        public Axis[] XAxesMonth { get; private set; }

        //Data
        public string CurrentDay { get; set; }                                  //Based on this we can access which day it is, so for last 7 days it is CurrentDay - 7, CurrentDay -6 ... CurrentDay
        public DateTime[] Last24Hours { get; private set; }                     //Last 24 hours
        public  DateTime[] Last7Days { get; private set; }                      //Last 7 days
        public DateTime[] Last30Days { get; private set; }                      //Last 30 days
        
        public double CurrentTemperature { get; private set; }                 //Current temperature
        public List<double> DayTemperature { get; private set; }               //All temperature data of today
        public List<double> WeekTemperature { get; private set; }              //All temperature data of this week
        public List<double> MonthTemperature { get; private set; }             //All temperature data of this month
        public List<double> HourlyDayTemperatureAverage { get; private set; }  //Average temperature of today (useful for graphing)
        public List<double> DailyWeekTemperatureAverage { get; private set; }  //Average humidity of this week (useful for graphing)
        public List<double> DailyMonthTemperatureAverage { get; private set; } //Average temperature of this month (useful for graphing)
        
        public double CurrentHumidity { get; private set; }                    //Current humidity
        public List<double> DayHumidity { get; private set; }                  //All humidity of today
        public List<double> WeekHumidity { get; private set; }                 //All humidity of this week
        public List<double> MonthHumidity { get; private set; }                //All humidity of this month
        public List<double> HourlyDayHumidityAverage { get; private set; }     //Average humidity of today (useful for graphing)
        public List<double> DailyWeekHumidityAverage { get; private set; }     //Average humidity of this week (useful for graphing)
        public List<double> DailyMonthHumidityAverage { get; private set; } //Average temperature of this month (useful for graphing)
        //GUI
        public List<Button> LocationButtons { get; private set; }               //Buttons for the locations
        
        public MainWindow()
        {
            //Initialize everything
            InitializeComponent();
            DataContext = this;
            CurrentDay = DateTime.Now.DayOfWeek.ToString();
            Last24Hours = new DateTime[24];
            Last7Days = new DateTime[7];
            Last30Days = new DateTime[30];
            LocationButtons = new List<Button>();
            
            HourlyDayTemperatureAverage = new List<double>();
            HourlyDayHumidityAverage = new List<double>();
            
            DailyWeekTemperatureAverage = new List<double>();
            DailyWeekHumidityAverage = new List<double>();
            
            DailyMonthTemperatureAverage = new List<double>();
            DailyMonthHumidityAverage = new List<double>();

            //Simple GetData
            CurrentTemperature = DataAccess.GetData(AccesableData.CurrentTemperature, Locations.Enschede)[0];
            DayTemperature = DataAccess.GetData(AccesableData.DayTemperature, Locations.Enschede);
            WeekTemperature = DataAccess.GetData(AccesableData.WeekTemperature, Locations.Enschede);
            MonthTemperature = DataAccess.GetData(AccesableData.MonthHumidity, Locations.Enschede);
            
            CurrentHumidity = DataAccess.GetData(AccesableData.CurrentHumidity, Locations.Enschede)[0];
            DayHumidity = DataAccess.GetData(AccesableData.DayHumidity, Locations.Enschede);
            WeekHumidity = DataAccess.GetData(AccesableData.CurrentHumidity, Locations.Enschede);
            MonthHumidity = DataAccess.GetData(AccesableData.CurrentHumidity, Locations.Enschede);
            
            //Calculate averages
            for (int i = 0; i < 24; i++)
            {
                HourlyDayTemperatureAverage.Add(DayTemperature.GetRange(i * DatapointsPerHour, DatapointsPerHour).Average());
                HourlyDayHumidityAverage.Add(DayHumidity.GetRange(i * DatapointsPerHour, DatapointsPerHour).Average());
            } //Gets the average of each hour over the day and adds it to the list of averages.

            for (int i = 0; i < 7; i++)
            {
                DailyWeekTemperatureAverage.Add(WeekTemperature.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
                DailyWeekHumidityAverage.Add(WeekHumidity.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
            } //Gets the average of each day over the week and adds it to the list of averages.
            for (int i = 0; i < 30; i++)
            {
                DailyMonthTemperatureAverage.Add(MonthTemperature.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
                DailyMonthHumidityAverage.Add(MonthHumidity.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
            } //Gets the average of each day over the month and adds it to the list of averages.
            
            for (int i = 23; i >= 0; i++)
            {
                Last24Hours[i] = DateTime.Now.AddHours(-i);
            }

            for (int i = 7; i >= 1; i++)
            {
                Last7Days[i] = DateTime.Now.AddDays(-i);
            }
            
            for (int i = 29; i >= 0; i++)
            {
                Last30Days[i] = DateTime.Now.AddDays(-i);
            }
            
            TemperatureDaySeries =
            [
                new LineSeries<double>
                {
                    Values = HourlyDayTemperatureAverage,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red),
                    GeometrySize = 10,
                    Name = "Temperature"
                }
            ];
            
            TemperatureWeekSeries =
            [
                new LineSeries<double>
                {
                    Values = DailyWeekTemperatureAverage,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red),
                    GeometrySize = 10,
                    Name = "Temperature"
                }
            ];
            
            TemperatureMonthSeries =
                [
                    new LineSeries<double>
                    {
                        Values = DailyMonthTemperatureAverage,
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature"
                    }
                ];
            
            HumidityDaySeries =
            [
                new LineSeries<double>
                {
                    Values = HourlyDayHumidityAverage,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.SkyBlue),
                    GeometrySize = 10,
                    Name = "Humidity"
                }
            ];
            
            HumidityWeekSeries =
            [
                new LineSeries<double>
                {
                    Values = DailyWeekHumidityAverage,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.SkyBlue),
                    GeometrySize = 10,
                    Name = "Humidity"
                }
            ];
            
            HumidityMonthSeries =
                [
                    new LineSeries<double>
                    {
                        Values = DailyMonthHumidityAverage,
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.SkyBlue),
                        GeometrySize = 10,
                        Name = "Humidity"
                    }
                ];
            
            XAxesDay =
            [
                new Axis
                {
                    Labels = Last24Hours.Select(x => x.ToString("HH:mm")).ToArray(),
                    Name = "Time"
                }
            ];
            
            XAxesWeek =
            [
                new Axis
                {
                    Labels = Last7Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Time"
                }
            ];
            
            XAxesMonth =
            [
                new Axis
                {
                    Labels = Last30Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Time"
                }
            ];

            foreach (Locations location in Enum.GetValues(typeof(Locations)))
            {
                Button button = new Button()
                {
                    Content = $"${location}",
                    Width = 100,
                    Height = 50
                };
                LocationButtons.Add(button);
                LocationStackPanel.Children.Add(button);
            }

        }
    }
}