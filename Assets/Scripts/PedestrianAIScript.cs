using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PedestrianAIScript : MonoBehaviour {
	public float lookAhead = 0.4f;
	public float speedLookAhead = 0.3f;

	public float moveSpeed = 3f;

	Tilemap traffic;
	Vector3 goal;
	PedestrianScript walking;
	Rigidbody2D body;
	LayerMask pedestrianMask;
	string lastDir;
	bool lastDirWasTurn;

	Dictionary<string, Vector3> dirVectors = new Dictionary<string, Vector3>() {
		{ "north", Vector3.up },
		{ "south", Vector3.down },
		{ "west", Vector3.left },
		{ "east", Vector3.right },
	};

	void Awake() {
		ConfigScript config = (ConfigScript)Object.FindObjectOfType(typeof(ConfigScript));

		traffic = GameObject.Find("Pedestrians Traffic").GetComponent<Tilemap>();
		walking = GetComponent<PedestrianScript>();
		body = GetComponent<Rigidbody2D>();
		pedestrianMask = LayerMask.GetMask("Pedestrians");

		Vector3 frontSide = transform.position;
		Vector3 cellCenter = traffic.CellToLocalInterpolated(traffic.WorldToCell(frontSide) +
			new Vector3(0.5f, 0.5f, 0));
		goal = GetNextCell(cellCenter);
	}

	public void GetWalkingActions() {
		Vector3 centerSprite = transform.position;
		Vector3 goalDist = goal - centerSprite;

		while (goalDist.magnitude < lookAhead + body.velocity.magnitude * speedLookAhead) {
			Vector3 nextGoal = GetNextCell(goal);
			if (nextGoal == goal) {
				break;
			}
			goal = nextGoal;
			goalDist = goal - centerSprite;
		}
		Debug.DrawLine(centerSprite, goal, Color.red, Time.deltaTime);

		Vector3 front = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(front, goal, pedestrianMask);

		
		transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), goal, moveSpeed * Time.deltaTime);
	}

	Vector3 GetNextCell(Vector3 currentCell) {
		TileBase currentTile = traffic.GetTile(traffic.WorldToCell(currentCell));
		if (currentTile == null || currentTile.name == null) {
			return currentCell;
		}
		string[] dirs = currentTile.name.Split(',');

		// Pick a direction at random from the dirs available on this tile.
		// Avoid taking two turns in a row if possible.
		// If a dir has a ! after it, it can only be taken if it matches the last dir.
		string dir;
		bool mustMatch;
		do {
			dir = dirs[Random.Range(0, dirs.Length)];
			mustMatch = dir.Contains("!");
			if (mustMatch) {
				dir = dir.Remove(dir.Length - 1);
			}
		}
		while ((dirs.Length > 1 && lastDirWasTurn && dir != lastDir) ||
			(mustMatch && dir != lastDir));

		lastDirWasTurn = lastDir != dir;
		lastDir = dir;
		return currentCell + dirVectors[dir];
	}
}
