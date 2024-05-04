using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCamera : MonoBehaviour
{
    /*
    Simple screenshot script - attach this to any Camera 
    object and hit the specified hotkey to save a .PNG of
    what the Camera sees!

    Note: To use with VR, set the additional camera to render
    to Display 2! Best "minimap" results with an orthographic
    Camera. 

    Partially inspired by https://www.youtube.com/watch?v=lT-SRLKUe5k
    */ 

    public KeyCode hotkey = KeyCode.S;
    public int width;
    public int height;
    
    private Camera cam;
    private int screenshotCounter = 1;
    private string camLabel = "Screenshot";
    
    void Start() {

        cam = gameObject.GetComponent<Camera>();
        camLabel = cam.name;
        if (width == 0) width = Screen.width;
        if (height == 0) height = Screen.height;

    }

    void Update() {

        if (Input.GetKeyDown(hotkey)) {
            TakeScreenshot(width, height);
        }

    }

    private IEnumerator CaptureCoroutine() {

        yield return new WaitForEndOfFrame();

        // Retrieve camera texture
        RenderTexture tex = cam.targetTexture;
        Texture2D render = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect (0, 0, tex.width, tex.height);
        RenderTexture.active = tex;
        render.ReadPixels(rect, 0, 0);

        // Save as PNG
        string pngFile = string.Format("{0}/{1}_{2}.png", Application.dataPath, "Screenshot", screenshotCounter);
        byte[] byteArray = render.EncodeToPNG();
        System.IO.File.WriteAllBytes(pngFile, byteArray);
        Debug.Log(string.Format("Saved {0} x {1} screenshot to {2}.", width, height, pngFile));

        // Free up space
        RenderTexture.ReleaseTemporary(tex);
        RenderTexture.active = Camera.main.targetTexture;
        cam.targetTexture = null;
        screenshotCounter += 1;

        yield return null;

    }

    public void TakeScreenshot(int width, int height) {

        cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        StartCoroutine("CaptureCoroutine");

    }

}
