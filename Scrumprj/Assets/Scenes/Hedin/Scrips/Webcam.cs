using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    [SerializeField] private RawImage img = default;
    private WebCamTexture webCam;

    private void Start()
    {
        webCam = new WebCamTexture();
        if (!webCam.isPlaying)
        {
            webCam.Play();
            //ApplyZoom(img, 2f);
            img.texture = webCam;
        }
    }
  /*  Texture2D ApplyZoom(RawImage original, float zoomFactor)
    {
        int width = original.texture.width;
        int height = original.texture.height;

        int cropWidth = (int)(width / zoomFactor);
        int cropHeight = (int)(height / zoomFactor);

        int startX = (width - cropWidth) / 2;
        int startY = (height - cropHeight) / 2;

        Color[] croppedPixels = original.GetPixels(startX, startY, cropWidth, cropHeight);
        Texture2D zoomedTexture = new Texture2D(cropWidth, cropHeight);
        zoomedTexture.SetPixels(croppedPixels);
        zoomedTexture.Apply();

        return zoomedTexture;
    }*/
}
