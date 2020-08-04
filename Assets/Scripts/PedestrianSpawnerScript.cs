using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PedestrianSpawnerScript : MonoBehaviour {
	public int targetCount = 5;
	public float interval = 1;
	public int minSpawnMargin = 2;
	public int maxSpawnMargin = 7;
	public float clearRadius = 3;
	public PedestrianScript[] pedestrianPrefabs;
	//public DrivingScript playerCar;

	Vector3 halfCell = new Vector3(0.5f, 0.5f, 0);
	List<string> dirs = new List<string>(new string[] { "north", "west", "south", "east" });

	Camera cam;
	float height;
	Tilemap pedestrians;
	int layerMask;
	Vector3 margin;
	float elapsed = 0;

	void Awake()
	{
		//SpawnPlayer();
	}

	void Start() {
		cam = Camera.main;
		height = cam.GetComponent<CameraFollowScript>().height;
		pedestrians = GameObject.Find("Pedestrians Traffic").GetComponent<Tilemap>();
		layerMask = LayerMask.GetMask("Pedestrians");

		margin = new Vector3(maxSpawnMargin, maxSpawnMargin, 0);

		//SpawnPlayer();

		BoundsInt outerCellBounds = GetOuterCellBounds();
		SpawnPedestrians(outerCellBounds);
		RemovePedestrians(outerCellBounds);
	}

	void Update() {
		elapsed += Time.deltaTime;
		if (elapsed >= interval) {
			elapsed -= interval;

			BoundsInt outerCellBounds = GetOuterCellBounds();
			SpawnPedestrians(outerCellBounds);
			RemovePedestrians(outerCellBounds);
		}
	}

	BoundsInt GetOuterCellBounds() {
		Vector3 cameraMin = cam.ViewportToWorldPoint(new Vector3(0, 0, height));
		Vector3 cameraMax = cam.ViewportToWorldPoint(new Vector3(1, 1, height));

		BoundsInt outerBoundsInt = new BoundsInt();
		outerBoundsInt.SetMinMax(
			pedestrians.WorldToCell(new Vector3(cameraMin.x, cameraMin.y, 0) - margin),
			pedestrians.WorldToCell(new Vector3(cameraMax.x, cameraMax.y, 0) + margin) + Vector3Int.one);

		return outerBoundsInt;
	}

	Bounds GetOuterBounds(BoundsInt outerCellBounds) {
		Bounds outerBounds = new Bounds();
		outerBounds.SetMinMax(outerCellBounds.min, outerCellBounds.max);
		return outerBounds;
	}

	void SpawnPedestrians(BoundsInt outerBoundsInt) {
		TileBase[] tiles = pedestrians.GetTilesBlock(outerBoundsInt);
		int marginWidth = maxSpawnMargin - minSpawnMargin;
		int farMarginX = outerBoundsInt.size.x - marginWidth;
		int farMarginY = outerBoundsInt.size.y - marginWidth;

		int carCount = Physics2D.OverlapAreaAll(
			(Vector3)outerBoundsInt.min, (Vector3)outerBoundsInt.max, layerMask).Length;

		foreach (int y in Enumerable.Range(0, outerBoundsInt.size.y).OrderBy(y => Random.value)) {
			foreach (int x in Enumerable.Range(0, outerBoundsInt.size.x).OrderBy(x => Random.value)) {
				// Only use tiles that are within the margin but not visible to the camera.
				if (carCount < targetCount &&
					(x < marginWidth || y < marginWidth || x >= farMarginX || y >= farMarginY)) {
					TileBase tile = tiles[y * outerBoundsInt.size.x + x];
					Vector3 point = pedestrians.CellToWorld(
						new Vector3Int(outerBoundsInt.xMin + x, outerBoundsInt.yMin + y, 0)) + halfCell;
					int dir = tile == null ? -1 : dirs.IndexOf(tile.name);

					if (dir > -1 && Physics2D.OverlapCircle(point, clearRadius, layerMask) == null) {
						PedestrianScript pedestrian = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];
						Instantiate(pedestrian, point + new Vector3(0, 0, pedestrian.transform.position.z),
							Quaternion.Euler(0, 0, 0 * dir), transform);

						carCount += 1;
					}
				}
			}
		}
	}

	void RemovePedestrians(BoundsInt outerCellBounds) {
		Bounds outerBounds = GetOuterBounds(outerCellBounds);

		foreach (GameObject car in GameObject.FindGameObjectsWithTag("pedestrian")) {
			if (!outerBounds.Contains(
				new Vector3(car.transform.position.x, car.transform.position.y, 0))) {
				Destroy(car);
			}
		}
	}
}
