using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct DrivingActions {
	public float acc;
	public float steer;
}

public class DrivingAIScript : MonoBehaviour {
	public float accAmount = 0.2f;
	public float lookAhead = 0.4f;
	public float speedLookAhead = 0.3f;
	public float turnBraking = 1.1f;

	Tilemap traffic;
	Vector3 goal;
	DrivingScript driving;
	Rigidbody2D body;
	float frontDist;

	Dictionary<string, Vector3> dirs = new Dictionary<string, Vector3>() {
		{ "north", Vector3.up },
		{ "south", Vector3.down },
		{ "west", Vector3.left },
		{ "east", Vector3.right },
	};

	void Start() {
		ConfigScript config = (ConfigScript)Object.FindObjectOfType(typeof(ConfigScript));

		traffic = GameObject.Find("Traffic").GetComponent<Tilemap>();
		driving = GetComponent<DrivingScript>();
		body = GetComponent<Rigidbody2D>();

		// frontDist = GetComponent<BoxCollider2D>().size.y / 2; // front of car
		frontDist = driving.wheelYoff / config.pixelsPerUnit; // front wheels
		Vector3 front = transform.position + transform.up * frontDist;
		Vector3 cellCenter = traffic.CellToLocalInterpolated(traffic.WorldToCell(front) +
			new Vector3(0.5f, 0.5f, 0));
		goal = getNextCell(cellCenter);
	}

	public DrivingActions getDrivingActions() {
		Vector3 front = transform.position + transform.up * frontDist;
		Vector3 goalDist = goal - front;

		while (goalDist.magnitude < lookAhead + body.velocity.magnitude * speedLookAhead) {
			Vector3 nextGoal = getNextCell(goal);
			if (nextGoal == goal) {
				break;
			}
			goal = nextGoal;
			goalDist = goal - front;
		}

		float thisAngleDiff = Vector3.SignedAngle(transform.up, goalDist, -transform.forward);
		// Debug.DrawLine(Vector3.zero, goal, Color.red, Time.deltaTime);

		float steer = Mathf.Clamp(thisAngleDiff / driving.maxWheelAngle, -1, 1);

		return new DrivingActions() {
			acc = accAmount * (1 - Mathf.Abs(steer * turnBraking)),
			steer = steer,
		};
	}

	Vector3 getNextCell(Vector3 currentCell) {
		TileBase goalTile = traffic.GetTile(traffic.WorldToCell(currentCell));
		if (goalTile == null || goalTile.name == null) {
			return currentCell;
		}
		return currentCell + dirs[goalTile.name];
	}
}
