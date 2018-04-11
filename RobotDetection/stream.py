from flask import Flask, Response, render_template
from webcam import WebCam

app = Flask(__name__)

@app.route('/')
def index():
    return render_template('index.html')

def stream(camera):
    while True:
        frame = camera.get_frame()
        yield (b'--frame\r\n' b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n\r\n')

@app.route('/stream')
def stream():
    return Response(stream(WebCam()), mimetype='multipart/x-mixed-replace; boundary=frame')

if __name__ == '__main__':
    app.run(host='0.0.0.0', debug=True)