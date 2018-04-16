using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingScript : MonoBehaviour {
	public GameObject Wheel;

	// Edit these values to customize car
	public float maxSpeed = 12;          // maximum speed car can attain
	public float maxSpeedRev = 8;        // maximum speed car can attain reversing

	// these values will largely depends on the density of the car
	public float engineAcc = 14;         // forward force, moving forward
	public float engineBrk = 16;         // braking force
	public float engineRev = 10;         // reverse force, moving backward

	public float wheelXoff = 14;         // relative x position of wheel in pixels
	public float wheelYoff = 13;         // relative y position of wheel in pixels
	public float maxWheelAngle = 30;     // the maximum angle the wheel can rotate
	public float wheelAngleVelocity = 5; // how quickly a wheel rotates in degrees per frame
	public float wheelMotorSpeed = 100;

	// *note: torque can be set to 0, and car will still turn!
	// also angular damping(box2d) greatly affects the turning capacity of the car.
	// public float torqueAcc = 6;          // extra torque on the car when turning
	// public float torqueRev = 8;          // extra torque on the car in reverse
	public float torqueDamp = 0.1f;      // how quickly the car will straighten when not turning
																	     // range: (0-1)

	public float drift = 0.5f;           // drift control, usually between (0-1) but can be higher
																	     // 0: no control | 1: high control

	public float ppu = 64;							 // pixels per unit

	Rigidbody2D body;
	GameObject[] wheels;
	float steerDir = 0;                  // the angle the wheels should be at

	void Start() {
		wheels = new GameObject[] {
			addWheel(-wheelXoff, wheelYoff, true, true),
			addWheel(wheelXoff, wheelYoff, true, true),
			addWheel(-wheelXoff, -wheelYoff, false, false),
			addWheel(wheelXoff, -wheelYoff, false, false),
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
		float acc = Input.GetAxisRaw("Vertical"); // up: -1, down: 1
		float steer = Input.GetAxisRaw("Horizontal"); // left: -1, right: 1

		float phySpeed = body.velocity.magnitude;
		// true if the car is moving forward, false when moving backwards
		bool movingForward = Vector2.Dot(body.velocity, transform.forward) > 0;

		float engine = 0;
		// get the force for each powered wheel
		if (acc == -1 && phySpeed <= maxSpeed) { // forward engine
			engine = -engineAcc;
		}
		else if (acc == 1) {
			if (movingForward) { // if moving forward use brake engine
				engine = engineBrk;
			}
			else if (phySpeed <= maxSpeedRev) { // use reverse engine
				engine = engineRev;
			}
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
			killSidewaysSpeed(wheel, drift);
		}

		// kill sideways speed of car
		killSidewaysSpeed(gameObject, drift);

		// // get torque value based acceleration
		// float torque = torqueAcc; // forward torque
		// float currentMaxSpeed = maxSpeed;
		// if (acc == -1 || !movingForward) {
		// 	torque = -torqueRev; // reverse torque
		// 	currentMaxSpeed = maxSpeedRev; // reverse max speed
		// }

		// // apply torque --- makes the car rotate faster
		// body.AddTorque(torque * Mathf.Sign(steer) * Mathf.Min(phySpeed * 2 / currentMaxSpeed, 1));

		// angular friction when not steering
		if (steerDir == 0) {
			body.angularVelocity -= Mathf.Sign(body.angularVelocity) * torqueDamp;
		}

		// prevent excess sliding
		if (phySpeed <= 0.2 && acc == 0) {
			body.velocity = Vector2.zero;
		}
	}

	void killSidewaysSpeed(GameObject obj, float drift) {
		Rigidbody2D body = obj.GetComponent<Rigidbody2D>();

		// project velocity to the axis and return the magnitude
		float mag = Vector2.Dot(obj.transform.right, body.velocity);

		// apply drift effect *allows some sideways speed by decreasing mag
		mag *= drift / Mathf.Max(body.velocity.magnitude, 1);

		// subtract the sideways speed from the total speed
		// note: decreasing mag from drifting will leave some sideways speed
		body.velocity -= mag * new Vector2(obj.transform.right.x, obj.transform.right.y);
	}

	static float getCurrentAngle(Transform transform) {
		float currentAngle = (360 - transform.localEulerAngles.z) % 360;
		if (currentAngle > 180) {
			currentAngle -= 360;
		}
		return currentAngle;
	}
}
