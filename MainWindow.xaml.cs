using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using sql_fetcher;
using Backend;
using enums;

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
        
        public List<Locations> CurrentLocations { get; set; }
        public List<Button> LocationButtons { get; private set; }
        public Dictionary<Locations, Dictionary<GraphDataEnum, List<double>>> graphData { get; set; }

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
            CurrentLocations = new List<Locations>();
            CurrentLocations.Add(Locations.Wierden);
            graphData = new Dictionary<Locations, Dictionary<GraphDataEnum, List<double>>>();
            
            // Fetch data and handle null cases
            //TODO: Add multiple location support for current temperature and humidity
            CurrentTemperature = DataAccess.GetData(AccesableData.CurrentTemperature, Locations.Wierden)?.FirstOrDefault() ?? 0;
            CurrentHumidity = DataAccess.GetData(AccesableData.CurrentHumidity, Locations.Wierden)?.FirstOrDefault() ?? 0;
            
            // Fetch graph data
            void RefreshData()
            {
                foreach (Locations location in Enum.GetValues(typeof(Locations)))
                {
                    graphData[location] = new GraphData().FetchGraphData(location);
                }
            }
            RefreshData();
            
            
            // Initialize chart series
            foreach (Locations location in CurrentLocations)
            {
                TemperatureDaySeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.HourlyDayTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    }
                };
            }
            
            foreach (Locations location in CurrentLocations)
            {
                HumidityDaySeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.HourlyDayHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                };
            }
            
            foreach (Locations location in CurrentLocations)
            {
                TemperatureWeekSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.DailyWeekTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    }
                };
            }
            

            foreach (Locations location in CurrentLocations)
            {
                HumidityWeekSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.DailyWeekHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                };
            }

            foreach (Locations location in CurrentLocations)
            {
                TemperatureMonthSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.DailyMonthTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    }
                };
            }

            foreach (Locations location in CurrentLocations)
            {
                HumidityMonthSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][GraphDataEnum.DailyMonthHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                };
            }

            // Initialize axes
            XAxesDay = new List<Axis>
            {
                new Axis
                {
                    Labels = Last24Hours.Select(x => x.ToString("HH:mm")).ToArray(),
                    Name = "Time (HH:MM)"
                }
            };

            XAxesWeek = new List<Axis>
            {
                new Axis
                {
                    Labels = Last7Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Date (DD/MM)"
                }
            };

            XAxesMonth = new List<Axis>
            {
                new Axis
                {
                    Labels = Last30Days.Select(x => x.ToString("dd/MM")).ToArray(),
                    Name = "Date (DD/MM)"
                }
            };

            // Create location buttons
            foreach (Locations location in Enum.GetValues(typeof(Locations)))
            {
                var current_location = location;
                Button button = new Button
                {
                    Content = current_location,
                    Width = 100,
                    Height = 50,
                };
                button.Click += (sender, args) =>
                {
                    if(CurrentLocations.Contains(current_location)) 
                        CurrentLocations.Remove(current_location);
                    else 
                        CurrentLocations.Add(current_location);
                    
                    CurrentLocationBlock.Text = "";
                    foreach(Locations location in CurrentLocations)
                    {
                        CurrentLocationBlock.Text += location.ToString() + " ";
                    }
                    if(button.Background == Brushes.LightBlue)
                        button.Background = Brushes.LightGray;
                    else button.Background = Brushes.LightBlue;
                };
                LocationButtons.Add(button);
                LocationStackPanel.Children.Add(button);
                RefreshData();
            }
        }


    }
}
