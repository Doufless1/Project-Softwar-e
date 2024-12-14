using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly int DatapointsPerHour = 1; // Placeholder
        private static readonly DataAccess DataAccess = new DataAccess();

        // Graph Properties
        public List<ISeries> InsideTemperatureDaySeries { get; private set; }
        public List<ISeries> OutsideTemperatureDaySeries { get; private set; }
        public List<ISeries> HumidityDaySeries { get; private set; }
        public List<ISeries> LightDaySeries { get; set; }
        public List<ISeries> PressureDaySeries { get; set; }
        public List<ISeries> LuminosityDaySeries { get; set; }

        public List<ISeries> InsideTemperatureWeekSeries { get; private set; }
        public List<ISeries> OutsideTemperatureWeekSeries { get; private set; }
        public List<ISeries> HumidityWeekSeries { get; private set; }
        public List<ISeries> LightWeekSeries { get; set; }
        public List<ISeries> PressureWeekSeries { get; set; }
        public List<ISeries> LuminosityWeekSeries { get; set; }

        public List<ISeries> InsideTemperatureMonthSeries { get; private set; }
        public List<ISeries> OutsideTemperatureMonthSeries { get; private set; }
        public List<ISeries> HumidityMonthSeries { get; private set; }
        public List<ISeries> LightMonthSeries { get; set; }
        public List<ISeries> PressureMonthSeries { get; set; }
        public List<ISeries> LuminosityMonthSeries { get; set; }

        public List<Axis> XAxesDay { get; set; }
        public List<Axis> XAxesWeek { get; set; }
        public List<Axis> XAxesMonth { get; set; }

        // Data
        public string CurrentDay { get; set; }
        public DateTime[] Last24Hours { get; private set; }
        public DateTime[] Last7Days { get; private set; }
        public DateTime[] Last30Days { get; private set; }
        public double CurrentInsideTemperature { get; set; }
        public double CurrentOutsideTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public double CurrentLight { get; set; }
        public double CurrentPressure { get; set; }


        public double CurrentSignalToNoiseRatio { get; set; }
        public double CurrentModelId { get; set; }
        public double CurrentBatteryVoltage { get; set; }
        
        public double CurrentBatteryPercentage { get; set; }

        public List<string> CurrentLocations { get; set; }
        public List<Button> LocationButtons { get; private set; }
        public Dictionary<string, Dictionary<FrontendReadyData, List<double>>> GraphData { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<AccesableData, double>>> GatewayData { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private double _selectedBatteryPercentage;
        
        private double _batteryWidth;
        public double BatteryWidth
        {
            get => _batteryWidth;
            set
            {
                _batteryWidth = value;
                RaisePropertyChanged(nameof(BatteryWidth));
            }
        }
        
        private Brush _batteryColor = Brushes.Green;
        public Brush BatteryColor
        {
            get => _batteryColor;
            set
            {
                _batteryColor = value;
                RaisePropertyChanged(nameof(BatteryColor));
            }
        }

// Helper Method to Update Color
        private void UpdateBatteryColor(double percentage)
        {
            if (percentage > 75)
                BatteryColor = Brushes.Green;
            else if (percentage > 50)
                BatteryColor = Brushes.Yellow;
            else if (percentage > 25)
                BatteryColor = Brushes.Orange;
            else
                BatteryColor = Brushes.Red;
        }

        public double SelectedBatteryPercentage
        {
            get => _selectedBatteryPercentage;
            set
            {
                    _selectedBatteryPercentage = value;
                    RaisePropertyChanged(nameof(SelectedBatteryPercentage));
                    RaisePropertyChanged(nameof(SelectedBatteryPercentageText));
                    
                    BatteryWidth = (_selectedBatteryPercentage / 100) * 100; // Max width of 80
                    UpdateBatteryColor(_selectedBatteryPercentage);
            }
        }

        public string SelectedBatteryPercentageText => $"{SelectedBatteryPercentage:F2}%";
        
    


    public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SelectedBatteryPercentage = 50;

            // Initialize data
            CurrentDay = DateTime.Now.DayOfWeek.ToString();
            Last24Hours = Enumerable.Range(0, 24).Select(i => DateTime.Now.AddHours(-i)).Reverse().ToArray();
            Last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(-i)).Reverse().ToArray();
            Last30Days = Enumerable.Range(0, 30).Select(i => DateTime.Now.AddDays(-i)).Reverse().ToArray();
            LocationButtons = new List<Button>();
            CurrentLocations = new List<string>();
            // CurrentLocations.Add(Locations.Wierden);

            InsideTemperatureDaySeries = new List<ISeries>();
            OutsideTemperatureDaySeries = new List<ISeries>();
            HumidityDaySeries = new List<ISeries>();
            LightDaySeries = new List<ISeries>();
            PressureDaySeries = new List<ISeries>();

            InsideTemperatureWeekSeries = new List<ISeries>();
            OutsideTemperatureWeekSeries = new List<ISeries>();
            HumidityWeekSeries = new List<ISeries>();
            LightWeekSeries = new List<ISeries>();
            PressureWeekSeries = new List<ISeries>();

            InsideTemperatureMonthSeries = new List<ISeries>();
            OutsideTemperatureMonthSeries = new List<ISeries>();
            HumidityMonthSeries = new List<ISeries>();
            LightMonthSeries = new List<ISeries>();
            PressureMonthSeries = new List<ISeries>();

            GraphData graphDataObject = new GraphData();

            GraphData = new Dictionary<string, Dictionary<FrontendReadyData, List<double>>>();
            GatewayData = new Dictionary<string, Dictionary<string, Dictionary<AccesableData, double>>>();
            
            foreach (string location in Devices.GetDevices())
            {
                GraphData[location] = graphDataObject.FetchGraphData(location);
                GatewayData[location] = graphDataObject.FetchGatewayData(location);
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
            // Create location buttons
            foreach (string location in Devices.GetDevices())
            {
                var current_location = location;
                Button button = new Button
                {
                    Content = current_location,
                    Width = Double.NaN,
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
                    foreach(string location in CurrentLocations)
                    {
                        TextBox tb = new TextBox
                        {
                            Text =
                                $"{location.ToUpper()} \n" +
                                $"Current inside temperature: {(GraphData[location][FrontendReadyData.CurrentInsideTemperature].FirstOrDefault() == -100 ? "N/A" : GraphData[location][FrontendReadyData.CurrentInsideTemperature].FirstOrDefault().ToString())} °C\n" +
                                $"Current outside temperature: {(GraphData[location][FrontendReadyData.CurrentOutsideTemperature].FirstOrDefault() == -100 ? "N/A" : GraphData[location][FrontendReadyData.CurrentOutsideTemperature].FirstOrDefault().ToString())} °C\n" +
                                $"Current humidity: {(GraphData[location][FrontendReadyData.CurrentHumidity].FirstOrDefault() == -100 ? "N/A" : GraphData[location][FrontendReadyData.CurrentHumidity].FirstOrDefault().ToString())} %\n" +
                                $"Current luminosity: {(GraphData[location][FrontendReadyData.CurrentLight].FirstOrDefault() == -100 ? "N/A" : GraphData[location][FrontendReadyData.CurrentLight].FirstOrDefault().ToString())} %\n" +
                                $"Current pressure: {(GraphData[location][FrontendReadyData.CurrentPressure].FirstOrDefault() == -100 ? "N/A" : GraphData[location][FrontendReadyData.CurrentPressure].FirstOrDefault().ToString())} Pa\n"                        
                        };
                        CurrentLocationBlock.Children.Add(tb);
                    }
                };
                
                MenuItem batteryStatus = new MenuItem();
                batteryStatus.Header = "Battery Status";
                batteryStatus.Click += (sender, args) =>
                {
                    if (GraphData[current_location].ContainsKey(FrontendReadyData.BatteryPercentage))
                    {
                        SelectedBatteryPercentage = GraphData[current_location][FrontendReadyData.BatteryPercentage].FirstOrDefault();
                    }
                    else
                    {
                        SelectedBatteryPercentage = 0; // Default value
                    }

                    Console.WriteLine($"This is the battery %: {SelectedBatteryPercentage}");
                    MessageBox.Show($"Battery Percentage for {current_location}: {SelectedBatteryPercentage:F2}%", "Battery Percentage", MessageBoxButton.OK, MessageBoxImage.Information);
                };
                
                MenuItem gateways = new MenuItem();
                gateways.Header = "Gateways";
                gateways.Click += (sender, args) =>
                {
                    string message = $"{location} is currently connected to the following gateways:\n";
                    foreach (var gateway in GatewayData[location])
                    {
                        message += $"{gateway.Key}:\n";
                        message += $"\t Latitude {gateway.Value[AccesableData.Latitude]}\n";
                        message += $"\t Longitude {gateway.Value[AccesableData.Longitude]}\n";
                        message += $"\t Altitude {gateway.Value[AccesableData.Altitude]}\n";
                        message += $"\t Average RSSI {gateway.Value[AccesableData.AvgRssi]}\n";
                        message += $"\t Max RSSI {gateway.Value[AccesableData.MaxRssi]}\n";
                        message += $"\t Min RSSI {gateway.Value[AccesableData.MinRssi]}\n";
                        message += $"\t Average SNR {gateway.Value[AccesableData.AvgSnr]}\n";
                        message += $"\t Max SNR {gateway.Value[AccesableData.MaxSnr]}\n";
                        message += $"\t Min SNR {gateway.Value[AccesableData.MinSnr]}\n";
                    }
                    MessageBox.Show(message, "Gateways", MessageBoxButton.OK, MessageBoxImage.Information);
                };
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Items.Add(batteryStatus);
                contextMenu.Items.Add(gateways);

                button.MouseRightButtonDown += (sender, args) =>
                {
                    contextMenu.PlacementTarget = button;
                    contextMenu.IsOpen = true;
                };
                LocationButtons.Add(button);
                LocationStackPanel.Children.Add(button);
            }
        }
        
    
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        void RefreshData()
        {
            CurrentInsideTemperature = 0;
            CurrentOutsideTemperature = 0;
            CurrentHumidity = 0;
            CurrentLight = 0;
            CurrentPressure = 0;

            CurrentBatteryPercentage = 0;
            SelectedBatteryPercentage = 0;
            CurrentSignalToNoiseRatio = 0;
            CurrentModelId = 0;
            CurrentBatteryVoltage = 0;

            InsideTemperatureDaySeries.Clear();
            OutsideTemperatureDaySeries.Clear();
            HumidityDaySeries.Clear();
            LightDaySeries.Clear();
            PressureDaySeries.Clear();

            InsideTemperatureWeekSeries.Clear();
            OutsideTemperatureWeekSeries.Clear();
            HumidityWeekSeries.Clear();
            LightWeekSeries.Clear();
            PressureWeekSeries.Clear();

            InsideTemperatureMonthSeries.Clear();
            OutsideTemperatureMonthSeries.Clear();
            HumidityMonthSeries.Clear();
            LightMonthSeries.Clear();
            PressureMonthSeries.Clear();

            // Initialize chart series
            foreach (string location in CurrentLocations)
            {
                CurrentInsideTemperature = GraphData[location][FrontendReadyData.CurrentInsideTemperature].FirstOrDefault();
                CurrentOutsideTemperature = GraphData[location][FrontendReadyData.CurrentInsideTemperature].FirstOrDefault();
                CurrentHumidity = GraphData[location][FrontendReadyData.CurrentHumidity].FirstOrDefault();
                CurrentLight = GraphData[location][FrontendReadyData.CurrentLight].FirstOrDefault();
                CurrentPressure = GraphData[location][FrontendReadyData.CurrentPressure].FirstOrDefault();

                CurrentBatteryPercentage = GraphData[location][FrontendReadyData.BatteryPercentage].FirstOrDefault();
                CurrentModelId = GraphData[location][FrontendReadyData.ModelId].FirstOrDefault();
                CurrentBatteryVoltage = GraphData[location][FrontendReadyData.BatteryVoltage].FirstOrDefault();

                InsideTemperatureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            GraphData[location][FrontendReadyData.HourlyDayInsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Inside temperature {location} (°C)",
                    });
                
                OutsideTemperatureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            GraphData[location][FrontendReadyData.HourlyDayOutsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Outside temperature {location} (°C)",
                    }
                );

                HumidityDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            GraphData[location][FrontendReadyData.HourlyDayHumidityAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Humidity {location} (%)",
                    }
                );

                PressureDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            GraphData[location][FrontendReadyData.HourlyDayPressureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location} (Pa)",
                    }
                );

                LightDaySeries.Add(
                    new LineSeries<double>
                    {
                        Values = new ChartValues<double>(
                            GraphData[location][FrontendReadyData.HourlyDayLightAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location} (lux)",
                    }
                );

                InsideTemperatureWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyWeekInsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Inside temperature {location} (°C)",
                    }
                );
                
                OutsideTemperatureWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyWeekOutsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Outside temperature {location} (°C)",
                    }
                );

                HumidityWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyWeekHumidityAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Humidity {location} (%)",
                    }
                );

                PressureWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyWeekPressureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location} (Pa)",
                    }
                );

                LightWeekSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyWeekLightAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location} (lux)",
                    }
                );

                InsideTemperatureMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(
                                GraphData[location][FrontendReadyData.DailyMonthInsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Inside temperature {location} (°C)",
                    }
                );
                
                OutsideTemperatureMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(
                                GraphData[location][FrontendReadyData.DailyMonthOutsideTemperatureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Outside temperature {location} (°C)",
                    }
                );

                HumidityMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyMonthHumidityAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Humidity {location} (%)",
                    }
                );

                LightMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyMonthLightAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Luminosity {location} (lux)",
                    }
                );

                PressureMonthSeries.Add(
                    new LineSeries<double>
                    {
                        Values =
                            new ChartValues<double>(GraphData[location][FrontendReadyData.DailyMonthPressureAverage].Where(value => value != -100)),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red),
                        GeometrySize = 10,
                        Name = $"Pressure {location} (Pa)",
                    }
                );
            }
        }
    }
}