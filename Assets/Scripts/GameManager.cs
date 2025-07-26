using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //things
    private static float BOX_Y = -1.85f;
    public enum State
    {
        intro,
        tutorial,
        phase1,
        phase2,
        phase3,
        phase4,
        end
    }
    public State state;
    public float startTimer;
    private float timer;

    //stuff
    public GameObject happyBossContainer;
    public GameObject screenFlashPrefab;
    public GameObject mainBossContainer;
    public GameObject projectilePrefab;

    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.intro:
                startTimer -= Time.deltaTime;
                if (startTimer <= 0) state = State.tutorial;
                break;
            case State.tutorial:
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    timer = 1;
                    SpawnProjectile(0,.8f);
                }
                break;
        }
    }

    void SpawnProjectile(float x, float y, float z = 0)
    {
        Instantiate(projectilePrefab, new Vector3(x * 16 - 8, BOX_Y + y * 10 - 5), Quaternion.Euler(0,0,z));
    }
}