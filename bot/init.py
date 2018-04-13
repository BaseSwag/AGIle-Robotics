import sys, time, json, traceback
from base64 import b64encode
from cv2 import imencode
import atexit
from subprocess import Popen
from threading import Timer

from enemy import enemy
from dashboard.factory import DashboardServerFactory
from dashboard.protocol import DashboardServerProtocol
from twisted.web.server import Site
from twisted.web.static import File

webdir = File("./public")
site = Site(webdir)

from ai.factory import AIServerFactory
from ai.protocol import AIServerProtocol

from camera import getNextFrame
from motor import Control
from sensor import Sensor

from twisted.internet import reactor, task
from twisted.python import log

from autobahn.twisted.websocket import listenWS

log.startLogging(sys.stdout)

mode = False
def changeMode(m):
    global mode

    print('changing mode to ', m)
    control(mode, 0, 0)
    if m == 'stop':
        mode = False
    else:
        mode = m

lastX, lastY = 0, 0
def control (m, x, y):
    global mode, lastX, lastY
    if m == mode:
        Control.joystick(x, y)
        lastX, lastY = x, y

factory = DashboardServerFactory(u"ws://127.0.0.1:9000")
factory.protocol = DashboardServerProtocol
factory.control = control
factory.changeMode = changeMode
listenWS(factory)

aiFactory = AIServerFactory()
aiFactory.protocol = AIServerProtocol
aiFactory.control = control

reactor.listenTCP(1337, aiFactory)
reactor.listenTCP(8000, site)

aiProcess = Popen('/usr/local/bin/dotnet "/home/pi/neural/RobotControl.dll" "/home/pi/network2.json"', shell=True)

def cleanup():
    aiFactory.broadcast('exit')
    timer = Timer(5, lambda: aiProcess.kill())

    try:
        timer.start()
        out = aiProcess.communicate()
    finally:
        timer.cancel()

    print('bye bye')


atexit.register(cleanup)

def sendImage(image):
    success, buffer = imencode('.jpg', image)
    if success:
        factory.broadcastImage(buffer.tobytes())


def sendStatus(status):
    factory.broadcastStatus(status)

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
    return ((x1 + x2) / 640.0)
    #return ((x1 + x2) / 640.0).item()

def activateAI(status):
    info = [
        str(status['target']),
        '0', '0', '0',
        str(status['front']),
        str(status['back'])
    ]
    msg = ' '.join(info)
    aiFactory.broadcast(msg)


def activateStrategy(status):
    print('activate', mode, status)
    if mode == 'ai':
        activateAI(status)
    elif mode == 'enemy':
        enemy(status['target'], 1, control)

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

main = task.LoopingCall(loop)
main.start(0)

reactor.run()

