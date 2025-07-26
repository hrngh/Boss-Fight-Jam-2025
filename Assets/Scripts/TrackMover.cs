using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{
    public AnimationCurve x;
    public AnimationCurve y;
    public float time;

    private float timer;
    private Vector3 start;
    void Start()
    {
        start = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = start + new Vector3(x.Evaluate(timer % time / time), y.Evaluate(timer % time / time));
    }
}
