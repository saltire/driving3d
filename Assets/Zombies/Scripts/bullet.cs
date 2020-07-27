using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float dmg = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "zombie" || other.tag == "soldier") {
            other.transform.SendMessage("Damage", dmg);
            Destroy(this.gameObject);
        }

        if(other.tag == "building" || other.tag == "explosion") {
            Destroy(this.gameObject);
        }        
    }    
}
