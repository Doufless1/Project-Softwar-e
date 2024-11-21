using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using sql_fetcher;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Axis = LiveChartsCore.SkiaSharpView.Axis;


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

        public Axis[] XAxes { get; private set; }

        //Data
        public string CurrentDay { get; set; } //Based on this we can access which day it is, so for last 7 days it is CurrentDay - 7, CurrentDay -6 ... CurrentDay
        public double CurrentTemperature { get; private set; }                 //Current temperature
        public List<double> DayTemperature { get; private set; }               //All temperature data of today
        public List<double> WeekTemperature { get; private set; }              //All temperature data of this week
        public List<double> HourlyDayTemperatureAverage { get; private set; }  //Average temperature of today (useful for graphing)
        public List<double> DailyWeekTemperatureAverage { get; private set; }  //Average humidity of this week (useful for graphing)
        
        public double CurrentHumidity { get; private set; }                    //Current humidity
        public List<double> DayHumidity { get; private set; }                  //All humidity of today
        public List<double> WeekHumidity { get; private set; }                 //All humidity of this week
        public List<double> HourlyDayHumidityAverage { get; private set; }     //Average humidity of today (useful for graphing)
        public List<double> DailyWeekHumidityAverage { get; private set; }     //Average humidity of this week (useful for graphing)
        
        
        public MainWindow()
        {
            //Initialize everything
            InitializeComponent();
            DataContext = this;
            CurrentDay = DateTime.Now.DayOfWeek.ToString();
            
            HourlyDayTemperatureAverage = new List<double>();
            HourlyDayHumidityAverage = new List<double>();
            
            DailyWeekTemperatureAverage = new List<double>();
            DailyWeekHumidityAverage = new List<double>();

            //Simple GetData
            CurrentTemperature = DataAccess.GetData(AccesableData.CurrentTemperature)[0];
            DayTemperature = DataAccess.GetData(AccesableData.DayTemperature);
            WeekTemperature = DataAccess.GetData(AccesableData.WeekTemperature);
            
            CurrentHumidity = DataAccess.GetData(AccesableData.CurrentHumidity)[0];
            DayHumidity = DataAccess.GetData(AccesableData.DayHumidity);
            WeekHumidity = DataAccess.GetData(AccesableData.CurrentHumidity);

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
            
            XAxes =
            [
                new Axis
                {
                    Labels =
                    [
                        "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
                    ],
                    Name = "Time"
                }
            ];
        }
    }
}