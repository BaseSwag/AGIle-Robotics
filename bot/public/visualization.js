let vWidth;
let vHeight;
let vCenterX;
let vCenterY;
let vRobotHeight;
let vRobotWidth;

let vCanvas;

let vRobotData = {
    joyX: 0,
    joyY: 0,
    sensorF: 0,
    sensorB: 0,
}

function windowResized() {
    let robotContainer = document.querySelector('#robotContainer');
    vWidth = robotContainer.offsetWidth;
    vHeight = robotContainer.offsetHeight;
    let vCanvas = createCanvas(vWidth, vHeight);
    vCanvas.parent('robotContainer');
    vCenterX = vWidth / 2;
    vCenterY = vHeight / 2;


    vRobotHeight = vHeight * (4 / 5);
    if(vWidth < vRobotHeight * (3 / 4)) {
        vRobotWidth = vWidth * (4 / 5);
        vRobotHeight = vRobotWidth * (4 / 3);
    } else {
        vRobotWidth = vRobotHeight * (3 / 4);
    }
}

function setup() {
    windowResized();
    //noLoop();
    ellipseMode(CENTER);
    rectMode(CENTER);
    colorMode(RGB);
}

function draw() {
    //*
    frameRate(1);
    vRobotData = {
        joyX: random(-1, 1),
        joyY: random(-1, 1),
        sensorF: Math.round(random(0, 1)),
        sensorB: Math.round(random(0, 1)),
    }
    //*/
    clear();
    background('rgba(0,0,0,0)');
    drawCar();
    drawTyres();
    drawSensors();
    drawVector();
}

function drawRobot(data) {
    vRobotData = data;
    draw();
}

function drawCar() {
    push();
    fill(128);
    rect(vCenterX, vCenterY, vRobotWidth, vRobotHeight, 20);
    pop();
}

function drawVector() {
    let targetX = vCenterX + vRobotData.joyX * vRobotWidth / 2;
    let targetY = vCenterY - vRobotData.joyY * vRobotWidth / 2;
    push();
    stroke('red');
    strokeWeight(4);
    line(vCenterX, vCenterY, targetX, targetY);
    pop();
}

function drawTyres() {
    let tyreHeight = vRobotHeight / 4;
    let tyreWidth = vRobotWidth / 10;
    let middleSpacer = tyreHeight * (3/4);
    let sideSpacer = vRobotWidth / 20;
    
    let dif = joyToDif(vRobotData.joyX, vRobotData.joyY);

    drawTyre(vCenterX + vRobotWidth / 2 - tyreWidth / 2 - sideSpacer, vCenterY + middleSpacer / 2 + tyreHeight / 2, tyreWidth, tyreHeight, dif.r);
    drawTyre(vCenterX + vRobotWidth / 2 - tyreWidth / 2 - sideSpacer, vCenterY - middleSpacer / 2 - tyreHeight / 2, tyreWidth, tyreHeight, dif.r);
    drawTyre(vCenterX - vRobotWidth / 2 + tyreWidth / 2 + sideSpacer, vCenterY + middleSpacer / 2 + tyreHeight / 2, tyreWidth, tyreHeight, dif.l);
    drawTyre(vCenterX - vRobotWidth / 2 + tyreWidth / 2 + sideSpacer, vCenterY - middleSpacer / 2 - tyreHeight / 2, tyreWidth, tyreHeight, dif.l);
}

function joyToDif(x, y)
{
    x *= -1;
    let v = (1-Math.abs(x)) * y + y;
    let w = (1-Math.abs(y)) * x + x;

    let r = (v+w)/2;
    let l = (v-w)/2;
    return {r: r, l: l};
}

function drawTyre(x, y, width, height, speed) {
    let color = speed.map(-1, 1, 0, 100);
    push();
    //strokeWeight(4);
    colorMode(HSL);
    fill(color, 100, 50);
    rect(x, y, width, height, 20);
    pop();
}

function drawSensors() {
    let size = vRobotHeight / 5;
    let y = vCenterY - vRobotHeight / 2 + size;
    push();
    fillBySensor(vRobotData.sensorF);
    ellipse(vCenterX, y, size, size);
    y = vCenterY + vRobotHeight / 2 - size;
    fillBySensor(vRobotData.sensorB);
    ellipse(vCenterX, y, size, size);
    pop();
}

function fillBySensor(s) {
    if(s == 1) {
        fill('white');
        stroke('black')
    } else {
        fill('black');
        stroke('white')
    }
}
