using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float speed;
    public float range;
    public float stoppingDistance;

    public RuntimeAnimatorController run;
    public RuntimeAnimatorController idle;

    private Transform target;
    private SpriteRenderer mySpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Car").GetComponent<Transform>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        speed = Random.Range(1, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {       
        if (Vector2.Distance(transform.position, target.position) < range) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            Animator animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = run as RuntimeAnimatorController;
        }
        else {
            Animator animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = idle as RuntimeAnimatorController;
        }
        mySpriteRenderer.flipX = target.position.x < this.transform.position.x;       
    }    
}
