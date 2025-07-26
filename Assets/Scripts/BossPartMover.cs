using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPartMover : MonoBehaviour
{
    public float maxAccel;
    public float maxRange;
    public float rotateAccel;
    public float rotateRange;
    public bool isEye;
    public float eyeStrength;

    private Vector3 origPos;
    private float xVel;
    private float yVel;
    private float rotVel;

    void Start()
    {
        origPos = transform.localPosition;
    }

    void Update()
    {
        if (!isEye)
        {
            //movement
            float randomRat = Mathf.Clamp01(1 - (origPos - transform.localPosition).magnitude / maxRange);
            xVel += Random.Range(-1f, 1f) * Time.deltaTime * maxAccel * randomRat;
            yVel += Random.Range(-1f, 1f) * Time.deltaTime * maxAccel * randomRat;
            Vector3 grav = (origPos - transform.localPosition).normalized;
            xVel += grav.x * Time.deltaTime * maxAccel * (1 - randomRat);
            yVel += grav.y * Time.deltaTime * maxAccel * (1 - randomRat);
            transform.Translate(new Vector2(xVel, yVel) * Time.deltaTime);

        } else
        {
            Vector3 dir = (Player.Instance.transform.position - transform.position).normalized;
            transform.localPosition = origPos + dir * eyeStrength;
        }

        if (rotateRange != 0)
        {
            //rotation
            float currentRot = transform.eulerAngles.z;
            if (currentRot < -180) currentRot += 360;
            if (currentRot > 180) currentRot -= 360;
            float rot_randomRat = Mathf.Clamp01(1 - Mathf.Abs(currentRot) / rotateRange);
            rotVel += Random.Range(-1f, 1f) * Time.deltaTime * rotateAccel * rot_randomRat;
            float rot_grav = -currentRot;
            rotVel += rot_grav * Time.deltaTime * rotateAccel * (1 - rot_randomRat);
            transform.Rotate(0, 0, rotVel * Time.deltaTime);
        }
    }
}
