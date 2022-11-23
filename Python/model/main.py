import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import os, cv2

from utils import preprocess_image

# load data from csv
DATA_DIR = "D://Projects//gamedev//unity//self-driving-car-simulator//data//2022-09-14 11_43_42//csv//record_log.csv"
data_df = pd.read_csv (DATA_DIR, names=['center', 'left', 'right', 'steering', 'speed'])
X_paths = data_df[['center', 'left', 'right']].values
y = data_df['steering'].values

# print (len(X_paths))

# preprocess image
(IMG_WIDTH, IMG_HEIGHT) = (80, 80)

X = [[0, 0, 0]] * len (X_paths)

for i in range (len (X_paths)):
    for j in range (len (X_paths[i])):
        image = cv2.imread (X_paths[i][j])
        image = preprocess_image (image)
        X[i][j] = image

# convert to image array 
X = np.array(X, dtype=object)
y = np.array(y)

# reshape image array
(CROP_WIDTH, CROP_HEIGHT) =  len(X[0][0][0]), len(X[0][0])
for i in range(len(X_paths)):
    for j in range(len(X_paths[i])):
        X[i][j] = np.array(X[i][j]).reshape(-1, CROP_HEIGHT, CROP_WIDTH, 3)


# build model
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation, Flatten, Conv2D, Lambda
from keras.callbacks import ModelCheckpoint
from keras.optimizers import Adam

X = X/255.0

model = Sequential()
model.add(Lambda(lambda x: x/127.5-1.0, input_shape=X.shape[1:]))
model.add(Conv2D(24, (5, 5), strides=(2, 2), activation='elu'))

model.add(Conv2D(36, (5, 5), strides=(2, 2)))
model.add (Activation ('elu'))
model.add(Conv2D(48, (3, 3), strides=(1, 1)))
model.add (Activation ('elu'))

model.add(Dropout(0.1))

model.add(Flatten())

model.add(Dense(1))
model.add (Activation ('sigmoid'))

model.summary() 

# compile model
checkpoint = ModelCheckpoint('model.h5', monitor='val_loss', verbose=1, mode='auto', save_best_only=True)
model.compile(loss="mean_squared_error", optimizer=Adam(learning_rate=1.0e-4), metrics=['accuracy'])

# Training Model
X = np.asarray(X).astype('float32')
model.fit(X, y, batch_size=32, epochs=20, validation_split=0.3, callbacks=[checkpoint])

# calculate accuracy
accuracy = model.evaluate(X, y, verbose=0)
print (accuracy)