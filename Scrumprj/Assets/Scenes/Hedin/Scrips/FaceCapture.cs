using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class FaceCapture : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    // Use this for initialization
    void Start()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (!result.success)
        {
            Debug.LogError("Failed to capture photo.");
            return;
        }

        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        targetTexture = ApplyZoom(targetTexture, 1.5f);

        // Apply oval mask to the captured texture
        ApplyOvalMask(targetTexture);

        // Create a gameobject that we can apply our texture to
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>();

        // Use a transparent material to support alpha
        Material transparentMaterial = new Material(Shader.Find("Unlit/Transparent"));
        quadRenderer.material = transparentMaterial;

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate our camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }


    void ApplyOvalMask(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Color[] pixels = texture.GetPixels();

        float centerX = width / 2f;
        float centerY = height / 2f;
        float radiusX = width / 3f;
        float radiusY = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedX = (x - centerX) / radiusX;
                float normalizedY = (y - centerY) / radiusY;

                if ((normalizedX * normalizedX + normalizedY * normalizedY) > 1f)
                {
                    int index = y * width + x;
                    pixels[index] = new Color(0, 0, 0, 0);
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
    Texture2D ApplyZoom(Texture2D original, float zoomFactor)
    {
        int width = original.width;
        int height = original.height;

        int cropWidth = (int)(width / zoomFactor);
        int cropHeight = (int)(height / zoomFactor);

        int startX = (width - cropWidth) / 2;
        int startY = (height - cropHeight) / 2;

        Color[] croppedPixels = original.GetPixels(startX, startY, cropWidth, cropHeight);
        Texture2D zoomedTexture = new Texture2D(cropWidth, cropHeight);
        zoomedTexture.SetPixels(croppedPixels);
        zoomedTexture.Apply();

        return zoomedTexture;
    }


    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}