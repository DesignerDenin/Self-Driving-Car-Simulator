import zmq, cv2
import numpy as np

from keras.models import load_model
from utils import preprocess_image

model = load_model('model.h5')

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

(CROP_HEIGHT, CROP_WIDTH) = (80, 56)

image = [[0, 0, 0]]
count = 0
res = 0

print("Waiting for image data...\n")

while True:
    bytes_received = socket.recv()
    path_string = bytes_received.decode ("utf-8")
    paths = path_string.split("|")

    for i in range (3):
        print(paths[i])
        img = cv2.imread (paths[i])
        image[0][i] = preprocess_image (img)

    image = np.array (image)
    for i in range (3):
        image[0][i] = np.array(image[0][i]).reshape (-1, CROP_WIDTH, CROP_HEIGHT, 3)
    
    image = image /255.0
    res = model.predict(image)[0][0]
    print(res)

    msg = bytes(str(res), encoding= 'utf-8')
    socket.send(msg)
        