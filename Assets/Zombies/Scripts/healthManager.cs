using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class healthManager : MonoBehaviour
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
        if (pistol.GetComponent<Player>().currentHealth <= 0) {
            pistol.GetComponent<SpriteRenderer>().enabled = false;
            pistol.GetComponent<BoxCollider2D>().enabled = false;
            pistol.transform.GetChild(0).gameObject.SetActive(false);
            pistol.GetComponent<PlayerShoot>().enabled = false;
            pistol.GetComponent<Grenade>().enabled = false;
            pistol.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = false;
            pistol.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = false;
            pistol.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = false;
            pistol.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = false;            
        }   

        if (ak47.GetComponent<Player>().currentHealth <= 0) {
            ak47.GetComponent<SpriteRenderer>().enabled = false;
            ak47.GetComponent<BoxCollider2D>().enabled = false;
            ak47.transform.GetChild(0).gameObject.SetActive(false);
            ak47.GetComponent<PlayerShoot>().enabled = false;
            ak47.GetComponent<Grenade>().enabled = false;
            ak47.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = false;
            ak47.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = false;
            ak47.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = false;
            ak47.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = false;                         
        }

        if (smg.GetComponent<Player>().currentHealth <= 0) {
            smg.GetComponent<SpriteRenderer>().enabled = false;
            smg.GetComponent<BoxCollider2D>().enabled = false;
            smg.transform.GetChild(0).gameObject.SetActive(false);
            smg.GetComponent<PlayerShoot>().enabled = false;
            smg.GetComponent<Grenade>().enabled = false;
            smg.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = false;
            smg.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = false;    
            smg.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = false;
            smg.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = false;                      
        }    

        if (laserGun.GetComponent<Player>().currentHealth <= 0) {
            laserGun.GetComponent<SpriteRenderer>().enabled = false;
            laserGun.GetComponent<BoxCollider2D>().enabled = false;
            laserGun.transform.GetChild(0).gameObject.SetActive(false);
            laserGun.GetComponent<PlayerShoot>().enabled = false;
            laserGun.GetComponent<Grenade>().enabled = false;
            laserGun.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = false;
            laserGun.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = false;    
            laserGun.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = false;
            laserGun.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = false;                      
        }         
    }
}
