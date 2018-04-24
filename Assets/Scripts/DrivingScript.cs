using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingScript : MonoBehaviour {
	public bool playerControlled = false;
	public bool frontWheelDrive = false;

	// Edit these values to customize car
	public float airResistance = 0.2f;     // air drag
	public float rollingResistance = 1.5f; // road friction
	public float brakeResistance = 15;     // handbrake friction

	// these values will largely depends on the density of the car
	public float engineAcc = 14;           // forward force, moving forward
	public float engineBrk = 16;           // braking force
	public float engineRev = 10;           // reverse force, moving backward

	public float wheelXoff = 14;           // relative x position of wheel in pixels
	public float wheelYoff = 13;           // relative y position of wheel in pixels
	public float maxWheelAngle = 30;       // the maximum angle the wheel can rotate
	public float wheelAngleVelocity = 5;   // how quickly a wheel rotates in degrees per frame
	public float wheelMotorSpeed = 100;

	public float steerTorque = 5;          // extra torque on the car when turning (multiplied by speed)
	public float maxSteerTorque = 20;      // max amount of extra steering torque
	public float brakeTorque = 8;          // extra torque when handbraking
	public float torqueDamp = 0.75f;       // how quickly the car will straighten when not turning

	public float driftControl = 0.5f;      // drift control, usually between (0-1) but can be higher

	public GameObject wheelPrefab;

	struct Wheel {
		public GameObject obj;
		public bool powered;
		public bool rotatable;
		public bool handbrake;
	}

	Wheel[] wheels;
	float ppu;
	float steerDir = 0;                    // the angle the wheels should be at
	Rigidbody2D body;
	DrivingAIScript ai;

	void Start() {
		ConfigScript config = (ConfigScript)Object.FindObjectOfType(typeof(ConfigScript));
		ppu = config.pixelsPerUnit;

		wheels = new Wheel[] {
			addWheel(-wheelXoff, wheelYoff, frontWheelDrive, true, false),
			addWheel(wheelXoff, wheelYoff, frontWheelDrive, true, false),
			addWheel(-wheelXoff, -wheelYoff, !frontWheelDrive, false, true),
			addWheel(wheelXoff, -wheelYoff, !frontWheelDrive, false, true),
		};

		body = GetComponent<Rigidbody2D>();
		ai = GetComponent<DrivingAIScript>();
	}

	Wheel addWheel(float xoff, float yoff, bool powered, bool rotatable, bool handbrake) {
		Wheel wheel = new Wheel {
			obj = (GameObject)Instantiate(wheelPrefab, transform),
			powered = powered,
			rotatable = rotatable,
			handbrake = handbrake,
		};

		wheel.obj.transform.localPosition += new Vector3(xoff / ppu, yoff / ppu, 0);

		if (rotatable) {
			HingeJoint2D hingeJoint = wheel.obj.AddComponent<HingeJoint2D>();
			hingeJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
			hingeJoint.useLimits = true;
			hingeJoint.limits = new JointAngleLimits2D() { max = maxWheelAngle, min = -maxWheelAngle };
			hingeJoint.useMotor = true;
		}
		else {
			FixedJoint2D fixedJoint = wheel.obj.AddComponent<FixedJoint2D>();
			fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
		}

		return wheel;
	}

	void FixedUpdate() {
		float acc = 0;
		float steer = 0;
		float brake = 0;

		if (playerControlled) {
			acc = Input.GetAxisRaw("Vertical");     // up: 1, down: -1
			steer = Input.GetAxisRaw("Horizontal"); // left: -1, right: 1
			brake = Input.GetAxisRaw("Handbrake");
		}
		else if (ai != null) {
			DrivingActions actions = ai.getDrivingActions();
			acc = actions.acc;
			steer = actions.steer;
		}

		bool movingForward = Vector2.Dot(body.velocity, transform.up) > 0;

		// get the engine force for each powered wheel
		float engine = 0;
		if (acc > 0) {
			engine = engineAcc * acc;
		}
		else if (acc < 0) {
			engine = (movingForward ? engineBrk : engineRev) * acc;
		}

		steerDir += Mathf.Clamp((steer * maxWheelAngle) - steerDir,
			-wheelAngleVelocity, wheelAngleVelocity);

		foreach (Wheel wheel in wheels) {
			Rigidbody2D wheelBody = wheel.obj.GetComponent<Rigidbody2D>();

			// set the rotation(angle) for the rotatable wheels
			if (wheel.rotatable) {
				float currentAngle = getCurrentAngle(wheel.obj.transform);
				float angleDiff = steerDir - currentAngle;

				HingeJoint2D joint = wheel.obj.GetComponent<HingeJoint2D>();
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
			if (wheel.powered) {
				wheelBody.AddRelativeForce(new Vector2(0, engine));
			}

			Vector2 localWheelVelocity = transform.InverseTransformVector(wheelBody.velocity);

			// add rolling resistance
			Vector2 rollingVector = new Vector2(0, localWheelVelocity.y) * -rollingResistance;
			body.AddRelativeForce(rollingVector);
			// drawVector(wheel.transform, transform.rotation * rollingVector, Color.red);

			// add handbrake resistance
			if (brake == 1 && wheel.handbrake) {
				Vector2 brakeVector = new Vector2(0, localWheelVelocity.y) * -brakeResistance;
				wheelBody.AddRelativeForce(brakeVector);
				// drawVector(wheel.transform, transform.rotation * brakeVector, Color.magenta);
			}

			// kill sideways speed of wheel
			killSidewaysSpeed(wheel.obj);
		}

		// kill sideways speed of car
		killSidewaysSpeed(gameObject);

		// add extra torque when steering, to make up for angular drag
		float torque = (movingForward ? -1 : 1) * steer *
			(Mathf.Min(steerTorque * body.velocity.magnitude, maxSteerTorque) +
			(brake * brakeTorque * body.velocity.magnitude));
		body.AddTorque(torque);

		// prevent excess sliding
		if (body.velocity.magnitude <= 0.2 && acc == 0) {
			body.velocity = Vector2.zero;
		}

		// angular friction when not steering
		if (steerDir == 0) {
			body.angularVelocity -= Mathf.Sign(body.angularVelocity) * torqueDamp;
		}

		// add air resistance
		Vector2 airVector = body.velocity * body.velocity.magnitude * -airResistance;
		body.AddForce(airVector);
		// drawVector(transform, airVector, Color.blue);

		// Debug.Log(body.velocity.magnitude);
		// drawVector(transform, body.velocity, Color.green);
	}

	void killSidewaysSpeed(GameObject obj) {
		Rigidbody2D body = obj.GetComponent<Rigidbody2D>();

		// project velocity to the axis and return the magnitude
		float mag = Vector2.Dot(obj.transform.right, body.velocity);

		// apply drift effect *allows some sideways speed by decreasing mag
		mag *= driftControl / Mathf.Max(body.velocity.magnitude, 1);

		Vector2 sideVelocity = mag * obj.transform.right;
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
			transform.position + (Vector3)vector, color, Time.deltaTime, false);
	}
}
