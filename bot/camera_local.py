import cv2
import atexit

cascadePath = 'cascade_both.xml'

capture = cv2.VideoCapture(0)
cascade = cv2.CascadeClassifier(cascadePath)

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
    return (x1, y1, x2, y2)


def visualizeTarget(frame, target):
    if target != False:
        x1, y1, x2, y2 = target
        cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
