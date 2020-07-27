using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunflash : MonoBehaviour
{
    public float flashTime;
    Color origionalColor;
    public SpriteRenderer thisRenderer;

    float damageTime = 0.25f; //How often you want to damage to be done to the player
    //change to 0.25f for every quarter second/0.5f for half
    float currentDamageTime;    

    void Start()
    {
        origionalColor = thisRenderer.color;
    }

    void FlashRed()
    {
        thisRenderer.color = Color.red;
        Invoke("ResetColor", flashTime);
        //Debug.Log("Flashing red!");
    }

    void ResetColor()
    {
        thisRenderer.color = origionalColor;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "zombie") {

            currentDamageTime += Time.deltaTime;
            if(currentDamageTime > damageTime)
            {
                FlashRed();
                currentDamageTime = 0.0f;
            }    
        }
    }           
}
