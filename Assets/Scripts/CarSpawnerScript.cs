using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CarSpawnerScript : MonoBehaviour {
	public int targetCarCount = 5;
	public float interval = 1;
	public int minSpawnMargin = 2;
	public int maxSpawnMargin = 7;
	public float clearRadius = 3;
	public DrivingScript playerCarPrefab;

	DrivingScript[] carPrefabs;

	Vector3 halfCell = new Vector3(0.5f, 0.5f, 0);
	List<string> dirs = new List<string>(new string[] { "north", "west", "south", "east" });

	Camera cam;
	Tilemap traffic;
	int layerMask;
	Vector3 margin;
	float elapsed = 0;

	void Start() {
		cam = Camera.main;
		traffic = GameObject.Find("Traffic").GetComponent<Tilemap>();
		layerMask = LayerMask.GetMask("Cars");

		margin = new Vector3(maxSpawnMargin, maxSpawnMargin, 0);

		carPrefabs = Resources.LoadAll<DrivingScript>("/");

		SpawnPlayer();

		BoundsInt outerCellBounds = GetOuterCellBounds();
		SpawnCars(outerCellBounds);
		RemoveCars(outerCellBounds);
	}

	void Update() {
		elapsed += Time.deltaTime;
		if (elapsed >= interval) {
			elapsed -= interval;

			BoundsInt outerCellBounds = GetOuterCellBounds();
			SpawnCars(outerCellBounds);
			RemoveCars(outerCellBounds);
		}
	}

	BoundsInt GetOuterCellBounds() {
		Vector3 cameraMin = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
		Vector3 cameraMax = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

		BoundsInt outerBoundsInt = new BoundsInt();
		outerBoundsInt.SetMinMax(
			traffic.WorldToCell(new Vector3(cameraMin.x, cameraMin.y, 0) - margin),
			traffic.WorldToCell(new Vector3(cameraMax.x, cameraMax.y, 0) + margin) + Vector3Int.one);

		return outerBoundsInt;
	}

	Bounds GetOuterBounds(BoundsInt outerCellBounds) {
		Bounds outerBounds = new Bounds();
		outerBounds.SetMinMax(outerCellBounds.min, outerCellBounds.max);
		return outerBounds;
	}

  void SpawnPlayer() {
    DrivingScript carPrefab = playerCarPrefab != null ? playerCarPrefab :
			carPrefabs[Random.Range(0, carPrefabs.Length)];
    DrivingScript playerCar = Instantiate(carPrefab,
      transform.position + new Vector3(0, 0, carPrefab.transform.position.z), transform.rotation);

    playerCar.playerControlled = true;
		FindObjectOfType<CameraFollowScript>().SetPlayerCar(playerCar);
		FindObjectOfType<InputScript>().SetPlayerCar(playerCar);
  }

	void SpawnCars(BoundsInt outerBoundsInt) {
		TileBase[] tiles = traffic.GetTilesBlock(outerBoundsInt);
		int marginWidth = maxSpawnMargin - minSpawnMargin;
		int farMarginX = outerBoundsInt.size.x - marginWidth;
		int farMarginY = outerBoundsInt.size.y - marginWidth;

		int carCount = Physics2D.OverlapAreaAll(
			(Vector3)outerBoundsInt.min, (Vector3)outerBoundsInt.max, layerMask).Length;

		foreach (int y in Enumerable.Range(0, outerBoundsInt.size.y).OrderBy(y => Random.value)) {
			foreach (int x in Enumerable.Range(0, outerBoundsInt.size.x).OrderBy(x => Random.value)) {
				// Only use tiles that are within the margin but not visible to the camera.
				if (carCount < targetCarCount &&
					(x < marginWidth || y < marginWidth || x >= farMarginX || y >= farMarginY)) {
					TileBase tile = tiles[y * outerBoundsInt.size.x + x];
					Vector3 point = traffic.CellToWorld(
						new Vector3Int(outerBoundsInt.xMin + x, outerBoundsInt.yMin + y, 0)) + halfCell;
					int dir = tile == null ? -1 : dirs.IndexOf(tile.name);

					if (dir > -1 && Physics2D.OverlapCircle(point, clearRadius, layerMask) == null) {
						DrivingScript car = carPrefabs[Random.Range(0, carPrefabs.Length)];
						Instantiate(car, point + new Vector3(0, 0, car.transform.position.z),
							Quaternion.Euler(0, 0, 90 * dir), transform);

						carCount += 1;
					}
				}
			}
		}
	}

	void RemoveCars(BoundsInt outerCellBounds) {
		Bounds outerBounds = GetOuterBounds(outerCellBounds);

		foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car")) {
			if (!outerBounds.Contains(
				new Vector3(car.transform.position.x, car.transform.position.y, 0))) {
				Destroy(car);
			}
		}
	}
}
