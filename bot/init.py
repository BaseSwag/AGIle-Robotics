import sys, time, json, traceback
from base64 import b64encode
from cv2 import imencode

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

factory = DashboardServerFactory(u"ws://127.0.0.1:9000")
factory.protocol = DashboardServerProtocol
listenWS(factory)

lastX = 0
lastY = 0
def control(x, y):
    global lastX, lastY
    Control.joystick(x, y)
    lastX, lastY = x, y

aiFactory = AIServerFactory()
aiFactory.protocol = AIServerProtocol
aiFactory.control = control

reactor.listenTCP(8337, aiFactory)
reactor.listenTCP(8000, site)

def loop():
    try:
        frame, target = getNextFrame()
        success, buffer = imencode('.jpg', frame)
        msg = b64encode(buffer)
        factory.broadcast('image', msg)
    except:
        print(traceback.format_exc())
        pass

main = task.LoopingCall(loop)
main.start(1)

reactor.run()
