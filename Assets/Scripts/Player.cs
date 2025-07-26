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
    public GameObject capture;
    public GameObject circlerContainer;
    public GameObject[] circlers;
    public GameObject attackPrefab;
    public GameObject healthbar;

    private int health;
    private int captureCount;
    private int circlerCount;
    private float captureDelay;
    private float attackTimer;

    void Start()
    {
        health = maxHealth;
        Instance = this;
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
                captureCount = 0;
            }
        } else
        {
            attackTimer = 0;
        }
    }

    public void AddCapture()
    {
        if (captureDelay > 0 || captureCount >= 7) return;
        captureDelay = 2/3f;
        captureCount += testing ? 7 : 1;
        float c = captureCount / 7f;
        capture.transform.localScale = new Vector3(c,c,c);
    }
    private float iFrameTimer;
    public void Hurt()
    {
        if (iFrameTimer > 0) return;
        iFrameTimer = .5f;
        health--;
        healthbar.transform.localScale = new Vector3(8 * health/maxHealth, healthbar.transform.localScale.y, 1);
        //TODo die
    }
    public void Heal(int quantity)
    {
        health = Mathf.Clamp(health + quantity, 0, maxHealth);
        healthbar.transform.localScale = new Vector3(8 * health / maxHealth, healthbar.transform.localScale.y, 1);
    }
}
