using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        public List<ISeries> TemperatureDaySeries { get; private set; }
        public List<ISeries> HumidityDaySeries { get; private set; }
        public List<ISeries> LightDaySeries { get; set; }
        public List<ISeries> TemperatureWeekSeries { get; private set; }
        public List<ISeries> HumidityWeekSeries { get; private set; }
        public List<ISeries> LightWeekSeries { get; set; }

        public List<ISeries> TemperatureMonthSeries { get; private set; }
        public List<ISeries> HumidityMonthSeries { get; private set; }
        public List<ISeries> LightMonthSeries { get; set; }

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
        public double CurrentLight { get; set; }
        
        public List<Locations> CurrentLocations { get; set; }
        public List<Button> LocationButtons { get; private set; }
        public Dictionary<Locations, Dictionary<FrontendReadyData, List<double>>> graphData { get; set; }

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
            
            TemperatureDaySeries = new List<ISeries>();
            HumidityDaySeries = new List<ISeries>();
            LightDaySeries = new List<ISeries>();
                
            TemperatureWeekSeries = new List<ISeries>();
            HumidityWeekSeries = new List<ISeries>();
            LightWeekSeries = new List<ISeries>();
                
            TemperatureMonthSeries = new List<ISeries>();
            HumidityMonthSeries = new List<ISeries>();
            LightMonthSeries = new List<ISeries>();
            
            GraphData graphDataObject = new GraphData();
            
            graphData = new Dictionary<Locations, Dictionary<FrontendReadyData, List<double>>>();
            foreach (Locations location in Locations.GetValues(typeof(Locations)))
            {
                graphData[location] = graphDataObject.FetchGraphData(location);
            }
            RefreshData();

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
                    if (CurrentLocations.Contains(current_location))
                    {
                        CurrentLocations.Remove(current_location);
                        button.Background = Brushes.LightGray;
                    }
                    else
                    {
                        CurrentLocations.Add(current_location);
                        button.Background = Brushes.LightBlue;
                    }
                    CurrentLocationBlock.Text = "";
                    foreach(Locations location in CurrentLocations)
                    {
                        CurrentLocationBlock.Text += location.ToString() + " ";
                    }
                    RefreshData();
                };
                
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Battery Status";
                menuItem.Click += (sender, args) =>
                {
                    MessageBox.Show("Battery Status: " +
                                    graphData[current_location][FrontendReadyData.BatteryStatus].FirstOrDefault());
                };
                    
                MenuItem menuItem2 = new MenuItem();
                menuItem2.Header = "Signal to Noise Ratio";
                menuItem2.Click += (sender, args) =>
                {
                    MessageBox.Show("Signal to Noise Ratio: " +
                                    graphData[current_location][FrontendReadyData.SignalToNoiseRatio].FirstOrDefault());
                };
                    
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Items.Add(menuItem);
                contextMenu.Items.Add(menuItem2);
                
                button.MouseRightButtonDown += (sender, args) =>
                {
                    contextMenu.PlacementTarget = button;
                    contextMenu.IsOpen = true;
                };
                LocationButtons.Add(button);
                LocationStackPanel.Children.Add(button);
            }
        }

        void RefreshData()
        {
            CurrentTemperature = 0;
            CurrentHumidity = 0;
            CurrentLight = 0;
            
            TemperatureDaySeries.Clear();
            HumidityDaySeries.Clear();
            LightDaySeries.Clear();
                
            TemperatureWeekSeries.Clear();
            HumidityWeekSeries.Clear();
            LightWeekSeries.Clear();
                
            TemperatureMonthSeries.Clear();
            HumidityMonthSeries.Clear();
            LightMonthSeries.Clear();
            
            // Initialize chart series
            foreach (Locations location in CurrentLocations)
            {
                CurrentTemperature = graphData[location][FrontendReadyData.CurrentTemperature].FirstOrDefault();
                CurrentHumidity = graphData[location][FrontendReadyData.CurrentHumidity].FirstOrDefault();
                CurrentLight = graphData[location][FrontendReadyData.CurrentLight].FirstOrDefault();
                
                TemperatureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    });

                HumidityDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                );

                TemperatureWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyWeekTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    }
                );
                
                HumidityWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyWeekHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                );

                TemperatureMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(
                                graphData[location][FrontendReadyData.DailyMonthTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Temperature (°C)",
                    }
                );

                HumidityMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyMonthHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = "Humidity (%)",
                    }
                );
            }
        }
    }
}
