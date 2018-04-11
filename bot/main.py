import time

def sendImage(image):
    success, buffer = imencode('.jpg', frame)
    if success:
        msg = b64encode(buffer)
        factory.broadcast('image', msg)

def activateStrategy(status):
    info = [
        str(status['target'])
        '0', '0', '0',
        str(status['front']),
        str(status['back'])
    ]
    msg = ' '.join(info)
    aiFactory.broadcast(msg)

def sendStatus(status):
    factory.broadcast('status', status)

def getStatus(target):
    global lastX, lastY
    target = getTarget(target)
    return {
        'target': target, 
        'lastX': lastX, 
        'lastY': lastY, 
        'front': Sensor.getFront(), 
        'back': Sensor.getBack()
    }

def getTarget(target):
    if target == False:
        return -1
    x1, y1, x2, y2 = target
    return (x1 + x2) / 640.0

lastX, lastY = 0, 0
def control (x, y):
    global lastX, lastY
    Control.joystick(x, y)
    lastX, lastY = x, y

def loop():
    start = time.time()

    frame, target = getNextFrame()
    frameTime = time.time() - start
    sendImage(frame)

    status = getStatus(target)
    activateStrategy(status)
    status['frameTime'] = frameTime
    status['loopTime'] = time.time() - start
    sendStatus(status)
