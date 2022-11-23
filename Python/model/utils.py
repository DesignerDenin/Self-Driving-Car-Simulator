import cv2

(IMG_WIDTH, IMG_HEIGHT) = (80, 80)

def preprocess_image (image):
    image = cv2.cvtColor (image, cv2.COLOR_BGR2RGB)
    image = cv2.resize (image, (IMG_WIDTH, IMG_HEIGHT))
    image = cv2.convertScaleAbs(image, alpha=2, beta=40)
    image = image[20:76, :, :]
    image = cv2.cvtColor (image, cv2.COLOR_RGB2YUV)
    # print (image.shape)
    return image