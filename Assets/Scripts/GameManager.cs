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
        waiting,
        attacking,
        dead,
        resetting
    }
    public State state;
    private float timer;
    public HashSet<AttackEvent> events;
    public HashSet<Attack> attacks;
    public HashSet<BossPartMover> allMovers;
    private Bloom m_Bloom;
    public float bloomStrength;
    public GameObject testAttack;

    //stuff
    public Transform mouthTransform;
    public Transform eyeLTransform;
    public Transform eyeRTransform;
    public Transform handLTransform;
    public Transform handRTransform;
    public SpriteRenderer playerSprite;
    public SpriteRenderer quitter;
    public GameObject screenFlashPrefab;
    public PostProcessVolume m_Volume;
    public AudioSource audioSource;
    public GameObject[] attackMoves;
    private List<GameObject>[] attackFreqs = new List<GameObject>[3];

    void Start()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
        Time.timeScale = 1;
        events = new HashSet<AttackEvent>();
        attacks = new HashSet<Attack>();
        m_Volume.profile.TryGetSettings<Bloom>(out m_Bloom);
        audioSource.volume = 0;
        attackFreqs[0] = new List<GameObject>(attackMoves);
        for (int i=1; i<attackFreqs.Length; i++)
        {
            attackFreqs[i] = new List<GameObject>();
        }
        timer = 20;
        Invoke("StartUp", 1);
    }

    public void addMover(BossPartMover b)
    {
        if (allMovers == null) allMovers = new HashSet<BossPartMover>();
        allMovers.Add(b);
    }

    private void StartUp()
    {
        timer = 3;
        state = State.waiting;
        audioSource.Play();
    }

    void Update()
    {

        //quitting
        if (Input.GetKey(KeyCode.Escape))
        {
            quitter.color = new Color(1, 1, 1, quitter.color.a + Time.unscaledDeltaTime / 2);
            if (quitter.color.a >= 1)
            {
                ClearStuff();
                quitter.color = new Color(1, 1, 1, -1);
                Application.Quit();
            }
        }
        else
        {
            if (quitter.color.a > 0) quitter.color = new Color(1, 1, 1, quitter.color.a - Time.unscaledDeltaTime * 2);
        }
        if (state != State.dead) eyeStrengthMult = (1 - quitter.color.a);

        switch (state)
        {
            case State.waiting:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    ChangeAttack();
                    state = State.attacking;
                }
                break;
            case State.attacking:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    ClearStuff();
                    state = State.waiting;
                    timer = 1;
                }
                break;
            case State.dead:
                eyeStrengthMult = Mathf.Clamp01(eyeStrengthMult - Time.unscaledDeltaTime / 2);
                if (playerSprite.color.a > 0) playerSprite.color = new Color(1, 1, 1, playerSprite.color.a - Time.unscaledDeltaTime * 2);
                //TODO fade in score text/prompt to continue
                break;
            case State.resetting:
                eyeStrengthMult = Mathf.Clamp01(eyeStrengthMult - Time.unscaledDeltaTime / 2);
                if (playerSprite.color.a > 0) playerSprite.color = new Color(1, 1, 1, playerSprite.color.a - Time.unscaledDeltaTime * 2);
                timer -= Time.unscaledDeltaTime;
                if (timer <= 1.5f && timer > 0)
                {
                    timer -= 1.5f;
                    Instantiate(screenFlashPrefab).GetComponent<Attack>().lifespan = 2f;
                }
                if (timer <= -1.5)
                {
                    SceneManager.LoadScene("MainScene");
                }
                break;
        }
        //TODO Use for on hit? m_Bloom.intensity.value = bloomStrength;
        m_Bloom.intensity.value = Mathf.Clamp(m_Bloom.intensity.value-(bloomStrength-1)*Time.deltaTime*2, 1, bloomStrength);
    }
    
    public void ChangeAttack()
    {
        // Override for testing
        if (testAttack)
        {
            Instantiate(testAttack);
            timer = testAttack.GetComponent<AttackDuration>().duration;
            return;
        }

        // Reset when needed
        if (attackFreqs[0].Count == 0)
        {
            for (int i = 0; i < attackFreqs.Length-1; i++)
            {
                attackFreqs[i] = attackFreqs[i+1];
            }
            attackFreqs[attackFreqs.Length - 1] = new List<GameObject>();
        }
        // Pick an attack
        int activei = Random.Range(0, attackFreqs.Length - 1);
        while(attackFreqs[activei].Count == 0)
        {
            activei = Random.Range(0, attackFreqs.Length - 1);
        }
        List<GameObject> active = attackFreqs[activei];
        int attackIndex = Random.Range(0, active.Count);
        GameObject attack = active[attackIndex];
        active.RemoveAt(attackIndex);
        attackFreqs[activei + 1].Add(attack);
        Instantiate(attack);
        timer = attack.GetComponent<AttackDuration>().duration;
    }

    public float eyeStrengthMult = 1;
    public void DoDeath()
    {
        audioSource.Stop();
        audioSource.pitch = .5f;
        Time.timeScale = 0;
        state = State.dead;
        timer = 4f;
    }

    public void HurtBoss()
    {
        foreach (BossPartMover b in allMovers)
        {
            b.Chaos();
        }
        //TODO update score + score text
    }

    void ClearStuff()
    {
        //attacks, events, clocks
        foreach (Attack a in attacks)
        {
            Destroy(a.gameObject);
        }
        foreach (AttackEvent e in events)
        {
            Destroy(e.gameObject);
        }
    }
}