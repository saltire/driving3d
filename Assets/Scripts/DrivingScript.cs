﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingScript : MonoBehaviour {
	public bool playerControlled = false;
	public bool frontWheelDrive = false;

	// Edit these values to customize car
	public float airResistance = 0.2f;   // air drag
	public float rollingResistance = 5;  // road friction

	// these values will largely depends on the density of the car
	public float engineAcc = 14;         // forward force, moving forward
	public float engineBrk = 16;         // braking force
	public float engineRev = 10;         // reverse force, moving backward

	public float wheelXoff = 14;         // relative x position of wheel in pixels
	public float wheelYoff = 13;         // relative y position of wheel in pixels
	public float maxWheelAngle = 30;     // the maximum angle the wheel can rotate
	public float wheelAngleVelocity = 5; // how quickly a wheel rotates in degrees per frame
	public float wheelMotorSpeed = 100;

	// public float steerTorque = 6;        // extra torque on the car when turning
	public float torqueDamp = 0.75f;     // how quickly the car will straighten when not turning
																	     // range: (0-1)

	public float driftControl = 0.5f;    // drift control, usually between (0-1) but can be higher
																	     // 0: no control | 1: high control

	public float ppu = 64;							 // pixels per unit

	public GameObject Wheel;

	Rigidbody2D body;
	GameObject[] wheels;
	float steerDir = 0;                  // the angle the wheels should be at

	void Start() {
		wheels = new GameObject[] {
			addWheel(-wheelXoff, wheelYoff, frontWheelDrive, true),
			addWheel(wheelXoff, wheelYoff, frontWheelDrive, true),
			addWheel(-wheelXoff, -wheelYoff, !frontWheelDrive, false),
			addWheel(wheelXoff, -wheelYoff, !frontWheelDrive, false),
		};

		body = GetComponent<Rigidbody2D>();
	}

	GameObject addWheel(float xoff, float yoff, bool powered, bool rotatable) {
		GameObject wheel = (GameObject)Instantiate(Wheel,
			transform.position + (transform.rotation * new Vector3(xoff / ppu, yoff / ppu, 0)),
			transform.rotation, transform);

		WheelScript scr = wheel.GetComponent<WheelScript>();
		scr.powered = powered;
		scr.rotatable = rotatable;

		if (rotatable) {
			HingeJoint2D hingeJoint = wheel.AddComponent<HingeJoint2D>();
			hingeJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
			hingeJoint.useLimits = true;
			hingeJoint.limits = new JointAngleLimits2D() { max = maxWheelAngle, min = -maxWheelAngle };
			hingeJoint.useMotor = true;
		}
		else {
			FixedJoint2D fixedJoint = wheel.AddComponent<FixedJoint2D>();
			fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
		}

		return wheel;
	}

	void FixedUpdate() {
		float acc = 0;
		float steer = 0;

		if (playerControlled) {
			acc = Input.GetAxisRaw("Vertical");     // up: 1, down: -1
			steer = Input.GetAxisRaw("Horizontal"); // left: -1, right: 1
		}

		bool movingForward = Vector2.Dot(body.velocity, transform.up) > 0;

		// get the engine force for each powered wheel
		float engine = 0;
		if (acc == 1) {
			engine = engineAcc;
		}
		else if (acc == -1) {
			engine = (movingForward ? -engineBrk : -engineRev);
		}

		steerDir += Mathf.Clamp((steer * maxWheelAngle) - steerDir, -wheelAngleVelocity, wheelAngleVelocity);

		foreach (GameObject wheel in wheels) {
			WheelScript wheelScr = wheel.GetComponent<WheelScript>();
			Rigidbody2D wheelBody = wheel.GetComponent<Rigidbody2D>();

			// set the rotation(angle) for the rotatable wheels
			if (wheelScr.rotatable) {
				float currentAngle = getCurrentAngle(wheel.transform);
				float angleDiff = steerDir - currentAngle;

				HingeJoint2D joint = wheel.GetComponent<HingeJoint2D>();
				JointMotor2D motor = joint.motor;
				if (Mathf.Abs(angleDiff) > .01) {
					motor.motorSpeed = wheelMotorSpeed * Mathf.Sign(angleDiff);
					joint.motor = motor;
				}
				else if (motor.motorSpeed != 0) {
					motor.motorSpeed = 0;
					joint.motor = motor;
				}
				// wheel.transform.Rotate(new Vector3(0, 0, currentAngle - steerDir));

				// prevent random wheel joint physics
				wheelBody.angularVelocity = 0;
			}

			// apply engine force
			if (wheelScr.powered) {
				wheelBody.AddRelativeForce(new Vector2(0, engine));
			}

			// kill sideways speed of wheel
			killSidewaysSpeed(wheel);
		}

		// kill sideways speed of car
		killSidewaysSpeed(gameObject);

		// // add torque value based acceleration
		// float torque = acc * steer * -steerTorque; // forward torque
		// float currentMaxSpeed = (acc < 0 ? maxSpeedRev : maxSpeed);
		// body.AddTorque(torque * Mathf.Min(body.velocity.magnitude * 2 / currentMaxSpeed, 1));

		// prevent excess sliding
		if (body.velocity.magnitude <= 0.2 && acc == 0) {
			body.velocity = Vector2.zero;
		}

		// angular friction when not steering
		if (steerDir == 0) {
			body.angularVelocity -= Mathf.Sign(body.angularVelocity) * torqueDamp;
		}

		// angular friction when not moving
		// if (body.velocity.magnitude < 5) {
		// 	body.angularVelocity -= body.angularVelocity * torqueDamp;
		// }

		// Debug.Log(body.velocity.magnitude);
		// drawVector(transform, body.velocity, Color.green);

		// add air and rolling resistance
		Vector2 localVelocity = transform.InverseTransformVector(body.velocity);

		Vector2 airVector = localVelocity * localVelocity.magnitude * -airResistance;
		body.AddRelativeForce(airVector);
		// drawVector(transform, transform.rotation * airVector, Color.blue);

		Vector2 rollingVector = new Vector2(0, localVelocity.y) * -rollingResistance;
		body.AddRelativeForce(rollingVector);
		// drawVector(transform, transform.rotation * rollingVector, Color.red);
	}

	void killSidewaysSpeed(GameObject obj) {
		Rigidbody2D body = obj.GetComponent<Rigidbody2D>();

		// project velocity to the axis and return the magnitude
		float mag = Vector2.Dot(obj.transform.right, body.velocity);

		// apply drift effect *allows some sideways speed by decreasing mag
		mag *= driftControl / Mathf.Max(body.velocity.magnitude, 1);

		Vector2 sideVelocity = mag * new Vector2(obj.transform.right.x, obj.transform.right.y);
		// drawVector(obj.transform, sideVelocity, Color.white);

		// subtract the sideways speed from the total speed
		// note: decreasing mag from drifting will leave some sideways speed
		body.velocity -= sideVelocity;
	}

	static float getCurrentAngle(Transform transform) {
		float currentAngle = (360 - transform.localEulerAngles.z) % 360;
		if (currentAngle > 180) {
			currentAngle -= 360;
		}
		return currentAngle;
	}

	static void drawVector(Transform transform, Vector2 vector, Color color) {
		Debug.DrawLine(transform.position,
			transform.position + new Vector3(vector.x, vector.y, 0), color, Time.deltaTime, false);
	}
}
