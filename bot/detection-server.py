import socket
import cv2
import numpy as np

cascadePath = 'classifier/cascade.xml'

cascade = cv2.CascadeClassifier(cascadePath)

def findTarget(frame):
    raw = cascade.detectMultiScale(
        frame,
        scaleFactor=1.1,
        minNeighbors=5,
        minSize=(30, 30),
    )

    if(len(raw) == 0):
        return False

    x1, y1 = (min(row[column] for row in raw) for column in range(2))
    x2, y2 = (max(row[column] + row[column + 2] for row in raw) for column in range(2))
    return (str(x1), str(y1), str(x2), str(y2))

soc = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
soc.connect(('10.0.0.1', 6331))

while True:
    raw = np.frombuffer(soc.recv(50000), np.uint8)
    print(len(raw))
    img = cv2.imdecode(raw, cv2.IMREAD_COLOR)
    #cv2.imshow('img', img)

    target = findTarget(img)
    if target != False:
        soc.send(' '.join(target).encode('utf-8'))
    else:
        soc.send('False'.encode('utf-8'))

