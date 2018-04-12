
function createChart(context, data) {
    const options = {};

    return new Chart(context, {
        type: 'line',
        options,
        data,
    })
}

function createLabels(count) {
    const labels = [];
    for (let i = 0; i < count; i++) {
        labels.push(`t-${i}`)
    }
    return labels;
}

function createDataSet(label, count) {
    const data = new Array(count);
    for (let i = 0; i < count; i++) {
        data[i] = Math.random() * 10;
    }
    return {
        label,
        data
    }
}

const labels = createLabels(15);
const config = {
    labels,
    datasets: [
        createDataSet('X', 15),
        createDataSet('Y', 15),
    ]
}
const sensorConfig = {
    labels,
    datasets: [
        createDataSet('Front', 15),
        createDataSet('Back', 15),
    ]
}
const timeConfig = {
    labels,
    datasets: [
        createDataSet('frameTime', 15),
        createDataSet('loopTime', 15),
    ]
}

Chart.defaults.global.legend.position = 'right';
Chart.defaults.global.legend.display = false;
Chart.defaults.global.responsive = false;

const elSteering = document.getElementById('chart-steering');
const elSensors = document.getElementById('chart-sensors');
const elTime = document.getElementById('chart-time');
const chart = createChart(elSteering, config);
const sensorChart = createChart(elSensors, config);
const timeChart = createChart(elTime, config);

function pushValues(chart, values) {
    for (let i=0; i < values.length; i++) {
        const dataset = chart.data.datasets[i];
        if (dataset !== undefined) {
            pushValue(dataset.data, values[i])
        }
    }
    chart.update();
}

function pushValue(data, value) {
    data.shift();
    data.push(value)
}