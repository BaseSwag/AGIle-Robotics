
function createChart(context, data) {
    const options = {
        scales:
            {
                xAxes: [{
                    display: false
                }]
            }
    };
    return new Chart(context, {
        type: 'line',
        options,
        data,
    })
}

function createLabels(count) {
    const labels = [];
    for (let i = count -1; i > 0; i--) {
        if ((count - 1 - i) % 5 == 0) {
            labels.push(`-${i}s`);
        } else {
            labels.push('');
        }
    }
    labels.push('now')
    return labels;
}

function createDataSet(label, color, count) {
    const data = new Array(count);
    for (let i = 0; i < count; i++) {
        data[i] = 0;
    }
    return {
        label,
        data,
        backgroundColor: color,
        borderColor: color,
        fill: false,
    }
}

const labels = createLabels(30);
const config = {
    labels,
    datasets: [
        createDataSet('X', 'rgb(255, 99, 132)', 30),
        createDataSet('Y', 'rgb(54, 162, 235)', 30),
    ]
}
const sensorConfig = {
    labels,
    datasets: [
        {
            steppedLine: true,
            ...createDataSet('Front', 'rgb(75, 192, 192)', 30),
        },
        {
            steppedLine: 'before',
            ...createDataSet('Back', 'rgb(255, 159, 64)', 30),
        }
    ]
}
const timeConfig = {
    labels,
    datasets: [
        createDataSet('frameTime', 'rgb(255, 205, 86)', 30),
        createDataSet('loopTime', 'rgb(153, 102, 255)', 30),
    ]
}

const elSteering = document.getElementById('chart-steering');
const elSensors = document.getElementById('chart-sensors');
const elTime = document.getElementById('chart-time');
const chart = createChart(elSteering, config);
const sensorChart = createChart(elSensors, sensorConfig);
const timeChart = createChart(elTime, timeConfig);

function pushValues(chart, values) {
    for (let i=0; i < values.length; i++) {
        const dataset = chart.data.datasets[i];
        if (dataset !== undefined) {
            pushValue(dataset.data, values[i])
        }
    }
}

function pushValue(data, value) {
    data.shift();
    data.push(value)
}

function updateChartData(status) {
    pushValues(chart, [status.lastX, status.lastY]);
    pushValues(sensorChart, [status.front, status.back]);
    pushValues(timeChart, [status.frameTime, status.loopTime]);
}