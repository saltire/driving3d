using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {
	public Transform target;
	public float speed = 0.1f;

	Vector3 distance;

	void Start() {
		distance = Vector3.back * 10;
		transform.position = transform.position + distance;
	}

	void FixedUpdate() {
		transform.position = Vector3.Lerp(transform.position, target.position + distance,
			speed);
	}
}
