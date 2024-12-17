// Initial Data
const weatherData = {
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
};

let chart;

export function initializeChart() {
    const ctx = document.getElementById('myChart').getContext('2d');
    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: Array(weatherData.insideTemperature.day.length).fill().map((_, i) => `20:30`),
            datasets: [{
                label: 'Inside Temperature',
                data: weatherData.insideTemperature.day,
                borderWidth: 1
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
    chart.data.labels = Array(weatherData[type][range].length).fill().map((_, i) => `20:30`);
    chart.data.datasets[0].data = weatherData[type][range];
    chart.update();
}

