import json

from autobahn.twisted.websocket import WebSocketServerProtocol

class DashboardServerProtocol(WebSocketServerProtocol):

    def onOpen(self):
        self.factory.register(self)

    def onMessage(self, payload, isBinary):
        if isBinary:
            print('igonoring binary data')
            return

        message = json.loads(payload.decode('utf8'))
        action, param = message['action'], message['param']
        if action == 'steer':
            x, y = param['x'], param['y']
            self.factory.control('manual', x, y)
        elif action == 'change-mode':
            self.factory.changeMode(param)



    def connectionLost(self, reason):
        WebSocketServerProtocol.connectionLost(self, reason)
        self.factory.unregister(self)
