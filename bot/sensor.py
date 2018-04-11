import RPi.GPIO as GPIO

SENSOR_FRONT = 18
SENSOR_BACK = 22

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

for port in (SENSOR_FRONT, SENSOR_BACK):
    GPIO.setup(port,GPIO.IN,pull_up_down=GPIO.PUD_UP)


#GPIO.add_event_detect(SENSOR_FRONT, GPIO.RISING, callback=on_event, bouncetime=100)

def getValue(pin):
    return GPIO.input(pin)


class Sensors:
    def __init__(self, front, back):
        self.front = front
        self.back = back

    def getFront(self):
        return getValue(self.front)

    def getBack(self):
        return getValue(self.back)

Sensor = Sensors(SENSOR_FRONT, SENSOR_BACK)
