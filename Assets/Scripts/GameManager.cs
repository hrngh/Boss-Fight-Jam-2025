using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //things
    public enum State
    {
        tutorial,
        phase1,
        phase2,
        phase3,
        phase4,
        phase5,
        end,
        transition,
        dead
    }
    public State state;
    public bool inPhase;
    private float timer;
    public HashSet<ClockMover> clocks;
    public HashSet<AttackEvent> events;
    public HashSet<Attack> attacks;
    private Bloom m_Bloom;
    private float finalCountdown = 23.67f;
    public float bloomStrength;

    //stuff
    public Transform mouthTransform;
    public Transform eyeLTransform;
    public Transform eyeRTransform;
    public GameObject[] tutorialTexts;
    public SpriteRenderer spamZ;
    public SpriteRenderer playerSprite;
    public GameObject happyBossContainer;
    public GameObject happyBossContainer2;
    public GameObject screenFlashPrefab;
    public GameObject mainBossContainer;
    public Animator mouthAnim;
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
    public GameObject[] phase4Attacks;


    //testing
    public bool testing;

    void Start()
    {
        Time.timeScale = 1;
        Instance = this;
        clocks = new HashSet<ClockMover>();
        events = new HashSet<AttackEvent>();
        attacks = new HashSet<Attack>();
        happyBossContainer.SetActive(true);
        mainBossContainer.SetActive(false);
        m_Volume.profile.TryGetSettings<Bloom>(out m_Bloom);
        audioSource.volume = 0;
        timer = 20;
        Invoke("StartUp", 1);
        if(testing || PlayerPrefs.GetInt("skipTutorial", 0) == 1)
        {
            foreach (GameObject g in tutorialTexts)
            {
                g.SetActive(false);
            }
        }
        if (testing)
        {
            happyBossContainer.SetActive(false);
            mainBossContainer.SetActive(true);
        }
    }

    private void StartUp()
    {
        timer = 0;
        if(!testing) audioSource.Play();
    }

    void Update()
    {
        if (testing)
        {
            audioSource.volume = 1;
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
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                testing = false;
                StartPhaseChange(4);
            }
            if (Input.GetKeyDown(KeyCode.P)) PlayerPrefs.SetInt("skipTutorial", 0);
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
            case State.tutorial:
                timer -= Time.deltaTime;
                audioSource.volume = Mathf.Clamp01(audioSource.volume + Time.deltaTime);
                inPhase = timer <= 8 / 3f;
                if(tutorialTexts[1].activeInHierarchy) tutorialTexts[0].SetActive(!inPhase);
                if (timer <= 0)
                {
                    timer += 16/3f;
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
            case State.phase3:
                timer -= Time.deltaTime;
                inPhase = timer <= 2 / 3f;
                if (timer <= 0)
                {
                    DoClock(2);
                    timer += 8/3f;
                }
                break;
            case State.phase4:
                timer -= Time.deltaTime;
                finalCountdown -= Time.deltaTime;
                if (timer <= 0)
                {
                    DoClock(3);
                    timer += 4 / 3f;
                }
                if (finalCountdown <= 0)
                {
                    state = State.phase5;
                    timer = 4 / 3f;
                    DoClock(4);
                    mouthAnim.Play("frown");
                    audioSource.Stop();
                    audioSource.clip = tracks[5];
                    audioSource.PlayDelayed(1.5f);
                }
                break;
            case State.phase5:
                timer -= Time.deltaTime;
                inPhase = audioSource.isPlaying;
                if (timer <= 0)
                {
                    spamZ.gameObject.SetActive(true);
                    DoClock(4);
                    timer += .2f;
                }
                break;
            case State.end:
                timer -= Time.deltaTime;
                float shrinkAmount = Mathf.Clamp01((timer-4.5f)*2);
                if (timer >= 4) {
                    foreach (BossPartMover b in BossPartMover.allMovers)
                    {
                        b.transform.localScale = Vector3.one * shrinkAmount;
                    }
                }
                if (timer <= .1f)
                {
                    happyBossContainer2.SetActive(true);
                }
                if (timer <= 0)
                {
                    timer = 1000;
                    Instantiate(screenFlashPrefab);
                    Application.Quit();
                }
                break;
            case State.dead:
                eyeStrengthMult = Mathf.Clamp01(eyeStrengthMult - Time.unscaledDeltaTime / 2);
                if(playerSprite.color.a > 0) playerSprite.color = new Color(1, 1, 1, playerSprite.color.a - Time.unscaledDeltaTime * 2);
                timer -= Time.unscaledDeltaTime;
                if (timer <= 2f && attackIndex == 0)
                {
                    attackIndex = 1;
                    Instantiate(screenFlashPrefab).GetComponent<Attack>().lifespan = 2f;
                }
                if (timer <= 0)
                {
                    SceneManager.LoadScene("MainScene");
                }
                break;
            case State.transition:
                timer -= Time.deltaTime;
                inPhase = false;
                if (timer <= 0)
                {
                    DoClock(newPhase-1);
                    timer += (1 + (newPhase == 4 ? 1 : newPhase)) * 2 / 3f;
                }
                break;
        }
        if (inPhase)
        {
            m_Bloom.intensity.value = bloomStrength;
            Player.Instance.iFrameTimer = .25f;
        } else
        {
            m_Bloom.intensity.value = 1;
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
            4 => phase4Attacks,
            _ => phase1Attacks 
        };
        Instantiate(active[attackIndex]);
    }

    public void DoGameEnd()
    {
        state = State.end;
        audioSource.Stop();
        audioSource.PlayOneShot(phaseChangeSound);
        inPhase = false;
        ClearStuff();
        timer = 5f;
    }

    public float eyeStrengthMult = 1;
    public void DoDeath()
    {
        audioSource.Stop();
        audioSource.pitch = .5f;
        audioSource.PlayOneShot(phaseChangeSound);
        Time.timeScale = 0;
        state = State.dead;
        attackIndex = 0;
        timer = 4f;
    }

    int hurtCounter;
    public void HurtBoss()
    {
        foreach (BossPartMover b in BossPartMover.allMovers)
        {
            b.Chaos();
        }
        hurtCounter++;
        if (hurtCounter == 40) DoGameEnd();
        if (state == State.phase5)
        {
            spamZ.color = new Color(1, 1, 1, spamZ.color.a - .1f);
            return;
        }
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
        audioSource.Stop();
        audioSource.PlayOneShot(phaseChangeSound);
        ClearStuff();
        if (i == 1)
        {
            Instantiate(screenFlashPrefab);
            tutorialTexts[0].SetActive(false);
            tutorialTexts[1].SetActive(false);
            if(tutorialTexts[2]) tutorialTexts[2].SetActive(false);
            happyBossContainer.SetActive(false);
            mainBossContainer.SetActive(true);
        } else
        {
            PlayerPrefs.SetInt("skipTutorial", 1);
        }
        Player.Instance.Heal(5);
        newPhase = i;
        state = State.transition;
        timer = newPhase == 2 ? 0 : 4 / 3f;
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
            4 => State.phase4,
            _ => State.end
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