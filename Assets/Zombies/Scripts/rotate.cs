using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update ()
    {
        transform.Rotate (0,0,50 * rotateSpeed *Time.deltaTime); //rotates 50 degrees per second around z axis
    }
}
