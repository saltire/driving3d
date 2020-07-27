using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
    public GameObject gun;
    public GameObject laserParticles;
    private LineRenderer _lineRenderer;

    // Use this for initialization
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource laserSFX = gameObject.GetComponent<AudioSource>();
        
        _lineRenderer.SetPosition(0, transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        if (hit.collider)
        {
            _lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, transform.position.z));
            //Instantiate(laserParticles, hit.point, Quaternion.identity);
            laserSFX.Play();
            
        }
        else
        {
            _lineRenderer.SetPosition(1, transform.up * 2000);
        }

        if (hit.transform.tag == "zombie") {
            hit.collider.GetComponent<damage>().health -= 2.0f;
            hit.collider.GetComponent<flash>().FlashRed();
            Instantiate(laserParticles, hit.point, Quaternion.identity);
        }

        if (hit.transform.tag == "soldier") {
            hit.collider.GetComponent<damageSoldier>().health -= 2.0f;
            hit.collider.GetComponent<soldierflash>().FlashRed();
            Instantiate(laserParticles, hit.point, Quaternion.identity);
        }        
    }    
}
