using UnityEngine;

public class TextController : MonoBehaviour
{
    public float lifespan;
    public float speed;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Time.timeScale == 0 || timer > lifespan) Destroy(gameObject);
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
}
