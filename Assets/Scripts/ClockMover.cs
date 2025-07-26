using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMover : MonoBehaviour
{
    public float vel;
    public SpriteRenderer[] sprites;
    private bool dead;
    void Start()
    {
        if (GameManager.Instance) GameManager.Instance.clocks.Add(this);
    }

    void Update()
    {
        if(vel > 0 && transform.position.x >= 0 || vel < 0 && transform.position.x <= 0)
        {
            Destroy(gameObject);
        }
        if (!dead)
        {
            transform.Translate(Vector2.right * vel * Time.deltaTime);
        } else
        {
            float newAlpha = sprites[0].color.a - Time.deltaTime;
            Color newColor = new Color(1, 1, 1, newAlpha);
            if (newAlpha <= 0) Destroy(gameObject);
            foreach (SpriteRenderer s in sprites)
            {
                s.color = newColor;
            }
        }
    }

    public void Kill()
    {
        dead = true;    
    }

    private void OnDestroy()
    {
        GameManager.Instance.clocks.Remove(this);
    }
}
