using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeTimer : MonoBehaviour
{
    public GameObject explosion; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Instantiate(explosion, this.transform.position, Quaternion.identity); //Instantiate the explosion at the current grenade location
        Destroy(gameObject); //Destroy the grenade sprite        
    }
}
