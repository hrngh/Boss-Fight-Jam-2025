using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public float curve;
    public float waveAmp;
    public float waveFreq;
    public bool piercing;
    public float lifespan;
    public bool chaser;
    public SFXManager.AttackType sound;
    public SpriteManager.AttackType look;

    private float timer;

    void Start()
    {
        if (GameManager.Instance) GameManager.Instance.attacks.Add(this);
        if (SFXManager.Instance) SFXManager.Instance.PlayAttack(sound);
        if (SpriteManager.Instance && GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GrabAttack(look);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (lifespan > 0) {
            lifespan -= Time.deltaTime;
            if (lifespan <= 0) Destroy(gameObject);
        }
        if (chaser)
        {
            Vector3 aim = Player.Instance.transform.position - transform.position;
            transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.right, aim) * (aim.y < 0 ? -1 : 1));
        }
        transform.Translate(Vector2.right * Time.deltaTime * speed);
        transform.Translate(Vector2.up * Time.deltaTime * waveAmp * Mathf.Cos(timer * waveFreq));
        transform.Rotate(Vector3.forward, curve * Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameManager.Instance.attacks.Remove(this);
    }

    private bool hitTop;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.Instance.Hurt();
            if(!piercing) Destroy(gameObject);
        }
        if (piercing) return;
        if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        } else if(collision.tag == "TopWall")
        {
            if(transform.position.y < collision.transform.position.y && !hitTop)
            {
                Destroy(gameObject);
            }
            hitTop = true;
        }
    }
}