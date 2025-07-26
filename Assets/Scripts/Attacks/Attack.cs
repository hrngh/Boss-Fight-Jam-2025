using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public float curve;

    void Start()
    {
        if (GameManager.Instance) GameManager.Instance.attacks.Add(this);
    }

    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * speed);
        transform.Rotate(Vector3.forward, curve * Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameManager.Instance.attacks.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall"){
            Destroy(gameObject);
        }
        if (collision.tag == "Player")
        {
            if (GameManager.Instance.inPhase)
            {
                Player.Instance.AddCapture();
            } else
            {
                Player.Instance.Hurt();
            }
            Destroy(gameObject);
        }
    }
}