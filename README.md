<h1>Neural Network Project</h1> 

<h2>Description</h2> 
This project demonstrates image recognition using a neural network in Unity, designed specifically for mobile devices. The objective was to create a simple model for recognizing triangles. 

<h3>Python</h3> 
The neural network model was first created in Python using the TensorFlow library. It was trained on 34 images, each with a resolution of 28x28 pixels—20 images of various triangle shapes and the rest of circles. After training, the model was saved in both `.keras` and `.onnx` formats, with the ONNX format enabling Unity compatibility through the Barracuda library. 

<h3>Unity</h3> 
The Unity project includes two main scripts: `DrawingScript.cs` and `Recognizer.cs`. `DrawingScript.cs` handles user input and drawing on the canvas. Once a user draws on the texture, the image is sent to `Recognizer.cs` to determine if the drawing resembles a triangle. Since the drawing texture is 1080x1080 pixels, it is resized to 28x28 pixels before recognition. Using Barracuda and the ONNX model, the script then processes the image and returns the final result. 

<h3>General Thoughts</h3> 
This is a simple project with limited recognition accuracy, primarily due to the basic neural network model and a small training dataset. Unlike the commonly used MNIST dataset, which includes 60,000 samples, this model was trained on only 34 images. Future improvements could involve expanding the dataset using a Python script to automate image processing (scaling, adding noise, etc.). 

<h2>Languages and Tools Used</h2>  

- <b>Unity</b>  

- <b>Visual Studio</b>

- <b>C#</b>  

- <b>Python</b>  

- <b>TensorFlow</b>

- <b>Barracuda</b>

- <b>Neural Network</b>
