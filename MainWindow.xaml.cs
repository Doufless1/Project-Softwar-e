using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using sql_fetcher;

namespace Weather_App
{
    public partial class MainWindow : Window
    {
        private readonly int DatapointsPerHour = 1; // Placeholder
        private static readonly DataAccess DataAccess = new DataAccess();

        // Graph Properties
        public ISeries[] TemperatureDaySeries { get; private set; }
        public ISeries[] HumidityDaySeries { get; private set; }
        public ISeries[] TemperatureWeekSeries { get; private set; }
        public ISeries[] HumidityWeekSeries { get; private set; }
        public ISeries[] TemperatureMonthSeries { get; private set; }
        public ISeries[] HumidityMonthSeries { get; private set; }

        public List<Axis> XAxesDay { get; set; }
        public List<Axis> XAxesWeek { get; set; }
        public List<Axis> XAxesMonth { get; set; }

        // Data
        public string CurrentDay { get; set; }
        public DateTime[] Last24Hours { get; private set; }
        public DateTime[] Last7Days { get; private set; }
        public DateTime[] Last30Days { get; private set; }
        public double CurrentTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public List<Button> LocationButtons { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize data
            CurrentDay = DateTime.Now.DayOfWeek.ToString();
            Last24Hours = Enumerable.Range(0, 24).Select(i => DateTime.Now.AddHours(-i)).Reverse().ToArray();
            Last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(-i)).Reverse().ToArray();
            Last30Days = Enumerable.Range(0, 30).Select(i => DateTime.Now.AddDays(-i)).Reverse().ToArray();
            LocationButtons = new List<Button>();

            // Fetch data and handle null cases
            CurrentTemperature = DataAccess.GetData(AccesableData.CurrentTemperature, Locations.Enschede)?.FirstOrDefault() ?? 0;
            CurrentHumidity = DataAccess.GetData(AccesableData.CurrentHumidity, Locations.Enschede)?.FirstOrDefault() ?? 0;

            var DayTemperature = DataAccess.GetData(AccesableData.DayTemperature, Locations.Enschede) ?? new List<double>();
            var WeekTemperature = DataAccess.GetData(AccesableData.WeekTemperature, Locations.Enschede) ?? new List<double>();
            var MonthTemperature = DataAccess.GetData(AccesableData.MonthTemperature, Locations.Enschede) ?? new List<double>();

            var DayHumidity = DataAccess.GetData(AccesableData.DayHumidity, Locations.Enschede) ?? new List<double>();
            var WeekHumidity = DataAccess.GetData(AccesableData.WeekHumidity, Locations.Enschede) ?? new List<double>();
            var MonthHumidity = DataAccess.GetData(AccesableData.MonthHumidity, Locations.Enschede) ?? new List<double>();

            // Calculate averages safely
            var HourlyDayTemperatureAverage = CalculateHourlyAverages(DayTemperature, 24);
            var DailyWeekTemperatureAverage = CalculateDailyAverages(WeekTemperature, 7);
            var DailyMonthTemperatureAverage = CalculateDailyAverages(MonthTemperature, 30);

            var HourlyDayHumidityAverage = CalculateHourlyAverages(DayHumidity, 24);
            var DailyWeekHumidityAverage = CalculateDailyAverages(WeekHumidity, 7);
            var DailyMonthHumidityAverage = CalculateDailyAverages(MonthHumidity, 30);

            // Initialize chart series
            TemperatureDaySeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(HourlyDayTemperatureAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red),
                    GeometrySize = 10,
                    Name = "Temperature"
                }
            };

            HumidityDaySeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(HourlyDayHumidityAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.SkyBlue),
                    GeometrySize = 10,
                    Name = "Humidity"
                }
            };

            TemperatureWeekSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(DailyWeekTemperatureAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red),
                    GeometrySize = 10,
                    Name = "Temperature"
                }
            };

            HumidityWeekSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(DailyWeekHumidityAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.SkyBlue),
                    GeometrySize = 10,
                    Name = "Humidity"
                }
            };

            TemperatureMonthSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(DailyMonthTemperatureAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red),
                    GeometrySize = 10,
                    Name = "Temperature"
                }
            };

            HumidityMonthSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new ChartValues<double>(DailyMonthHumidityAverage),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.SkyBlue),
                    GeometrySize = 10,
                    Name = "Humidity"
                }
            };

            // Initialize axes
            XAxesDay = new List<Axis>
            {
                new Axis
                {
                    Labels = Last24Hours.Select(x => x.ToString("HH:mm")).ToArray(),
                    Name = "Time"
                }
            };

            XAxesWeek = new List<Axis>
            {
                new Axis
                {
                    Labels = Last7Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Time"
                }
            };

            XAxesMonth = new List<Axis>
            {
                new Axis
                {
                    Labels = Last30Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Time"
                }
            };

            // Create location buttons
            foreach (Locations location in Enum.GetValues(typeof(Locations)))
            {
                Button button = new Button
                {
                    Content = location.ToString(),
                    Width = 100,
                    Height = 50
                };
                LocationButtons.Add(button);
                LocationStackPanel.Children.Add(button); // Ensure LocationStackPanel is not null
            }
        }

        private static List<double> CalculateHourlyAverages(List<double> data, int hours)
        {
            var averages = new List<double>();
            for (int i = 0; i < hours; i++)
            {
                if (data.Count > i)
                {
                    averages.Add(data.GetRange(i * (data.Count / hours), data.Count / hours).Average());
                }
                else
                {
                    averages.Add(0);
                }
            }
            return averages;
        }

        private static List<double> CalculateDailyAverages(List<double> data, int days)
        {
            var averages = new List<double>();
            for (int i = 0; i < days; i++)
            {
                if (data.Count > i)
                {
                    averages.Add(data.GetRange(i * (data.Count / days), data.Count / days).Average());
                }
                else
                {
                    averages.Add(0);
                }
            }
            return averages;
        }
    }
}