
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

const labels = createLabels(30);
const config = {
    labels,
    datasets: [
        createDataSet('X', 30),
        createDataSet('Y', 30),
    ]
}
const sensorConfig = {
    labels,
    datasets: [
        createDataSet('Front', 30),
        createDataSet('Back', 30),
    ]
}
const timeConfig = {
    labels,
    datasets: [
        createDataSet('frameTime', 30),
        createDataSet('loopTime', 30),
    ]
}

Chart.defaults.global.legend.position = 'right';
Chart.defaults.global.legend.display = false;
Chart.defaults.global.responsive = false;

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
    chart.update();
}

function pushValue(data, value) {
    data.shift();
    data.push(value)
}