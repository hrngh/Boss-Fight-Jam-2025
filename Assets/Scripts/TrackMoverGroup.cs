using UnityEngine;

public class TrackMoverGroup : MonoBehaviour
{
    public TrackMover[] tracks;
    public bool isHandL;
    public bool isHandR;
    private Vector3 start;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = isHandL ? GameManager.Instance.handLTransform.position : isHandR ? GameManager.Instance.handRTransform.position : Vector3.zero;
        foreach (TrackMover track in tracks)
        {
            track.enabled = false;
        }
        tracks[0].enabled = true;
        tracks[0].onTrack = true;
    }

    private void OnDestroy()
    {
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

    // Update is called once per frame
    private int index = 0;
    void Update()
    {
        if (index < tracks.Length && tracks[index].isDone)
        {
            tracks[index++].enabled = false;
            if (index < tracks.Length)
            {
                tracks[index].enabled = true;
                tracks[index].onTrack = true;
            }
        }
    }
}
