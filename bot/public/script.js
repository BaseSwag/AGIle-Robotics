var sock = null;
var ellog = null;

function onMessage({ data }) {
    if (data instanceof Blob) {
        const url = window.URL.createObjectURL(data);
        display.src = url;
        return;
    }
    const status = JSON.parse(data);
    updateChartData(status);
    drawRobot(status);
}
function send(action, param) {
    if (sock)
        sock.send(JSON.stringify({ action, param }));
}

function steer(x, y) {
    send('steer', { x, y });
}

function changeMode(mode) {
    send('change-mode', mode)
}

window.onload = function () {
    var wsuri;
    ellog = document.getElementById('log');
    if (window.location.protocol === "file:") {
        wsuri = "ws://10.0.0.1:9000";
    } else {
        wsuri = "ws://" + window.location.hostname + ":9000";
    }
    if ("WebSocket" in window) {
        sock = new WebSocket(wsuri);
    } else if ("MozWebSocket" in window) {
        sock = new MozWebSocket(wsuri);
    } else {
        log("Browser does not support WebSocket!");
    }
    if (sock) {
        sock.onopen = function () {
            console.log("Connected to " + wsuri);
        }
        sock.onclose = function (e) {
            log("Connection closed (wasClean = " + e.wasClean + ", code = " + e.code + ", reason = '" + e.reason + "')");
            sock = null;
        }
        display = document.getElementById('display');
        sock.onmessage = onMessage;
    }
};
function broadcast() {
    var msg = document.getElementById('message').value;
    if (sock) {
        sock.send(msg);
        log("Sent: " + msg);
    } else {
        log("Not connected.");
    }
};
function log(m) {
    ellog.innerHTML += m + '\n';
    ellog.scrollTop = ellog.scrollHeight;
};

const joystick = nipplejs.create({
    color: 'blue',
    mode: 'semi',
    zone: document.getElementById('nipple-zone'),
});
joystick.on('move', (evt, data) => {
    const x = Math.cos(data.angle.radian) * data.distance / 50;
    const y = Math.sin(data.angle.radian) * data.distance / 50;
    steer(x, y);
});
joystick.on('end', () => steer(0, 0));

Number.prototype.map = function (in_min, in_max, out_min, out_max) {
  return (this - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}
