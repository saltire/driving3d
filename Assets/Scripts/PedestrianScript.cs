using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianScript : MonoBehaviour {
	
	public bool playerControlled = false;

	float ppu;
	Rigidbody2D body;
	PedestrianAIScript ai;
	public float moveSpeed = 0.1f;

	void Start() {
		ConfigScript config = (ConfigScript)Object.FindObjectOfType(typeof(ConfigScript));
		ppu = config.pixelsPerUnit;

		body = GetComponent<Rigidbody2D>();
		ai = GetComponent<PedestrianAIScript>();
	}

	void FixedUpdate() {
		if (ai != null) {
			ai.GetWalkingActions();
		}
	}
}	
