using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopLightsScript : MonoBehaviour {
	public float speed = 10;
	public bool startOn = false;

	DrivingScript driving;
	Material material;
	bool lit;

	void Start() {
		driving = GetComponentInParent<DrivingScript>();
		material = GetComponent<SpriteRenderer>().material;

		setLights(startOn);
	}

	void Update() {
		// if (driving.playerControlled && Input.GetButtonDown("Cop lights")) {
		// 	setLights(!lit);
		// }

		if (lit) {
			foreach (Transform light in transform) {
				light.Rotate(new Vector3(speed, 0, 0));
			}
		}
	}

	void setLights(bool newLit) {
		lit = newLit;

		material.SetColor("_EmissionColor", lit ? Color.white : Color.black);

		foreach (Transform light in transform) {
			light.gameObject.SetActive(lit);
		}
	}
}
