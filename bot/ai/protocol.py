import json

from twisted.internet.protocol import Protocol

from motor import Control

class AIServerProtocol(Protocol):

    def connectionMade(self):
        self.factory.register(self.transport)

    def connectionLost(self, reason):
        Protocol.connectionLost(self, reason);
        self.factory.unregister(self.transport)

    def dataReceived(self, message):
        x, y = message.split(' ')[0:2]
        self.factory.control('ai', float(x), float(y))
        
