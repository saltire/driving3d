using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthographicCameraScript : MonoBehaviour {
  public float unitHeight = 9;
  public float pixelHeight = 360;

  int lastSize = 0;
  Camera cam;

  void Start() {
    cam = GetComponent<Camera>();
    UpdateOrthoSize();
  }

  void Update() {
    if (lastSize != Screen.height) {
      UpdateOrthoSize();
    }
  }

  void UpdateOrthoSize() {
    lastSize = Screen.height;

    float multiplier;
    if (Screen.height < pixelHeight) {
      // Find a power of 2 to divide the display size by to fit within the screen.
      multiplier = 1 / Mathf.Pow(2, Mathf.Ceil(Mathf.Log(pixelHeight / (float)Screen.height, 2)));
    }
    else {
      // Find a whole number to multiply the display size by and fit within the screen.
      multiplier = Mathf.Floor(Screen.height / pixelHeight);
    }

    cam.orthographicSize = Screen.height / (pixelHeight * multiplier) * unitHeight * 0.5f;
  }
}
