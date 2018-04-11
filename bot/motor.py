import RPi.GPIO as GPIO

MOTOR_RIGHT = 13	
MOTOR_LEFT = 20	
RIGHT_1 = 19	
RIGHT_2 = 16	
LEFT_1 = 21	
LEFT_2 = 26	

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

for port in (MOTOR_RIGHT, MOTOR_LEFT, RIGHT_1, RIGHT_2, LEFT_1, LEFT_2):
    GPIO.setup(port,GPIO.OUT,initial=GPIO.LOW)

def initPWM(port):
    pwm = GPIO.PWM(port,1000)
    pwm.start(0)
    pwm.ChangeDutyCycle(99)
    return pwm


pwmLeft=initPWM(MOTOR_LEFT)
pwmRight=initPWM(MOTOR_RIGHT)

def setMotor(m, speed, direction):
    motor, port, m1, m2 = m
    
    GPIO.output(port,True)
    setMotorSpeed(motor, speed)

    GPIO.output(m1, direction)
    GPIO.output(m2, not direction)


def setMotorSpeed(motor, speed):
    motor.ChangeDutyCycle(abs(speed) * 100)


def toDifferential(y, x):
    x *= -1
    v = (1 - abs(x)) * y + y
    w = (1 - abs(y)) * x + x

    r = (v + w) / 2
    l = (v - w) / 2	

    if y < 0:
        return r, l
    else:
        return l, r


class Motors:
    def __init__(self, left, right):
        lPort, l1, l2 = left
        rPort, r1, r2 = right
        lMotor = initPWM(lPort)
        rMotor = initPWM(rPort)

        self.left = lMotor, lPort, l1, l2
        self.right = rMotor, rPort, r1, r2

    def differential(self, l, r):
        lDirection = (l >= 0)
        rDirection = (r >= 0)
        setMotor(self.left, l, lDirection)
        setMotor(self.right, r, rDirection)

    def joystick(self, x, y):
        l, r = toDifferential(y, x)
        self.differential(l, r)


Control = Motors((MOTOR_LEFT, LEFT_1, LEFT_2), (MOTOR_RIGHT, RIGHT_1, RIGHT_2))
