using System.ComponentModel;
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
using LiveCharts;
using sql_fetcher;

namespace Weather_App
{
    public partial class MainWindow : Window, INotifyPropertyChanged // I added this but I need
    {

        //TODO: Fill in how much datapoints per hour are fetched into the database
        /*      private readonly int DatapointsPerHour = 15;                         //Placeholder
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
        */

        public MainWindow()
        {
            InitializeComponent();
            Sensor1Values = new ChartValues<double> { 50.3, 0.5, 0.0, 3.7, 2.8, 4.4 };
            Sensor2Values = new ChartValues<double> { 21.1, 22.0, 24.5, 26.0, 25.3, 22.7 };
            Sensor3Values = new ChartValues<double> { 20.5, 21.8, 23.0, 25.5, 23.8, 22.0 };
        }


        private ChartValues<double> _sensor1Values;
        public ChartValues<double> Sensor1Values
        {
            get { return _sensor1Values; }
            set
            {
                _sensor1Values = value;
                OnPropertyChanged(nameof(Sensor1Values));
            }
        }

        private ChartValues<double> _sensor2Values;
        public ChartValues<double> Sensor2Values
        {
            get { return _sensor2Values; }
            set
            {
                _sensor2Values = value;
                OnPropertyChanged(nameof(Sensor2Values));
            }
        }

        private ChartValues<double> _sensor3Values;
        public ChartValues<double> Sensor3Values
        {
            get { return _sensor3Values; }
            set
            {
                _sensor3Values = value;
                OnPropertyChanged(nameof(Sensor3Values));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
            /* try
             {
                 InitializeComponent();
                DataContext = this;

                 CurrentDay = DateTime.Now.DayOfWeek.ToString();

                 // Initialize Lists
                 HourlyDayTemperatureAverage = new List<double>();
                 HourlyDayHumidityAverage = new List<double>();
                 DailyWeekTemperatureAverage = new List<double>();
                 DailyWeekHumidityAverage = new List<double>();

                 // Fetch Data
                 try
                 {
                     CurrentTemperature = DataAccess.GetData(AccesableData.CurrentTemperature)[0];
                     DayTemperature = DataAccess.GetData(AccesableData.DayTemperature);
                     WeekTemperature = DataAccess.GetData(AccesableData.WeekTemperature);

                     CurrentHumidity = DataAccess.GetData(AccesableData.CurrentHumidity)[0];
                     DayHumidity = DataAccess.GetData(AccesableData.DayHumidity);
                     WeekHumidity = DataAccess.GetData(AccesableData.WeekHumidity);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show($"Error fetching data: {ex.Message}");
                     return;
                 }

                 // Calculate Averages
                 try
                 {
                     for (int i = 0; i < 24; i++)
                     {
                         if (DayTemperature.Count >= (i + 1) * DatapointsPerHour)
                         {
                             HourlyDayTemperatureAverage.Add(DayTemperature.GetRange(i * DatapointsPerHour, DatapointsPerHour).Average());
                             HourlyDayHumidityAverage.Add(DayHumidity.GetRange(i * DatapointsPerHour, DatapointsPerHour).Average());
                         }
                     }

                     for (int i = 0; i < 7; i++)
                     {
                         if (WeekTemperature.Count >= (i + 1) * 24 * DatapointsPerHour)
                         {
                             DailyWeekTemperatureAverage.Add(WeekTemperature.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
                             DailyWeekHumidityAverage.Add(WeekHumidity.GetRange(i * 24 * DatapointsPerHour, 24 * DatapointsPerHour).Average());
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show($"Error calculating averages: {ex.Message}");
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show($"An unexpected error occurred: {ex.Message}");
             }
         }
     }*/
        