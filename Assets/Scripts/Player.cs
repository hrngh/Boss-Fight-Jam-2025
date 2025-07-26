using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float speed;
    public int startHealth;

    private int health;
    private int captureCount;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        DoInputs();
    }

    private void DoInputs()
    {
        //inputs
        Vector2 vel = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) vel += Vector2.left;
        if (Input.GetKey(KeyCode.UpArrow)) vel += Vector2.up;
        if (Input.GetKey(KeyCode.RightArrow)) vel += Vector2.right;
        if (Input.GetKey(KeyCode.DownArrow)) vel += Vector2.down;
        transform.Translate(vel * speed * Time.deltaTime);
    }
}
