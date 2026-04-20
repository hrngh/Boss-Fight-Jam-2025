using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawn : AttackEvent
{
    public float[] angle;
    public bool evenDist;
    public float[] speed;
    public float curve;
    public float delay;
    public float size = 1;
    public bool isEvent;
    public bool relativeSpace;
    public bool fromMouth;
    public bool fromEyeL;
    public bool fromEyeR;
    public bool fromHandL;
    public bool fromHandR;
    public bool aiming;
    public bool chasePlayer;
    public bool detachPlayer;
    public Vector2 waveDat;
    public float spawnLifespan;
    public bool piercing;
    public bool chaser;
    public bool accelSpinner;
    public AudioSource audioSource;

    private float timer;
    private float accelAngle;

    public override void Start(){
        base.Start();
        timer = delay;
        if (waveDat == null) waveDat = Vector2.zero; 
    }

    public override void SpawnAttacks()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            if (SFXManager.Instance) SFXManager.Instance.PlayAttack(sound);
            if (audioSource)
            {
                audioSource.enabled = true;
            }
            timer = frequency;
            if (!accelSpinner)
            {
                for (int i = 0; i < burst; i++)
                {
                    float spawnX = position.x + Random.Range(-bounds[0] / 2, bounds[0] / 2);
                    float spawnY = position.y + Random.Range(-bounds[1] / 2, bounds[1] / 2);
                    Vector3 spawnPos = relativeSpace ? transform.position + new Vector3(spawnX, spawnY) : GetBoxSpace(spawnX, spawnY);
                    if (fromMouth) spawnPos = GameManager.Instance.mouthTransform.position;
                    if (fromEyeL) spawnPos = GameManager.Instance.eyeLTransform.position;
                    if (fromEyeR) spawnPos = GameManager.Instance.eyeRTransform.position;
                    if (fromHandL) spawnPos = GameManager.Instance.handLTransform.position;
                    if (fromHandR) spawnPos = GameManager.Instance.handRTransform.position;
                    float spawnAngle = (evenDist ? angle[0] + i * (angle[1] - angle[0]) / (burst - 1) : Random.Range(angle[0], angle[1])) + (relativeSpace ? transform.eulerAngles.z : 0);
                    if (aiming)
                    {
                        Vector3 aim = Player.Instance.transform.position - spawnPos;
                        spawnAngle = Vector2.Angle(Vector2.right, aim) * (aim.y < 0 ? -1 : 1);
                    }
                    if (!isEvent)
                    {
                        DoAttack(spawnPos, spawnAngle);
                    }
                    else
                    {
                        if (!chasePlayer)
                        {
                            GameObject attack = Instantiate(attackPrefab, spawnPos, Quaternion.Euler(0, 0, spawnAngle));
                            if (isEvent) {
                                attack.GetComponent<AttackEvent>().sound = sound;
                                attack.GetComponent<AttackEvent>().look = look;
                            } else
                            {
                                attack.GetComponent<Attack>().sound = sound;
                                attack.GetComponent<Attack>().look = look;
                            }
                        }
                        else
                        {
                            GameObject summon = Instantiate(attackPrefab, Player.Instance.transform);
                            if (isEvent)
                            {
                                summon.GetComponent<AttackEvent>().sound = sound;
                                summon.GetComponent<AttackEvent>().look = look;
                            }
                            else
                            {
                                summon.GetComponent<Attack>().sound = sound;
                                summon.GetComponent<Attack>().look = look;
                            }
                            float radSpawnAngle = spawnAngle / 180 * Mathf.PI;
                            spawnX = Mathf.Cos(radSpawnAngle) * position.x;
                            spawnY = Mathf.Sin(radSpawnAngle) * position.x;
                            summon.transform.localPosition = new Vector3(spawnX, spawnY);
                            if (detachPlayer) summon.transform.parent = null;
                            summon.transform.Rotate(Vector3.forward, spawnAngle + 180);
                        }
                    }
                }
            } else
            {
                frequency = Mathf.Clamp(frequency * .9f, .07f, 1);
                float radSpawnAngle = accelAngle / 180 * Mathf.PI;
                float spawnX = Mathf.Cos(radSpawnAngle) * 6;
                float spawnY = Mathf.Sin(radSpawnAngle) * 6;
                Vector2 spawnPos = new Vector2(0, -1.85f) + new Vector2(spawnX, spawnY);
                GameObject summon = Instantiate(attackPrefab, spawnPos, Quaternion.Euler(0, 0, accelAngle + 180));
                if (isEvent)
                {
                    summon.GetComponent<AttackEvent>().sound = sound;
                    summon.GetComponent<AttackEvent>().look = look;
                }
                else
                {
                    summon.GetComponent<Attack>().sound = sound;
                    summon.GetComponent<Attack>().look = look;
                }
                accelAngle += 25;
            }
        }
    }

    private void DoAttack(Vector2 spawnPos, float spawnAngle)
    {
        Attack attack = Instantiate(attackPrefab, spawnPos, Quaternion.Euler(0, 0, spawnAngle)).GetComponent<Attack>();
        if (isEvent)
        {
            attack.GetComponent<AttackEvent>().sound = sound;
            attack.GetComponent<AttackEvent>().look = look;
        }
        else
        {
            attack.GetComponent<Attack>().sound = sound;
            attack.GetComponent<Attack>().look = look;
        }
        attack.transform.localScale = new Vector3(size, size, size);
        float spawnSpeed = Random.Range(speed[0], speed[1]);
        spawnSpeed = Mathf.Round(spawnSpeed * 4) / 4;
        attack.speed = spawnSpeed;
        attack.curve = curve;
        attack.waveAmp = waveDat.x;
        attack.waveFreq = waveDat.y;
        attack.lifespan = spawnLifespan;
        attack.piercing = piercing;
        attack.chaser = chaser;
    }
}
