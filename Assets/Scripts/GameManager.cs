using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //things
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
    public bool inPhase;
    public float startTimer;
    private float timer;
    public HashSet<ClockMover> clocks;
    public HashSet<AttackEvent> events;
    public HashSet<Attack> attacks;
    private Bloom m_Bloom;
    public float bloomStrength;

    //stuff
    public GameObject happyBossContainer;
    public GameObject screenFlashPrefab;
    public GameObject mainBossContainer;
    public GameObject projectilePrefab;
    public PostProcessVolume m_Volume;


    //testing
    public bool testing;

    void Start()
    {
        Instance = this;
        clocks = new HashSet<ClockMover>();
        events = new HashSet<AttackEvent>();
        attacks = new HashSet<Attack>();
        m_Volume.profile.TryGetSettings<Bloom>(out m_Bloom);
    }

    // Update is called once per frame
    void Update()
    {
        if (testing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) timer += 4/3f;
            float bloomPulse = (timer * 8 % 8 / 8 * bloomStrength/2) - bloomStrength/4;
            inPhase = timer <= 2 / 3f;
            m_Bloom.intensity.value = timer <= 2/3f ? bloomStrength + bloomPulse : 1f;
            return;
        }
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
                }
                break;
        }
    }
}