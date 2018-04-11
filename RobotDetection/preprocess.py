import glob
import os
import io
import tensorflow as tf

from PIL import Image
from shutil import copyfile

inputPath = 'C:\\Users\\da-st\\Desktop\\Stack\\'
outputPath = 'C:\\Users\\da-st\\Desktop\\Training\\images\\'

os.makedirs(outputPath + 'tests\\', exist_ok=True)
os.makedirs(outputPath + 'training\\', exist_ok=True)

testQuote = 0.1

tests = 0
total = 0

testWriter = tf.python_io.TFRecordWriter(outputPath + 'test.record')
trainingWriter = tf.python_io.TFRecordWriter(outputPath + 'training.record')

def int64_feature(value):
    return tf.train.Feature(int64_list=tf.train.Int64List(value=[value]))


def int64_list_feature(value):
    return tf.train.Feature(int64_list=tf.train.Int64List(value=value))


def bytes_feature(value):
    return tf.train.Feature(bytes_list=tf.train.BytesList(value=[value]))


def bytes_list_feature(value):
    return tf.train.Feature(bytes_list=tf.train.BytesList(value=value))


def float_list_feature(value):
    return tf.train.Feature(float_list=tf.train.FloatList(value=value))


def create_record(path, data):
    with tf.gfile.GFile(path, 'rb') as fid:
        encoded_jpg = fid.read()

    encoded_jpg_io = io.BytesIO(encoded_jpg)
    image = Image.open(encoded_jpg_io)
    width, height = image.size
    width, height = 320, 240

    cls, *pos = data
    x, y, b, h = (float(i) for i in pos)

    filename = path.encode('utf8')
    image_format = b'jpg'
    xmins = [x]
    xmaxs = [x + b]
    ymins = [y]
    ymaxs = [y + h]
    classes_text = [cls.encode('utf8')]
    classes = [1]

    return tf.train.Example(features=tf.train.Features(feature={
        'image/height': int64_feature(height),
        'image/width': int64_feature(width),
        'image/filename': bytes_feature(filename),
        'image/source_id': bytes_feature(filename),
        'image/encoded': bytes_feature(encoded_jpg),
        'image/format': bytes_feature(image_format),
        'image/object/bbox/xmin': float_list_feature(xmins),
        'image/object/bbox/xmax': float_list_feature(xmaxs),
        'image/object/bbox/ymin': float_list_feature(ymins),
        'image/object/bbox/ymax': float_list_feature(ymaxs),
        'image/object/class/text': bytes_list_feature(classes_text),
        'image/object/class/label': int64_list_feature(classes),
    }))


for filename in glob.glob(inputPath + '*.txt'):
    basePath, ext = os.path.splitext(filename)
    imgPath = basePath + '.jpg'
    dir, imgName = os.path.split(imgPath)

    if not os.path.exists(imgPath):
        continue

    total += 1

    dir = 'training\\'
    writer = trainingWriter

    if (tests / total) < testQuote:
        tests += 1
        writer = testWriter
        dir = 'tests\\'

    targetPath = outputPath + dir + imgName

    copyfile(imgPath, targetPath)

    with open(filename) as file:
        data = file.readline().replace(',', '.').split()
        record = create_record(targetPath, data)
        writer.write(record.SerializeToString())

trainingWriter.close()
testWriter.close()
