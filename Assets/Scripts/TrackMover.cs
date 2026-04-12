using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{
    public AnimationCurve lerp;
    public float time;
    public Vector3 start;
    public Vector3 end;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(start, end, lerp.Evaluate(timer % time / time));
    }
}
