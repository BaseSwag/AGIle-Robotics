lastX = 0
lastY = 0

lastEnemyX = -1
lastEnemyWidth = -1

def enemy(pos_x, width, callback):
    global lastEnemyX, lastEnemyWidth, lastX, lastY
    newX, newY = 0, 0
    if pos_x == -1:
        if lastEnemyX <= 0.5:
            newX = -1
        else:
            newX = 1
        newX *= 0.6
        newY = 0
    else:
        newX = angleToX(pos_x)
        if abs(newX) < 0.1:
            newX = 0
        newX *= 0.75

    if newX >= 0.2:
        newX = max(newX, 0.4)
    elif newX <= -0.2:
        newX = min(newX, -0.4)
    else:
        newX = 0

    callback('enemy', newX, newY)
    lastX = newX
    lastY = newY
    if pos_x >= 0:
        lastEnemyX = pos_x
        lastEnemyWidth = width

def angleToX(a):
    return a * 2 - 1
