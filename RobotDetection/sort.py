import glob
import os
import io
from PIL import Image
from shutil import copyfile

inputPath = 'C:\\Users\\da-st\\Desktop\\TestingStack\\Output\\'
outputPath = 'C:\\Users\\da-st\\Desktop\\TestingStack\\Results2\\'
positivePath = outputPath + 'positive\\'
negativePath = outputPath + 'negative\\'

os.makedirs(positivePath, exist_ok=True)
os.makedirs(negativePath, exist_ok=True)

total = 0

def readInfo(fileName, imgSize):
    with open(fileName) as file:
        line = file.readline().replace(',', '.')
        data = line.split()[1:]
        raw = [float(x) for x in data]
        return [round(raw[i] * imgSize[i % 2]) for i in range(len(raw))]


def generateNegatives(image, posSize, imgSize):
    yolo = 0
    boxX, boxY, boxW, boxH = posSize
    width, height = imgSize
    startX = boxX % boxW
    endX = width - boxW + 1
    startY = boxY % boxH
    endY = height - boxH + 1
    for x in range(startX, endX, boxW):
        for y in range(startY, endY, boxH):
            if x == boxX and y == boxY:
                continue
            ngt = image.crop((x, y, x + boxW, y + boxH))
            ngt.save(negativePath + str(total) + '-' + str(yolo) + '.jpg')
            yolo += 1



def create_record(imgPath, infoPath):
    image = Image.open(imgPath)
    size = readInfo(infoPath, image.size)
    x, y, w, h = size

    if x < 0:
        image.save(negativePath + str(total) + '.jpg')
    else:
        cropped = image.crop((x, y, x+w, y+h))
        cropped.save(positivePath + str(total) + '.jpg')
        generateNegatives(image, size, image.size)


def process_file(filename):
    basePath, ext = os.path.splitext(filename)
    imgPath = basePath + '.jpg'
    dir, imgName = os.path.split(imgPath)

    if not os.path.exists(imgPath):
        return
    
    print(filename)
    create_record(imgPath, filename)


for filename in glob.glob(inputPath + '*.txt'):
    try:
        process_file(filename)
    except:
        print('Error!!!')
    total += 1
