using System.Collections;
using UnityEngine;

public class ScreenCaptureToMaterial : MonoBehaviour
{
    public Material writeToMaterial;
    public float hertz = 30f;

    new MeshRenderer renderer;
    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    float t = 0;
    void Update()
    {
        t += Time.deltaTime;
        if (t >= 1 / hertz)
        {
            t = 0;
            StartCoroutine(CaptureScreen());
        }
    }

    IEnumerator CaptureScreen ()
    {
        yield return new WaitForEndOfFrame();

        Destroy(writeToMaterial.GetTexture("_MainTex"));
        //Texture2D texture = Texture2D.whiteTexture;
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        writeToMaterial.SetTexture("_MainTex", texture);
    }
}
