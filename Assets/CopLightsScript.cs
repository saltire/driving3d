using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopLightsScript : MonoBehaviour {
	public float speed = 10;

	void Update() {
		foreach (Light light in GetComponentsInChildren<Light>()) {
			light.transform.Rotate(new Vector3(speed, 0, 0));
		}
	}
}
