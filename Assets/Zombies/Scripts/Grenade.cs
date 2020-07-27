using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Rigidbody2D projectile;

    public Transform explosionEmitter1;
    public Transform explosionEmitter2;
    public Transform explosionEmitter3;
    public Transform explosionEmitter4;

    public float fireRate = 1f;            // The number of bullets fired per second
    private float lastfired;                // The value of Time.time at the last firing moment
    public AudioClip grenadeClip;

    Coroutine reloadCoroutine;
    public GameObject shaker;
    public GameObject forcefield;

    public AudioSource audio2;

    void Update ()
    {
        bool grenadeyAttack = Input.GetButton ("GrenadeAttack");

        if (Input.GetButton("GrenadeAttack"))
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(Reload());

                lastfired = Time.time;
                Instantiate(projectile, explosionEmitter1.transform.position, explosionEmitter1.transform.rotation);
                Instantiate(projectile, explosionEmitter2.transform.position, explosionEmitter2.transform.rotation);
                Instantiate(projectile, explosionEmitter3.transform.position, explosionEmitter3.transform.rotation);
                Instantiate(projectile, explosionEmitter4.transform.position, explosionEmitter4.transform.rotation);

                audio2.clip = grenadeClip;
                audio2.Play();

                shaker.SendMessage("TriggerShake");
                forcefield.SetActive(true);
                forcefield.GetComponent<CircleCollider2D>().enabled = false;
            }
        }
    }

    IEnumerator Reload() 
    {
        yield return new WaitForSeconds(5f);
        forcefield.SetActive(false);
        forcefield.GetComponent<CircleCollider2D>().enabled = true;
        reloadCoroutine = null;
    }
}
