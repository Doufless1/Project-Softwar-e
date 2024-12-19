// Initial Data
const weatherData = {
    Enschede: {
        insideTemperature: {
            day: [22, 23, 21, 20, 22, 24, 23],
            week: [22, 21, 23, 24, 22, 20, 23],
            month: [22, 21, 23, 22, 23, 21, 20, 22, 24, 23, 21, 22, 20, 21, 23, 22, 20, 21, 23, 24, 22, 23, 21, 20, 22, 24, 23, 21, 20, 22]
        },
        outsideTemperature: {
            day: [15, 16, 14, 13, 15, 17, 16],
            week: [15, 14, 16, 17, 15, 13, 16],
            month: [15, 14, 16, 15, 16, 14, 13, 15, 17, 16, 14, 15, 13, 14, 16, 15, 13, 14, 16, 17, 15, 16, 14, 13, 15, 17, 16, 14, 13, 15]
        },
        humidity: {
            day: [60, 62, 58, 57, 61, 63, 62],
            week: [60, 58, 62, 63, 60, 57, 62],
            month: [60, 58, 62, 61, 62, 58, 57, 60, 63, 62, 58, 60, 57, 58, 62, 61, 57, 58, 62, 63, 60, 62, 58, 57, 61, 63, 62, 58, 57, 60]
        },
        luminocity: {
            day: [300, 320, 310, 290, 300, 330, 310],
            week: [300, 310, 320, 330, 300, 290, 310],
            month: [300, 310, 320, 300, 310, 320, 290, 300, 330, 310, 320, 300, 290, 310, 320, 300, 290, 310, 320, 330, 300, 310, 320, 290, 300, 330, 310, 320, 290, 300]
        },
        airPressure: {
            day: [1012, 1013, 1011, 1010, 1012, 1014, 1013],
            week: [1012, 1011, 1013, 1014, 1012, 1010, 1013],
            month: [1012, 1011, 1013, 1012, 1013, 1011, 1010, 1012, 1014, 1013, 1011, 1012, 1010, 1011, 1013, 1012, 1010, 1011, 1013, 1014, 1012, 1013, 1011, 1010, 1012, 1014, 1013, 1011, 1010, 1012]
        }
    },
    Deventer: {
        insideTemperature: {
            day: [20, 21, 19, 18, 20, 22, 21],
            week: [20, 19, 21, 22, 20, 18, 21],
            month: [20, 19, 21, 20, 21, 19, 18, 20, 22, 21, 19, 20, 18, 19, 21, 20, 18, 19, 21, 22, 20, 21, 19, 18, 20, 22, 21, 19, 18, 20]
        },
        outsideTemperature: {
            day: [14, 15, 13, 12, 14, 16, 15],
            week: [14, 13, 15, 16, 14, 12, 15],
            month: [14, 13, 15, 14, 15, 13, 12, 14, 16, 15, 13, 14, 12, 13, 15, 14, 12, 13, 15, 16, 14, 15, 13, 12, 14, 16, 15, 13, 12, 14]
        },
        humidity: {
            day: [65, 67, 63, 62, 66, 68, 67],
            week: [65, 63, 67, 68, 65, 62, 67],
            month: [65, 63, 67, 66, 67, 63, 62, 65, 68, 67, 63, 65, 62, 63, 67, 66, 62, 63, 67, 68, 65, 67, 63, 62, 66, 68, 67, 63, 62, 65]
        },
        luminocity: {
            day: [280, 300, 290, 270, 280, 310, 290],
            week: [280, 290, 300, 310, 280, 270, 290],
            month: [280, 290, 300, 280, 290, 300, 270, 280, 310, 290, 300, 280, 270, 290, 300, 280, 270, 290, 300, 310, 280, 290, 300, 270, 280, 310, 290, 300, 270, 280]
        },
        airPressure: {
            day: [1015, 1016, 1014, 1013, 1015, 1017, 1016],
            week: [1015, 1014, 1016, 1017, 1015, 1013, 1016],
            month: [1015, 1014, 1016, 1015, 1016, 1014, 1013, 1015, 1017, 1016, 1014, 1015, 1013, 1014, 1016, 1015, 1013, 1014, 1016, 1017, 1015, 1016, 1014, 1013, 1015, 1017, 1016, 1014, 1013, 1015]
        }
    },
    Appeldorn: {
        insideTemperature: {
            day: [45, 20, 22, 41, 23, 15, 4],
            week: [23, 22, 24, 25, 23, 21, 24],
            month: [23, 22, 24, 23, 24, 22, 21, 23, 25, 24, 22, 23, 21, 22, 24, 23, 21, 22, 24, 25, 23, 24, 22, 21, 23, 25, 24, 22, 21, 23]
        },
        outsideTemperature: {
            day: [16, 17, 15, 14, 16, 18, 17],
            week: [16, 15, 17, 18, 16, 14, 17],
            month: [16, 15, 17, 16, 17, 15, 14, 16, 18, 17, 15, 16, 14, 15, 17, 16, 14, 15, 17, 18, 16, 17, 15, 14, 16, 18, 17, 15, 14, 16]
        },
        humidity: {
            day: [55, 57, 53, 52, 56, 58, 57],
            week: [55, 53, 57, 58, 55, 52, 57],
            month: [55, 53, 57, 56, 57, 53, 52, 55, 58, 57, 53, 55, 52, 53, 57, 56, 52, 53, 57, 58, 55, 57, 53, 52, 56, 58, 57, 53, 52, 55]
        },
        luminocity: {
            day: [320, 340, 330, 310, 320, 350, 330],
            week: [320, 330, 340, 350, 320, 310, 330],
            month: [320, 330, 340, 320, 330, 340, 310, 320, 350, 330, 340, 320, 310, 330, 340, 320, 310, 330, 340, 350, 320, 330, 340, 310, 320, 350, 330, 340, 310, 320]
        },
        airPressure: {
            day: [1010, 1011, 1009, 1008, 1010, 1012, 1011],
            week: [1010, 1009, 1011, 1012, 1010, 1008, 1011],
            month: [1010, 1009, 1011, 1010, 1011, 1009, 1008, 1010, 1012, 1011, 1009, 1010, 1008, 1009, 1011, 1010, 1008, 1009, 1011, 1012, 1010, 1011, 1009, 1008, 1010, 1012, 1011, 1009, 1008, 1010]
        }
    }
};
const cityColors = { Enschede: 'red', Deventer: 'blue', Appeldorn: 'green' };
let activeCities = { Enschede: true, Appeldorn: false, Deventer: false};
let timeRange = { day: 360, week: 4320, month: 30240}
let chart;
function getDate(range){
    const currentTime = new Date(); // Get the current time
    const timeRangeMinutes = timeRange[range]; // Total time range of the chart in minutes
    const numDataPoints = weatherData.Enschede.insideTemperature.day.length;
    const interval = timeRangeMinutes / numDataPoints; // Time interval between points in minutes

    // Generate labels based on the current time and time interval
    const timeLabels = Array(numDataPoints).fill().map((_, i) => {
        const labelTime = new Date(currentTime.getTime() - (timeRangeMinutes - i * interval) * 60 * 1000);
        const date = labelTime.toLocaleDateString('en-GB'); // Format as DD/MM/YYYY (change locale as needed)
        const time = labelTime.toTimeString().slice(0, 5); // Format as HH:mm
        return `${date} ${time}`; // Combine date and time
    });
    return timeLabels
}
export function initializeChart() {
    const ctx = document.getElementById('myChart').getContext('2d');
    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: getDate('day'),
            datasets: [{
                label: 'Enschede',
                data: weatherData.Enschede.insideTemperature.day,
                borderWidth: 1,
                borderColor: 'red'
            }]
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            scales: {
                y: {
                    beginAtZero: false
                }
            }
        }
    });
}

export function updateChartData(type, range) {
    chart.data.datasets = [];
      for (const city in activeCities) {
        if (activeCities[city]) {
          chart.data.datasets.push({
            label: city,
            data: weatherData[city][type][range],
            borderWidth: 1,
            borderColor: cityColors[city],
          });
          chart.data.labels = getDate(range);
        }
      }
      chart.update();
}

export function toggleCityData(city, type, range) {
    if (activeCities[city]) {
      chart.data.datasets = chart.data.datasets.filter(dataset => dataset.label !== city);
      activeCities[city] = false;
    } else {
        chart.data.datasets.push({
            label: city,
            data: weatherData[city][type][range],
            borderWidth: 1,
            borderColor: cityColors[city],
          });
        chart.update();
        activeCities[city] = true;
    }
    chart.update();
  }
