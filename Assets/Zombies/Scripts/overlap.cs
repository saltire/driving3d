using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class overlap : MonoBehaviour
{
    public GameObject pistol;
    public GameObject ak47;
    public GameObject smg;
    public GameObject laserGun;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        if (colliders.Length > 54)
        {
            transform.position = new Vector2(0, 0);

            pistol.GetComponent<Player>().currentHealth = 100;
            ak47.GetComponent<Player>().currentHealth = 100;
            smg.GetComponent<Player>().currentHealth = 100;   
            laserGun.GetComponent<Player>().currentHealth = 100;

            //SceneManager.LoadScene("SampleScene");         
        }      
        
        //Debug.Log(colliders.Length + " zombies");
    }
}
