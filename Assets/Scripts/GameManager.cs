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
        end,
        transition
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
    public Transform mouthTransform;
    public Transform eyeLTransform;
    public Transform eyeRTransform;
    public GameObject happyBossContainer;
    public GameObject screenFlashPrefab;
    public GameObject mainBossContainer;
    public PostProcessVolume m_Volume;
    public GameObject clockHolder;
    public GameObject[] leftClocks;
    public GameObject[] rightClocks;
    public AudioSource audioSource;
    public AudioClip[] tracks;
    public AudioClip phaseChangeSound;
    public GameObject[] phase1Attacks;
    public GameObject[] phase2Attacks;
    public GameObject[] phase3Attacks;


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

    void Update()
    {
        if (testing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                testing = false;
                StartPhaseChange(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                testing = false;
                StartPhaseChange(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                testing = false;
                StartPhaseChange(3);
            }
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                DoClock(0);
                timer += 4 / 3f;
            }
            inPhase = timer <= 2 / 3f;
            m_Bloom.intensity.value = timer <= 2 / 3f ? bloomStrength : 1f;
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
                    //todo fix timing
                    timer += 1;
                }
                break;
            case State.phase1:
                timer -= Time.deltaTime;
                inPhase = timer <= 2 / 3f;
                if (timer <= 0)
                {
                    DoClock(0);
                    timer += 4/3f;
                }
                break;
            case State.phase2:
                timer -= Time.deltaTime;
                inPhase = timer <= 2 / 3f;
                if (timer <= 0)
                {
                    DoClock(1);
                    timer += 2;
                }
                break;
            case State.transition:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    DoClock(newPhase-1);
                    timer += (1 + newPhase) * 2 / 3f;
                }
                break;
        }
        if (inPhase)
        {
            m_Bloom.intensity.value = timer <= 2/3f ? bloomStrength : 1f;
        }
    }

    int attackIndex;
    public void ChangeAttack()
    {
        if (attackIndex % 2 == 0) OrderBoss();
        attackIndex++;
        ClearStuff(false);
        GameObject[] active = newPhase switch
        {
            1 => phase1Attacks,
            2 => phase2Attacks,
            3 => phase3Attacks,
            _ => phase1Attacks // The underscore '_' represents the default case
        };
        Instantiate(active[attackIndex]);
    }
    int hurtCounter;
    public void HurtBoss()
    {
        foreach (BossPartMover b in BossPartMover.allMovers)
        {
            b.Chaos();
        }
        hurtCounter++;
        if (hurtCounter == 7)
        {
            hurtCounter = 0;
            StartPhaseChange(newPhase + 1);
            Invoke("OrderBoss", 3f);
        }
    }
    private void OrderBoss()
    {
        foreach (BossPartMover b in BossPartMover.allMovers)
        {
            b.Order();
        }
    }

    private int newPhase;
    void StartPhaseChange(int i)
    {
        ClearStuff();
        Player.Instance.Heal(5);
        audioSource.Stop();
        audioSource.PlayOneShot(phaseChangeSound);
        newPhase = i;
        state = State.transition;
        timer = 4 / 3f;
        Invoke("PhaseChange", 4f);
    }
    void PhaseChange()
    {
        audioSource.clip = tracks[newPhase];
        audioSource.Play();
        timer = 0;
        attackIndex = -1;
        ChangeAttack();
        state = newPhase switch
        {
            1 => State.phase1,
            2 => State.phase2,
            3 => State.phase3,
            _ => State.end // The underscore '_' represents the default case
        };
    }
    void ClearStuff(bool withClock = true)
    {
        //attacks, events, clocks
        foreach(Attack a in attacks)
        {
            Destroy(a.gameObject);
        }
        foreach (AttackEvent e in events)
        {
            Destroy(e.gameObject);
        }
        if (withClock)
        {
            foreach (ClockMover c in clocks)
            {
                c.Kill();
            }
        }
    }

    void DoClock(int i)
    {
        Instantiate(rightClocks[i], clockHolder.transform).transform.Translate(Vector3.right * -timer * 1.5f);
        Instantiate(leftClocks[i], clockHolder.transform).transform.Translate(Vector3.left * -timer * 1.5f);
    }
}