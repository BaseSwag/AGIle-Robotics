import cv2
import socket

capture = cv2.VideoCapture(0)

soc = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
soc.bind(('0.0.0.0', 7331))
soc.listen(1)

client, addr = soc.accept()
print('Connected from ', addr)

def getNextFrame():
    frame = readFrame()
    target = findTarget(frame)
    visualizeTarget(frame, target)
    return (frame, target)

def readFrame():
    success, frame = capture.read()
    if not success:
        raise RuntimeError('Unable to read next Frame!')
    return cv2.resize(frame, (320, 240))


def findTarget(frame):
    try:
        success, buff = cv2.imencode('.jpg', frame)
        client.send(buff.tobytes())
        data = client.recv(200)
        target = data.split(' ')
        if len(target) == 4:
            return (int(target[0]), int(target[1]), int(target[2]), int(target[3]))
        else:
            return False
    except:
        return False

def visualizeTarget(frame, target):
    if target != False:
        x1, y1, x2, y2 = target
        cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
