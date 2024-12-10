using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Windows.Devices.SmartCards;
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
        public List<ISeries> PressureDaySeries { get; set; }
        
        public List<ISeries> TemperatureWeekSeries { get; private set; }
        public List<ISeries> HumidityWeekSeries { get; private set; }
        public List<ISeries> LightWeekSeries { get; set; }
        public List<ISeries> PressureWeekSeries { get; set; }

        public List<ISeries> TemperatureMonthSeries { get; private set; }
        public List<ISeries> HumidityMonthSeries { get; private set; }
        public List<ISeries> LightMonthSeries { get; set; }
        public List<ISeries> PressureMonthSeries { get; set; }

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
        public double CurrentPressure { get; set; }
        
        public double CurrentBatteryPercentage { get; set; }
        public double CurrentSignalToNoiseRatio { get; set; }
        public double CurrentModelId { get; set; }
        public double CurrentBatteryVoltage { get; set; }
        
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
            PressureDaySeries = new List<ISeries>();
                
            TemperatureWeekSeries = new List<ISeries>();
            HumidityWeekSeries = new List<ISeries>();
            LightWeekSeries = new List<ISeries>();
            PressureWeekSeries = new List<ISeries>();
                
            TemperatureMonthSeries = new List<ISeries>();
            HumidityMonthSeries = new List<ISeries>();
            LightMonthSeries = new List<ISeries>();
            PressureMonthSeries = new List<ISeries>();
            
            GraphData graphDataObject = new GraphData();
            
            graphData = new Dictionary<Locations, Dictionary<FrontendReadyData, List<double>>>();
            Enum.GetValues(typeof(Locations)).Cast<Locations>().Select(location => new LocationEnum(location)).ToList().ForEach(location => graphData[location.GetLocationEnum()] = new Dictionary<FrontendReadyData, List<double>>());
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
                    RefreshData();

                    CurrentLocationBlock.Children.Clear();
                    foreach(Locations location in CurrentLocations)
                    {
                        TextBox tb = new TextBox
                        {
                            Text =
                                $"Current temperature in {location} is: {graphData[current_location][FrontendReadyData.CurrentTemperature].FirstOrDefault()} °C\n" +
                                $"Current humidity in {location} is: {graphData[current_location][FrontendReadyData.CurrentHumidity].FirstOrDefault()} %\n" +
                                $"Current luminosity in {location} is: {graphData[current_location][FrontendReadyData.CurrentLight].FirstOrDefault()} %\n" +
                                $"Current pressure in {location} is: {graphData[current_location][FrontendReadyData.CurrentPressure].FirstOrDefault()} Pa\n"
                        };
                        CurrentLocationBlock.Children.Add(tb);
                    }
                };
                
                MenuItem batteryStatus = new MenuItem();
                batteryStatus.Header = "Battery Status";
                batteryStatus.Click += (sender, args) =>
                {
                    MessageBox.Show("Battery Status: " +
                                    graphData[current_location][FrontendReadyData.BatteryVoltage].FirstOrDefault());
                };
                    
                MenuItem snr = new MenuItem();
                snr.Header = "Signal to Noise Ratio";
                snr.Click += (sender, args) =>
                {
                    MessageBox.Show("Signal to Noise Ratio: " +
                                    graphData[current_location][FrontendReadyData.SignalToNoiseRatio].FirstOrDefault());
                };
                
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Items.Add(batteryStatus);
                contextMenu.Items.Add(snr);
                
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
            CurrentPressure = 0;
            
            CurrentBatteryPercentage = 0;
            CurrentSignalToNoiseRatio = 0;
            CurrentModelId = 0;
            CurrentBatteryVoltage = 0;
            
            TemperatureDaySeries.Clear();
            HumidityDaySeries.Clear();
            LightDaySeries.Clear();
            PressureDaySeries.Clear();
                
            TemperatureWeekSeries.Clear();
            HumidityWeekSeries.Clear();
            LightWeekSeries.Clear();
            PressureWeekSeries.Clear();
                
            TemperatureMonthSeries.Clear();
            HumidityMonthSeries.Clear();
            LightMonthSeries.Clear();
            PressureMonthSeries.Clear();
            
            // Initialize chart series
            foreach (Locations location in CurrentLocations)
            {
                CurrentTemperature = graphData[location][FrontendReadyData.CurrentTemperature].FirstOrDefault();
                CurrentHumidity = graphData[location][FrontendReadyData.CurrentHumidity].FirstOrDefault();
                CurrentLight = graphData[location][FrontendReadyData.CurrentLight].FirstOrDefault();
                CurrentPressure = graphData[location][FrontendReadyData.CurrentPressure].FirstOrDefault();
                
                CurrentBatteryPercentage = graphData[location][FrontendReadyData.BatteryPercentage].FirstOrDefault();
                CurrentSignalToNoiseRatio = graphData[location][FrontendReadyData.SignalToNoiseRatio].FirstOrDefault();
                CurrentModelId = graphData[location][FrontendReadyData.ModelId].FirstOrDefault();
                CurrentBatteryVoltage = graphData[location][FrontendReadyData.BatteryVoltage].FirstOrDefault();
                
                TemperatureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayTemperatureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Temperature {location.ToString()} (°C)",
                    });

                HumidityDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayHumidityAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Humidity {location.ToString()} (%)",
                    }
                );
                
                PressureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayPressureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location.ToString()} (Pa)",
                    }
                );
                
                LightDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            graphData[location][FrontendReadyData.HourlyDayLightAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location.ToString()} (%)",
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
                        Name = $"Temperature {location.ToString()} (°C)",
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
                        Name = $"Humidity {location.ToString()} (%)",
                    }
                );
                
                PressureWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyWeekPressureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location.ToString()} (Pa)",
                    }
                );
                
                LightWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyWeekLightAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location.ToString()} (%)",
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
                        Name = $"Temperature {location.ToString()} (°C)",
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
                        Name = $"Humidity {location.ToString()} (%)",
                    }
                );
                
                LightMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyMonthLightAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location.ToString()} (%)",
                    }
                );
                
                PressureMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(graphData[location][FrontendReadyData.DailyMonthPressureAverage]),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location.ToString()} (Pa)",
                    }
                );
            }
        }
    }
}
