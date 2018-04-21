using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {
	public Transform target;
	public float lead = 32;
	public float speed = 0.05f;
	public float maxLeadDistance = 5;

	Vector3 height = Vector3.back * 10;
	Vector3 lastTargetPosition;

	void Start() {
		transform.position = target.position + height;
		lastTargetPosition = target.position;
	}

	void FixedUpdate() {
		Vector3 leadDistance = (target.position - lastTargetPosition) * lead;
		// Confine camera distance to a circle.
		if (leadDistance.magnitude > maxLeadDistance) {
			float scale = maxLeadDistance / leadDistance.magnitude;
			leadDistance = Vector3.Scale(leadDistance, new Vector3(scale, scale, 0));
		}
		// Confine camera distance to a box.
		// leadDistance = Vector3.Min(leadDistance, new Vector3(maxLeadDistance, maxLeadDistance, 0));

		transform.position = Vector3.Lerp(transform.position,
			target.position + leadDistance + height, speed);
		lastTargetPosition = target.position;
	}
}
