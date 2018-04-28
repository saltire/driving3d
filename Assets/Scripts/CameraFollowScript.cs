using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {
	public float height = 10;
	public float lead = 32;
	public float speed = 0.05f;
	public float maxLeadDistance = 5;

	Transform target;
	Vector3 heightDistance;
	Vector3 lastTargetPosition;

	void Awake() {
		heightDistance = Vector3.back * height;

		DrivingScript[] cars = Component.FindObjectsOfType<DrivingScript>();
		DrivingScript playerCar = Array.Find<DrivingScript>(cars, c => c.playerControlled);
		if (playerCar) {
			target = playerCar.transform;
			transform.position = target.position + heightDistance;
			lastTargetPosition = target.position;
		}
	}

	void FixedUpdate() {
		if (target) {
			Vector3 leadDistance = (target.position - lastTargetPosition) * lead;
			// Confine camera distance to a circle.
			if (leadDistance.magnitude > maxLeadDistance) {
				float scale = maxLeadDistance / leadDistance.magnitude;
				leadDistance = Vector3.Scale(leadDistance, new Vector3(scale, scale, 0));
			}
			// Confine camera distance to a box.
			// leadDistance = Vector3.Min(leadDistance, new Vector3(maxLeadDistance, maxLeadDistance, 0));

			transform.position = Vector3.Lerp(transform.position,
				target.position + leadDistance + heightDistance, speed);
			lastTargetPosition = target.position;
		}
	}
}
