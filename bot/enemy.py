lastX = 0
lastY = 0

lastEnemyX = -1
lastEnemyWidth = -1

def enemy(pos_x, width, callback):
    newX, newY = 0, 0
    if pos_x == -1:
        if lastEnemyX <= 0.5:
            newX = -1
        else:
            newX = 1
        newY = 0
    else:
        newX = angleToX(pos_x)
        if abs(newX) < 0.1:
            newX = 0

    callback(newX, newY)
    lastX = newX
    lastY = newY
    lastEnemyX = pos_x
    lastEnemyWidth = width

def angleToX(a):
    return a * 2 - 1