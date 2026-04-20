using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{
    public AnimationCurve lerp;
    public bool loop;
    [HideInInspector] public bool isDone;
    [HideInInspector] public bool onTrack;
    public float time;
    public float delay;
    public Vector3 start;
    public Vector3 end;
    public bool isHandL;
    public bool isHandR;
    public bool targetPlayer;

    private float timer;

    private void Start()
    {
        timer = -delay;
        if (delay != 0) {
            Invoke("Aim", delay);
        } else
        {
            if (isHandL)
            {
                GameManager.Instance.handLTransform.gameObject.GetComponent<BossPartMover>().possessed = true;
                start = GameManager.Instance.handLTransform.position;
            }
            if (isHandR)
            {
                GameManager.Instance.handRTransform.gameObject.GetComponent<BossPartMover>().possessed = true;
                start = GameManager.Instance.handRTransform.position;
            }
            if (targetPlayer) end = Player.Instance.transform.position;
        }
    }
    private void Aim()
    {
        if (isHandL)
        {
            GameManager.Instance.handLTransform.gameObject.GetComponent<BossPartMover>().possessed = true;
            start = GameManager.Instance.handLTransform.position;
        }
        if (isHandR)
        {
            GameManager.Instance.handRTransform.gameObject.GetComponent<BossPartMover>().possessed = true;
            start = GameManager.Instance.handRTransform.position;
        }
        if (targetPlayer) end = Player.Instance.transform.position;
    }

    private void OnDestroy()
    {
        if (onTrack) return;
        if (isHandL)
        {
            GameManager.Instance.handLTransform.gameObject.GetComponent<BossPartMover>().possessed = false;
            GameManager.Instance.handLTransform.position = start;
        }
        if (isHandR)
        {
            GameManager.Instance.handRTransform.gameObject.GetComponent<BossPartMover>().possessed = false;
            GameManager.Instance.handRTransform.position = start;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= time) isDone = true;
        if (isDone && !loop) return; 
        Transform targetTransform = isHandL ? GameManager.Instance.handLTransform : isHandR ? GameManager.Instance.handRTransform : transform;
        if(timer > 0) targetTransform.position = Vector3.Lerp(start, end, lerp.Evaluate(timer % time / time));
    }
}
