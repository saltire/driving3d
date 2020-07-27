using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public GameObject shaker;

    float damageTime = 0.25f; //How often you want to damage to be done to the player
    float currentDamageTime;

    public AudioClip hurtSFX;
    public AudioSource audioSource_hurt;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        audioSource_hurt.GetComponent<AudioSource>().clip = hurtSFX;
    }

    void Update()
    {
        healthBar.SetHealth(currentHealth);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "zombie")
        {
            currentDamageTime += Time.deltaTime;
            if(currentDamageTime > damageTime)
            {
                TakeDamage(5);
                currentDamageTime = 0.0f;
                shaker.SendMessage("TriggerShake");
                audioSource_hurt.PlayOneShot(hurtSFX);
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
