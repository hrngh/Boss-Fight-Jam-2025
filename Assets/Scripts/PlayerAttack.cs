using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float speed;
    public float lifespan;
    public float targetRat;
    public Animator anim;

    private bool dead;
    private Vector2 vel;
    private float targetY;
    void Start()
    {
        float angle = Random.Range(-.5f,Mathf.PI+.5f);
        vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        targetY = Random.Range(1.5f,4f);
    }

    // Update is called once per frame
    void Update()
    {
        lifespan -= Time.deltaTime;
        if (Time.timeScale == 0) Destroy(gameObject);
        if (dead)
        {
            if (lifespan <= 0) Destroy(gameObject);
            return;
        }
        if(transform.position.y > targetY)
        {
            lifespan = 0.5f;
            anim.Play("attackExplode");
            GameManager.Instance.AddScore(50, transform.position);
            GameManager.Instance.HurtBoss();
            dead = true;
        }
        Vector2 target = new Vector3(0,2.5f) - transform.position;
        vel = (vel * (1-targetRat) + target * targetRat).normalized * speed;
        transform.Translate(vel * Time.deltaTime);
    }
}
