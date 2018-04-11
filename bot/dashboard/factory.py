import json

from autobahn.twisted.websocket import WebSocketServerFactory

class DashboardServerFactory(WebSocketServerFactory):
    
    def __init__(self, url):
        WebSocketServerFactory.__init__(self, url)
        self.clients = []

    def register(self, client):
        if client not in self.clients:
            print("registered client {}".format(client.peer))
            self.clients.append(client)

    def unregister(self, client):
        if client in self.clients:
            print("unregistered client {}".format(client.peer))
            self.clients.remove(client)

    def broadcast(self, type, data):
        msg = json.dumps({'type': type, 'data': data})
        for c in self.clients:
            c.sendMessage(msg.encode('utf8'))