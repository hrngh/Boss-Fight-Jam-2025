using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackEvent : MonoBehaviour
{
    private static float BOX_Y = -1.85f;
    public GameObject attackPrefab;
    public Vector2 position;
    public Vector2 bounds;
    public float burst;
    public float frequency;
    public float lifespan;
    public bool canExpire;

    public virtual void Start()
    {
        if(GameManager.Instance) GameManager.Instance.events.Add(this);
    }

    public virtual void Update()
    {
        SpawnAttacks();
        lifespan -= Time.deltaTime;
        if (canExpire && lifespan < 0) Destroy(gameObject);
    }

    public abstract void SpawnAttacks();

    private void OnDestroy()
    {
        GameManager.Instance.events.Remove(this);
    }

    public Vector2 GetBoxSpace(float x, float y)
    {
        x = Mathf.Clamp(Mathf.Round(x*40)/40, 0.05f, 0.95f);
        y = Mathf.Clamp(Mathf.Round(y * 40) / 40, 0.05f, 0.95f);
        return new Vector2(x * 8 - 4, BOX_Y + y * 5 - 2.5f);
    }
}
