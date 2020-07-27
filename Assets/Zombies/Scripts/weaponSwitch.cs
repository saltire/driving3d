using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSwitch : MonoBehaviour
{
    public int currentWeapon;
    public Transform[] weapons;         

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire4")) 
        {
            changeWeapon(3);
        }

        if(Input.GetButtonDown("Fire1")) 
        {
            changeWeapon(2);
        }

        if(Input.GetButtonDown("Fire2")) 
        {
            changeWeapon(1);
        }

        if(Input.GetButtonDown("Fire3")) 
        {
            changeWeapon(0);
        }        
    }

    public void changeWeapon(int num) {
        currentWeapon = num;
        for(int i = 0; i < weapons.Length; i++) 
        {
            if(i == num) {
                weapons[i].gameObject.GetComponent<SpriteRenderer>().enabled = true;
                weapons[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                weapons[i].gameObject.GetComponent<BoxCollider2D>().enabled = true;
                weapons[i].gameObject.GetComponent<PlayerShoot>().enabled = true;
                weapons[i].gameObject.GetComponent<Grenade>().enabled = true;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = true;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = true;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = true;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = true;                
            }    
            else {
                weapons[i].gameObject.GetComponent<SpriteRenderer>().enabled = false;
                weapons[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                weapons[i].gameObject.GetComponent<BoxCollider2D>().enabled = false;
                weapons[i].gameObject.GetComponent<PlayerShoot>().enabled = false;
                weapons[i].gameObject.GetComponent<Grenade>().enabled = false;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<laser>().enabled = false;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<laser>().enabled = false; 
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserRight").gameObject.GetComponent<LineRenderer>().enabled = false;
                weapons[i].gameObject.transform.GetChild(2).transform.Find("laserLeft").gameObject.GetComponent<LineRenderer>().enabled = false;                               
            }    
        }
    }    
}
