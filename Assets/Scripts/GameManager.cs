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
    public HashSet<BossPartMover> allMovers;
    private Bloom m_Bloom;
    private float finalCountdown = 23.67f;
    public float bloomStrength;
    public float volume = 1;

    //stuff
    public Transform mouthTransform;
    public Transform eyeLTransform;
    public Transform eyeRTransform;
    public GameObject[] audioPips;
    public GameObject[] tutorialTexts;
    public SpriteRenderer spamZ;
    public SpriteRenderer playerSprite;
    public SpriteRenderer quitter;
    public GameObject happyBossContainer;
    public GameObject happyBossContainer2;
    public GameObject happyBossContainer3;
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
        if (Instance) Destroy(Instance);
        Instance = this;
        Time.timeScale = 1;
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
        volume = 1;
    }

    public void addMover(BossPartMover b)
    {
        if (allMovers == null) allMovers = new HashSet<BossPartMover>();
        allMovers.Add(b);
    }

    private void StartUp()
    {
        timer = 0;
        if(!testing) audioSource.Play();
    }

    private void getFucked()
    {
        Application.Quit();
        Time.timeScale = 0;
        Instantiate(screenFlashPrefab);
    }

    void Update()
    {
        //audio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            volume -= 0.25f;
            if (volume == -0.25f)
            {
                volume = 1;
                audioPips[0].SetActive(true);
                audioPips[1].SetActive(true);
                audioPips[2].SetActive(true);
                audioPips[3].SetActive(false);
            }
            else
            {
                if (volume != 0)
                {
                    audioPips[(int)(volume * 4) - 1].SetActive(false);
                } else
                {
                    audioPips[3].SetActive(true);
                }
            }
            audioSource.volume = volume;
            Player.Instance.sfxSource.volume = volume;
            Player.Instance.sfxSource2.volume = volume;
            Player.Instance.channelSource.volume = volume * .4f;
        }

        //quitting
        if (Input.GetKey(KeyCode.Escape))
        {
            quitter.color = new Color(1, 1, 1, quitter.color.a + Time.unscaledDeltaTime / 2);
            if (quitter.color.a >= 1)
            {
                ClearStuff();
                happyBossContainer.SetActive(false);
                mainBossContainer.SetActive(false);
                quitter.color = new Color(1,1,1,-1);
                if (Random.Range(0f,1f) <= .5f)
                {
                    audioSource.Stop();
                    audioSource.pitch = 0.4f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = 0.5f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = 0.6f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    happyBossContainer2.SetActive(true);
                    Invoke("getFucked", .2f);
                } else {
                    Application.Quit();
                }
            }
        }
        else
        {
            if(quitter.color.a > 0) quitter.color = new Color(1, 1, 1, quitter.color.a - Time.unscaledDeltaTime * 2);
        }
        if (state != State.dead && state != State.transition) eyeStrengthMult = (1 - quitter.color.a);

        if (Input.GetKeyDown(KeyCode.T)) PlayerPrefs.SetInt("skipTutorial", 0);
        if (Input.GetKeyDown(KeyCode.H)) PlayerPrefs.SetInt("hard", 0);

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
                audioSource.volume = Mathf.Clamp(audioSource.volume + Time.deltaTime, 0, volume);
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
                    foreach (BossPartMover b in allMovers)
                    {
                        b.transform.localScale = Vector3.one * shrinkAmount;
                    }
                }
                if (timer <= .2f && attackIndex == 0)
                {
                    attackIndex = 1;
                    audioSource.pitch = .4f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = .45f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = .5f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = .55f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    audioSource.pitch = .6f;
                    audioSource.PlayOneShot(phaseChangeSound);
                    if (PlayerPrefs.GetInt("hard", 0) == 0)
                    {
                        happyBossContainer2.SetActive(true);
                    } else
                    {
                        happyBossContainer3.SetActive(true);
                    }
                    PlayerPrefs.SetInt("hard", 1);
                }
                if (timer <= 0)
                {
                    timer = 1000;
                    Instantiate(screenFlashPrefab);
                    Time.timeScale = 0;
                    Application.Quit();
                }
                break;
            case State.dead:
                eyeStrengthMult = Mathf.Clamp01(eyeStrengthMult - Time.unscaledDeltaTime / 2);
                if(playerSprite.color.a > 0) playerSprite.color = new Color(1, 1, 1, playerSprite.color.a - Time.unscaledDeltaTime * 2);
                timer -= Time.unscaledDeltaTime;
                if (timer <= 1.5f && attackIndex == 0)
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
                eyeStrengthMult = Mathf.Clamp(eyeStrengthMult + Time.deltaTime * 4, -10, 1);
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
        attackIndex = 0;
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
        foreach (BossPartMover b in allMovers)
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
        foreach (BossPartMover b in allMovers)
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
            eyeStrengthMult = -6f;
            Instantiate(screenFlashPrefab);
            tutorialTexts[0].SetActive(false);
            tutorialTexts[1].SetActive(false);
            tutorialTexts[3].SetActive(false);
            if (tutorialTexts[2]) tutorialTexts[2].SetActive(false);
            happyBossContainer.SetActive(false);
            mainBossContainer.SetActive(true);
        } else
        {
            PlayerPrefs.SetInt("skipTutorial", 1);
        }
        if (i == 2) Player.Instance.Heal(3);
        if (i == 3) Player.Instance.Heal(5);
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