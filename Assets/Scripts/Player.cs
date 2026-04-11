using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float speed;
    public AnimationCurve attackSlowRatio;
    public AnimationCurve attackCurve;
    public float attackTime;
    public int maxHealth;

    public Rigidbody2D rb;
    public CircleCollider2D hitbox;
    public GameObject attackPrefab;
    public GameObject healthbar;
    public SpriteRenderer zButton;
    public SpriteRenderer[] wasd;
    public AudioSource sfxSource;
    public AudioClip shootSound;
    public AudioSource sfxSource2;
    public AudioClip hurtSound;

    private int health;
    private float attackTimer;
    private float subAttackTimer;
    private bool doneWASD;
    private bool doneZ;

    void Start()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
        health = maxHealth;
    }

    void Update()
    {
        DoInputs();
        iFrameTimer -= Time.deltaTime;
    }

    private void DoInputs()
    {
        //inputs
        Vector2 vel = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) vel += Vector2.left;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) vel += Vector2.up;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) vel += Vector2.right;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) vel += Vector2.down;
        rb.linearVelocity = vel * speed;
        if (!doneWASD && vel.magnitude != 0)
        {
            float a = wasd[0].color.a - Time.deltaTime / 1.5f;
            foreach (SpriteRenderer s in wasd)
            {
                s.color = new Color(1,1,1,a);
            }
            if (a <= 0) doneWASD = true;
        }

        // Randomization
        if (Input.GetKeyDown(KeyCode.R))
        {
            SFXManager.Instance.RandomizeSounds();
            SpriteManager.Instance.RandomizeSprites();
        }

        // attacking
        if (Input.GetKey(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
        {
            if (!doneZ)
            {
                float a = zButton.color.a - Time.deltaTime / 1.5f;
                zButton.color = new Color(1, 1, 1, a);
                if (a <= 0) doneZ = true;
            }
            rb.linearVelocity *= attackSlowRatio.Evaluate(Mathf.Clamp01(attackTimer / attackTime));
            attackTimer += Time.deltaTime;
            subAttackTimer += Time.deltaTime;
            if (subAttackTimer > attackCurve.Evaluate(Mathf.Clamp01(attackTimer/attackTime))) { 
                Instantiate(attackPrefab, transform.position, Quaternion.identity);
                sfxSource.PlayOneShot(shootSound);
                subAttackTimer -= attackCurve.Evaluate(Mathf.Clamp01(attackTimer / attackTime));
            }
        } else
        {
            attackTimer = 0;
            subAttackTimer = attackCurve.Evaluate(0)-.1f;
        }
    }
    public float iFrameTimer;
    public void Hurt()
    {
        if (iFrameTimer > 0) return;
        GameManager.Instance.DoBloom();
        iFrameTimer = .5f;
        health--;
        sfxSource2.PlayOneShot(hurtSound);
        healthbar.transform.localScale = new Vector3(8f * health/maxHealth, healthbar.transform.localScale.y, 1);
        if (health == 0) GameManager.Instance.DoDeath();
    }
    public void Heal(int quantity)
    {
        health = Mathf.Clamp(health + quantity, 0, maxHealth);
        healthbar.transform.localScale = new Vector3(8f * health / maxHealth, healthbar.transform.localScale.y, 1);
    }
}
