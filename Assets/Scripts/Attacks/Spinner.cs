using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : AttackEvent
{
    public float speed;
    public AnimationCurve radius;
    public float period;
    public float delay;
    public float size = 1;

    public GameObject holderPrefab;

    private float timer;
    private bool spawned;
    private GameObject[] holders;
    private GameObject[] projectiles;
    public override void Start()
    {
        base.Start();
        timer = -delay;
        holders = new GameObject[(int)burst];
        projectiles = new GameObject[(int)burst];
    }

    public override void SpawnAttacks()
    {
        timer += Time.deltaTime;
        if (timer >= 0 && !spawned)
        {
            spawned = true;
            for (int i = 0; i < burst; i++)
            {
                float spawnAngle = i * 360 / burst;
                holders[i] = Instantiate(holderPrefab, position, Quaternion.Euler(0, 0, spawnAngle));
                projectiles[i] = Instantiate(attackPrefab, holders[i].transform);
                projectiles[i].transform.localScale = Vector3.one * size;
                projectiles[i].GetComponent<Attack>().piercing = true;
                projectiles[i].GetComponent<Attack>().sound = sound;
                projectiles[i].GetComponent<Attack>().look = look;
            }
        }
        if(spawned)
        {
            foreach (GameObject h in holders)
            {
                h.transform.Rotate(Vector3.forward, Time.deltaTime * speed);
            }
            float evalPoint = timer%period / period;
            foreach (GameObject p in projectiles)
            {
                p.transform.localPosition = Vector3.right * radius.Evaluate(evalPoint);
            }
        }
    }
}
