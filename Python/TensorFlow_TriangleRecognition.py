# NOTEs:
# Training model for only one drawing(e.g. triangles) doesnt make NN able to recognize if something IS NOT traingle. 
# NN will always try to recognize something so best way is to give it set of different shapes/images to work on

from PIL import Image
import numpy as np
import tensorflow as tf
import os
import tf2onnx

def create_model():
    image_folder = "Triangles"
    image_files = [f for f in os.listdir(image_folder) if f.endswith('.jpg')]

    # read images
    images = []
    for file in image_files:
        img = Image.open(os.path.join(image_folder, file)).convert('L')  # to grayscale
        img = img.resize((28, 28))

        # normalize
        img_array = np.array(img) / 255.0
        images.append(img_array)

    # 4D array for NN
    images = np.array(images).reshape(-1, 28, 28, 1) 

    # labels for each of drawings
    labels = np.array([1 if 'triangle' in file_name else 0 for file_name in image_files])
    # for file, label in zip(image_files, labels):
    #     print(f"Plik: {file}, Etykieta: {label}")

    # model of NN
    model = tf.keras.models.Sequential([
    tf.keras.layers.Conv2D(32, (3, 3), activation='relu', input_shape=(28, 28, 1)),
    tf.keras.layers.MaxPooling2D((2, 2)),
    tf.keras.layers.Conv2D(64, (3, 3), activation='relu'), 
    tf.keras.layers.MaxPooling2D((2, 2)),
    tf.keras.layers.Flatten(),
    tf.keras.layers.Dense(64, activation='relu'),
    tf.keras.layers.Dense(2, activation='softmax')
    ])

    # compile
    model.compile(optimizer='adam',
              loss='sparse_categorical_crossentropy',  
              metrics=['accuracy'])


    # train model for 10 epochs, 20% of images for validation 80% for training
    history = model.fit(images, labels, epochs=10, validation_split=0.2)

    #print(history)
    return model

    

def recognize(input_image):
    image = Image.open(input_image).convert('L').resize((28, 28))
    image_array = np.array(image) / 255.0
    image_array = image_array.reshape((1, 28, 28, 1))

    prediction = model.predict(image_array)
    # print("Prediction:", prediction)

    triangle_confidence = prediction[0][1]
    non_triangle_confidence = prediction[0][0]

    # print(prediction[0][0])
    # print(prediction[0][1])

    # Decyzja o klasie na podstawie większego prawdopodobieństwa
    if triangle_confidence > non_triangle_confidence:
        print(f"Traingle! (p = {triangle_confidence:.4f})")
    else:
        print(f"Not Triangle. (p = {non_triangle_confidence:.4f})")


### MAIN
## CREATE new model
model = create_model()
input_signature = [tf.TensorSpec(model.inputs[0].shape, model.inputs[0].dtype, name='digit')]
## Display the model's architecture
# model.summary()

## SAVE new model
# For python
model.save('saved_model/my_model.keras')
# For barracuda 
model_proto, _ = tf2onnx.convert.from_keras(model, input_signature, opset=13)
onnx_model_path = "saved_model/my_model.onnx"
with open(onnx_model_path, "wb") as f: # save as onnx
    f.write(model_proto.SerializeToString())

## LOAD model and TEST
model = tf.keras.models.load_model('saved_model/my_model.keras')
recognize("kolo.jpg")