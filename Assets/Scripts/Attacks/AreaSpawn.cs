using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawn : AttackEvent
{
    public float[] angle;
    public bool evenDist;
    public float[] speed;
    public float delay;
    public float size = 1;
    public bool isEvent;

    private float timer;

    public override void Start(){
        base.Start();
        timer = delay;
    }

    public override void SpawnAttacks()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = frequency;
            for (int i=0; i<burst; i++)
            {
                float spawnX = position.x + Random.Range(-bounds[0] / 2, bounds[0] / 2);
                float spawnY = position.y + Random.Range(-bounds[1] / 2, bounds[1] / 2);
                float spawnAngle = evenDist ? angle[0] + i * (angle[1] - angle[0]) / (burst - 1) : Random.Range(angle[0], angle[1]);
                if (!isEvent) {
                    DoAttack(spawnX, spawnY, spawnAngle);
                } else
                {
                    Instantiate(attackPrefab, GetBoxSpace(spawnX, spawnY), Quaternion.Euler(0, 0, spawnAngle));
                }
            }
        }
    }

    private void DoAttack(float spawnX, float spawnY, float spawnAngle)
    {
        Attack attack = Instantiate(attackPrefab, GetBoxSpace(spawnX, spawnY), Quaternion.Euler(0, 0, spawnAngle)).GetComponent<Attack>();
        attack.transform.localScale = new Vector3(size, size, size);
        float spawnSpeed = Random.Range(speed[0], speed[1]);
        spawnSpeed = Mathf.Round(spawnSpeed * 4) / 4;
        attack.speed = spawnSpeed;
    }
}
