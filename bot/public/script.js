var sock = null;
var ellog = null;

function onMessage({ data }) {
    data = JSON.parse(data);
    switch (data.type) {
        case "image":
            display.src = `data:image/jpg;base64,${data.data}`
            break;
    }
}
function send(type, data) {
    if (sock)
        sock.send(JSON.stringify({ type, data }));
}
function steer(x, y) {
    console.log('steering', x, y);
    send('steer', { x, y });
}

window.onload = function () {
    var wsuri;
    ellog = document.getElementById('log');
    if (window.location.protocol === "file:") {
        wsuri = "ws://localhost:9000";
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
            log("Connected to " + wsuri);
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
});
joystick.on('move', (evt, data) => {
    const x = Math.cos(data.angle.radian) * data.distance / 50;
    const y = Math.sin(data.angle.radian) * data.distance / 50;
    steer(x, y);
});
joystick.on('end', () => steer(0, 0));