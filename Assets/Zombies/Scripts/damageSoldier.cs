using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageSoldier : MonoBehaviour
{
    public float health = 90;
    public GameObject blood;

    public AudioClip soldierScreamSFX;
    public AudioSource audioSource_scream;

    private bool hasPlayed = false;

    public BoxCollider2D boxCol;
    public BoxCollider2D boxCol2;   

    public GameObject zombie; 

    // Start is called before the first frame update
    void Start()
    {
        audioSource_scream.GetComponent<AudioSource>().clip = soldierScreamSFX;
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
                audioSource_scream.PlayOneShot(soldierScreamSFX);
                hasPlayed = true;

                Instantiate(blood, transform.position, Quaternion.identity);
                Instantiate(zombie, transform.position, Quaternion.identity);
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
