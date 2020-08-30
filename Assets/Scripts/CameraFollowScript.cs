using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {
	public float lead = 32;
	public float cameraLag = 0.05f;
	public float maxLeadDistance = 5;
	public float maxZoomOut = 1.5f;

	Transform car;
	Vector3 lastCarPosition;
	float initialSize;
	Vector3 distanceVector;

	Camera captureCamera;
	public Camera perspectiveCamera;

	void Awake() {
		DrivingScript[] cars = Component.FindObjectsOfType<DrivingScript>();
		DrivingScript playerCar = Array.Find<DrivingScript>(cars, c => c.playerControlled);
		if (playerCar) {
			SetPlayerCar(playerCar);
		}

		captureCamera = GetComponent<Camera>();
		initialSize = captureCamera.orthographicSize;
		distanceVector = Vector3.forward * transform.position.z;
	}

	public void SetPlayerCar(DrivingScript playerCar) {
		car = playerCar.transform;
		transform.position = car.position + distanceVector;
		lastCarPosition = car.position;
	}

	void SetPerspectiveCameraDistance() {
		float distance = captureCamera.orthographicSize /
			Mathf.Tan(perspectiveCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		perspectiveCamera.transform.position = new Vector3(
			perspectiveCamera.transform.position.x, perspectiveCamera.transform.position.y, -distance);
	}

	void SetPerspectiveCameraFOV() {
		perspectiveCamera.fieldOfView = 2.0f *
			Mathf.Atan(captureCamera.orthographicSize / -perspectiveCamera.transform.position.z) *
			Mathf.Rad2Deg;
	}

	void FixedUpdate() {
		if (car) {
			Vector3 leadVector = Vector3.ClampMagnitude(
				(car.position - lastCarPosition) * lead, maxLeadDistance);

			Vector3 targetPosition = car.position + leadVector + distanceVector;
			transform.position = Vector3.Lerp(transform.position, targetPosition, cameraLag);

			float targetSize = initialSize *
				Mathf.Lerp(1, maxZoomOut, leadVector.magnitude / maxLeadDistance);
			captureCamera.orthographicSize = Mathf.Lerp(captureCamera.orthographicSize, targetSize,
				cameraLag);

			// SetPerspectiveCameraDistance();
			SetPerspectiveCameraFOV();

			lastCarPosition = car.position;
		}
	}
}
