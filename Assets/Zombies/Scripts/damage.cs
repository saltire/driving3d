using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    public float health = 90;
    public GameObject blood;

    public AudioClip zombieGrowlSFX;
    public AudioSource audioSource_growl;

    private bool hasPlayed = false;

    public BoxCollider2D boxCol;
    public BoxCollider2D boxCol2;    

    // Start is called before the first frame update
    void Start()
    {
        audioSource_growl.GetComponent<AudioSource>().clip = zombieGrowlSFX;
    }

    void Damage(float dmg) 
    {
        health -= dmg;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0) 
        {
            if(!hasPlayed)
            {
                audioSource_growl.PlayOneShot(zombieGrowlSFX);
                hasPlayed = true;

                Instantiate(blood, transform.position, Quaternion.identity);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<FollowPlayer>().speed = 0f;
                boxCol.enabled = false;
                boxCol2.enabled = false;
                StartCoroutine(WaitForSFX());
            }
        }
    }    

    IEnumerator WaitForSFX()
    {
        yield return new WaitForSeconds(0.888f);
        Destroy(gameObject);
    }
}
