using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PedestrianAIScript : MonoBehaviour {
	public float lookAhead = 0.4f;
	public float speedLookAhead = 0.3f;

	public float moveSpeed = 3f;

	private float oldPosition;

	public SpriteRenderer thisRenderer;

	Tilemap traffic;
	Vector3 goal;
	PedestrianScript walking;
	Rigidbody2D body;
	LayerMask pedestrianMask;
	string lastDir;

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

	void Start() {
		oldPosition = transform.position.x;
	}

	void Update() {
        if (transform.position.x > oldPosition) // he's looking right
        {
            thisRenderer.flipX = false;
        }

        if (transform.position.x < oldPosition) // he's looking left
        {
            thisRenderer.flipX = true;
        }
        oldPosition = transform.position.x; // update the variable with the new position so we can check against it next frame
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

		// Pick a direction at random from the dirs available on this tile.
		string[] dirs = currentTile.name.Split(',');

		// If a dir has a ! after it, it can only be taken if it matches the last dir.
		string[] validDirs = dirs
			.Where(d => !d.EndsWith("!") || lastDir == null || d.StartsWith(lastDir))
			.ToArray();

		if (validDirs.Length == 0) {
			Util.Log("No valid directions for", currentTile.name, "from", lastDir);
			return currentCell;
		}

		// Avoid doubling back if possible.
		string[] validDirsNoDoubleBack = validDirs
			// Opposite vectors have a dot product of -1.
			.Where(d => lastDir == null || Vector3.Dot(dirVectors[d], dirVectors[lastDir]) > -1)
			.ToArray();

		string[] availableDirs = (validDirsNoDoubleBack.Length > 0 ? validDirsNoDoubleBack : validDirs)
			.Select(d => d.Replace("!", ""))
			.ToArray();

		string dir = availableDirs[Random.Range(0, availableDirs.Length)];

		lastDir = dir;
		return currentCell + dirVectors[dir];
	}
}
