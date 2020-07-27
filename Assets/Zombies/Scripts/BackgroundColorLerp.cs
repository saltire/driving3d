// ping-pong animate background color
using UnityEngine;
using System.Collections;

public class BackgroundColorLerp : MonoBehaviour
{
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public float duration = 0.01f;

    public Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    public void LerpBackgroundColor()
    {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        cam.backgroundColor = Color.Lerp(color1, color2, t);
    }

    public void LerpBackgroundColorBack()
    {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, color1, t);
    }    
}