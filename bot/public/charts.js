
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

const elSteering = document.getElementById('chart-steering');
const chart = createChart(elSteering, config);

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