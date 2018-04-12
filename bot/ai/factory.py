import json

from twisted.internet.protocol import Factory

class AIServerFactory(Factory):
    
    def __init__(self):
        self.clients = []

    def register(self, client):
        print('register', client)
        if client not in self.clients:
            print("registered client {}".format(client))
            self.clients.append(client)

    def unregister(self, client):
        if client in self.clients:
            print("unregistered client {}".format(client))
            self.clients.remove(client)

    def broadcast(self, message):
        print("broadcast", message);
        for c in self.clients:
            c.write(message);
