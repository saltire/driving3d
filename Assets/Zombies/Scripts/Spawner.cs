using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
 
    public float spawnTime;        // The amount of time between each spawn.
    public float spawnDelay;       // The amount of time before spawning starts.
    public GameObject enemy;
 
    public int maxDistance;
    public Transform target;
    public Transform myTransform;
    public CarSpawnerScript carSpawner;
 
    void Awake()
    {
        myTransform = transform;
    }
 
    void Start()
    {
        GameObject stop = carSpawner.playerCar.gameObject;
    
        target = stop.transform;
    
        maxDistance = 5;
    
        StartCoroutine(SpawnTimeDelay());      
    }
 
    IEnumerator SpawnTimeDelay()
    {
        while (true)
        {
            if (Vector2.Distance(target.position, myTransform.position) < maxDistance)
            {
                Instantiate(enemy, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(spawnTime);
            }
 
            if (Vector2.Distance(target.position, myTransform.position) > maxDistance)
            {
                yield return null;
            }                
        }
    }
}
