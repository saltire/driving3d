using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject gun;
    public GameObject bullet;
    public GameObject bulletEmitter;

    public GameObject laser;
    public GameObject forcefield;

    public GameObject shaker;

    public GameObject mainCamera;

    public string horizontalAxis = "RHorizontal";
    public string verticalAxis = "RVertical";

    public float fireRate = 20f;
    private float lastfired;                

    public bool canShoot = true;
    public bool canShootLaser = true;

    public AudioClip bulletSFX;
    public AudioSource audioSource_bullet;

    void ResetShot()
    {
        canShoot = true;
    }  

    void Start()
    {
        audioSource_bullet.GetComponent<AudioSource>().clip = bulletSFX;
    }

    // Update is called once per frame
    void Update()
    {
        float primaryAttack = Input.GetAxis ("PrimaryAttack");
        bool secondaryAttack = Input.GetButton ("SecondaryAttack");
        
        Vector2 shootDirection = Vector2.right*Input.GetAxis(horizontalAxis) + Vector2.up*Input.GetAxis(verticalAxis);
        transform.rotation = Quaternion.FromToRotation(shootDirection, Vector2.right);
        //Debug.Log(transform.rotation);

        if (transform.rotation.z > 0.7f) {
            transform.localScale = new Vector3(1,-1,1); 
        }
        else {
            transform.localScale = new Vector3(1,1,1);
        }
        
        if (primaryAttack > 0.01f && canShoot == true) {
            if (Time.time - lastfired > 1 / fireRate)
            {
                lastfired = Time.time;
                Instantiate(bullet, bulletEmitter.transform.position, transform.rotation);
                shaker.SendMessage("TriggerShake");
                audioSource_bullet.PlayOneShot(bulletSFX);

                mainCamera.GetComponent<BackgroundColorLerp>().LerpBackgroundColor();
            }
        }

        if (secondaryAttack == true)
        {
            if (canShootLaser)
            {        
                laser.SetActive(true);
                shaker.SendMessage("TriggerShake");
            }    
        }        
        else 
        {
            laser.SetActive(false);
        }     
                     
        mainCamera.GetComponent<BackgroundColorLerp>().LerpBackgroundColorBack();
    }    
}
