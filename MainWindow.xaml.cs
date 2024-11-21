using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using sql_fetcher;

namespace Weather_App
{
    public partial class MainWindow : Window
    {
        //TODO: Fill in how much datapoints per hour are fetched into the database
        private readonly int DatapointsPerHour = 15;                         //Placeholder
        private static readonly sql_fetcher.DataAccess DataAccess = new DataAccess();
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
        }
    }
}