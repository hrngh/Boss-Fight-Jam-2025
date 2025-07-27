using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool testing;

    public static Player Instance;
    public float speed;
    public float attackSlowRatio;
    public float attackTime;
    public float rotateSpeed;
    public float attackRotateRatio;
    public int maxHealth;

    public Rigidbody2D rb;
    public CircleCollider2D hitbox;
    public GameObject capture;
    public GameObject circlerContainer;
    public GameObject[] circlers;
    public GameObject attackPrefab;
    public GameObject healthbar;
    public SpriteRenderer chargeTutorial;
    public Sprite[] numbers;
    public GameObject zButton;
    public SpriteRenderer[] wasd;
    public AudioSource sfxSource;
    public AudioClip shootSound;
    public AudioSource sfxSource2;
    public AudioClip hurtSound;

    private int health;
    private int captureCount;
    private int circlerCount;
    private float captureDelay;
    private float attackTimer;
    private bool doneWASD;

    void Start()
    {
        health = maxHealth;
        Instance = this;
        if (!testing)
        {
            foreach (GameObject c in circlers)
            {
                c.SetActive(true);
            }
            circlerCount = 6;
            if (PlayerPrefs.GetInt("skipTutorial", 0) == 1)
            {
                doneWASD = true;
                doneTutorial = true;
                foreach (SpriteRenderer s in wasd)
                {
                    s.enabled = false;
                }
                captureCount = 7;
                capture.transform.localScale = Vector3.one;
            }
        } else
        {
            doneWASD = true;
            doneTutorial = true;
            foreach (SpriteRenderer s in wasd)
            {
                s.enabled = false;
            }
        }

    }

    void Update()
    {
        DoInputs();
        iFrameTimer -= Time.deltaTime;
        captureDelay -= Time.deltaTime;
        float rotateRatio = 1 + attackTimer / attackTime * attackRotateRatio;
        circlerContainer.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime * rotateRatio);
        if(circlerCount == 6 && captureCount == 7)
        {
            float containerScale = 1 - Mathf.Clamp01(attackTimer / attackTime - .8f) * 5f;
            circlerContainer.transform.localScale = Vector3.one * containerScale;
        }
        hitbox.radius = GameManager.Instance.inPhase ? .25f : .2f;
    }

    private void DoInputs()
    {
        //inputs
        Vector2 vel = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) vel += Vector2.left;
        if (Input.GetKey(KeyCode.UpArrow)) vel += Vector2.up;
        if (Input.GetKey(KeyCode.RightArrow)) vel += Vector2.right;
        if (Input.GetKey(KeyCode.DownArrow)) vel += Vector2.down;
        rb.velocity = vel * speed;
        if (!doneWASD && vel.magnitude != 0)
        {
            float a = wasd[0].color.a - Time.deltaTime / 1.5f;
            foreach (SpriteRenderer s in wasd)
            {
                s.color = new Color(1,1,1,a);
            }
            if (a <= 0) doneWASD = true;
        }

        //special attacking
        if(GameManager.Instance.state == GameManager.State.phase5)
        {
            capture.transform.localScale = Vector3.one * Mathf.Clamp01(capture.transform.localScale.x + Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Instantiate(attackPrefab, transform.position, Quaternion.identity);
                sfxSource.PlayOneShot(shootSound);
            }
        }

        //attacking
        if(captureCount == 7)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                rb.velocity *= attackSlowRatio;
                attackTimer += Time.deltaTime;
            } else
            {
                attackTimer -= Time.deltaTime * 2;
            }
            attackTimer = Mathf.Clamp(attackTimer, 0, attackTime);
            float c = (1 - (attackTimer / attackTime)) * (attackTimer == 0 ? (.8f + Time.time % 1 / 5) : 1);
            capture.transform.localScale = new Vector3(c, c, c);
            if(attackTimer == attackTime)
            {
                if (circlerCount < 6)
                {
                    circlers[circlerCount].SetActive(true);
                    circlerCount++;
                    GameManager.Instance.ChangeAttack();
                } else
                {
                    zButton.SetActive(false);
                    for (int i=0; i<7; i++)
                    {
                        Instantiate(attackPrefab, transform.position, Quaternion.identity);
                    }
                    foreach (GameObject circler in circlers)
                    {
                        circler.SetActive(false);
                    }
                    circlerContainer.transform.localScale = Vector3.one;
                    circlerCount = 0;
                }
                Heal(1);
                captureCount = 0;
            }
        } else
        {
            attackTimer = 0;
        }
    }
    bool doneTutorial;
    public void AddCapture()
    {
        if (captureDelay > 0 || captureCount >= 7) return;
        if (!doneTutorial)
        {
            if (!chargeTutorial.enabled) chargeTutorial.enabled = true;
            if(captureCount < 6)
            {
                chargeTutorial.sprite = numbers[captureCount];
            } else
            {
                chargeTutorial.gameObject.SetActive(false);
                zButton.SetActive(true);
                doneTutorial = true;
            }
        }
        captureDelay = 1/6f;
        captureCount += testing ? 7 : 1;
        float c = captureCount / 7f;
        capture.transform.localScale = new Vector3(c,c,c);
    }
    public float iFrameTimer;
    public void Hurt()
    {
        if (iFrameTimer > 0) return;
        iFrameTimer = .5f;
        health--;
        sfxSource2.PlayOneShot(hurtSound);
        healthbar.transform.localScale = new Vector3(8f * health/maxHealth, healthbar.transform.localScale.y, 1);
        if (!testing && health == 0) GameManager.Instance.DoDeath();
    }
    public void Heal(int quantity)
    {
        health = Mathf.Clamp(health + quantity, 0, maxHealth);
        healthbar.transform.localScale = new Vector3(8 * health / maxHealth, healthbar.transform.localScale.y, 1);
    }
}
