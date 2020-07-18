using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightsScript : MonoBehaviour {
	public GameObject headlightPrefab;
	public Transform headlightOffset;
	public bool startOn = true;

	float ppu;
	DrivingScript driving;
	GameObject[] headlights;

	void Start() {
		ConfigScript config = (ConfigScript)Object.FindObjectOfType(typeof(ConfigScript));
		ppu = config.pixelsPerUnit;

		driving = GetComponent<DrivingScript>();

		headlights = new GameObject[] {
			AddHeadlight(true),
			AddHeadlight(false),
		};
		foreach (GameObject light in headlights) {
			light.SetActive(startOn);
		}
	}

	GameObject AddHeadlight(bool flipX) {
		GameObject headlight = Instantiate(headlightPrefab, transform);
		headlight.transform.localPosition += new Vector3(
			headlightOffset.localPosition.x * (flipX ? -1 : 1), headlightOffset.localPosition.y, 0);
		return headlight;
	}

	void Update() {
		if (driving.playerControlled && Input.GetButtonDown("Headlights")) {
			foreach (GameObject light in headlights) {
				light.SetActive(!light.activeSelf);
			}
		}
	}
}
