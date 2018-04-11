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
        type, data = message['type'], message['data']
        if type == 'steer':
            x, y = data['x'], data['y']
            print(x, y)


    def connectionLost(self, reason):
        WebSocketServerProtocol.connectionLost(self, reason)
        self.factory.unregister(self)
