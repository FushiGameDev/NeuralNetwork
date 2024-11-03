using UnityEngine;
using Unity.Barracuda;
using TMPro;
using System.IO;

public class Recognizer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] NNModel triangleModel;
    IWorker worker;

    private Texture2D inputImage;
    public Texture2D InputImage //encapsulation
    {
        get => inputImage;
        set
        {
            inputImage = value;
            Recognize();
        }
    }

    void Start()
    {
        var model = ModelLoader.Load(triangleModel);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    public void Recognize() 
    {
        Debug.Log(inputImage);
        // convert image to tensor
        Tensor inputTensor = TransformInput(inputImage);

        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();

        float isTriangle = outputTensor[1];
        float isNotTriangle = outputTensor[0];
        Debug.Log("Triangle: " + isTriangle + " Not: " + isNotTriangle);
        if (isTriangle > isNotTriangle)
            resultText.text = "Triangle! (p: " + isTriangle + " )";
        else
            resultText.text = "Not triangle (p: " + isNotTriangle + " )";

        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    Tensor TransformInput(Texture2D image)
    {
        Texture2D resizedTexture = new Texture2D(28, 28, TextureFormat.RGB24, false);

        // scale to 28x28
        Color[] pixels = new Color[28 * 28];
        for (int y = 0; y < 28; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                // normalize the value of the new pixels...
                float u = x / 28.0f;
                float v = y / 28.0f;

                // ... and use this value to get the same normalized pixel from input image
                pixels[y * 28 + x] = image.GetPixelBilinear(u, v);
            }
        }

        resizedTexture.SetPixels(pixels);
        resizedTexture.Apply();

        // save
        string filePath = Path.Combine(Application.dataPath, "testTrioResized.jpg");
        byte[] bytes = resizedTexture.EncodeToJPG();
        File.WriteAllBytes(filePath, bytes);

        // to grayscale
        float[] floatValues = new float[28 * 28];
        Color[] resizedPixels = resizedTexture.GetPixels();
        for (int i = 0; i < resizedPixels.Length; i++)
            floatValues[i] = resizedPixels[i].grayscale;

        // for the tensor
        return new Tensor(1, 28, 28, 1, floatValues);
    }

}
