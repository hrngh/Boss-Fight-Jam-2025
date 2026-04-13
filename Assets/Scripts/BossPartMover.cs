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
    public bool isHand;
    public float eyeStrength;
    private float maxSpeed = .8f;
    public bool possessed;

    private Vector3 origPos;
    private float xVel;
    private float yVel;
    private float rotVel;

    void Start()
    {
        origPos = transform.localPosition;
        Invoke("addToList", 0.1f);
    }

    private void addToList()
    {
        GameManager.Instance.addMover(this);
    }

    void Update()
    {
        if (possessed) return;
        if (!isEye)
        {
            //movement
            float randomRat = Mathf.Clamp01(1 - (origPos - transform.localPosition).magnitude / maxRange);
            xVel += Random.Range(-1f, 1f) * Time.deltaTime * maxAccel * randomRat;
            yVel += Random.Range(-1f, 1f) * Time.deltaTime * maxAccel * randomRat;
            Vector3 grav = (origPos - transform.localPosition).normalized;
            xVel += grav.x * Time.deltaTime * maxAccel * (1 - randomRat);
            yVel += grav.y * Time.deltaTime * maxAccel * (1 - randomRat);
            if (Mathf.Abs(xVel) > maxSpeed) xVel *= .97f;
            if (Mathf.Abs(yVel) > maxSpeed) yVel *= .97f;
            transform.Translate(new Vector2(xVel, yVel) * Time.deltaTime, Space.World);

        }
        else
        {
            Vector3 dir = (Player.Instance.transform.position - transform.position).normalized;
            transform.localPosition = origPos + dir * eyeStrength * Mathf.Clamp01(GameManager.Instance.eyeStrengthMult);
        }

        if (rotateRange != 0)
        {
            //rotation
            float currentRot = transform.localEulerAngles.z % 360;
            if (currentRot < -180) currentRot += 360;
            if (currentRot > 180) currentRot -= 360;
            float rot_randomRat = Mathf.Clamp01(1 - Mathf.Abs(currentRot) / rotateRange);
            rotVel += Random.Range(-1f, 1f) * Time.deltaTime * rotateAccel * rot_randomRat;
            float rot_grav = -currentRot;
            rotVel += rot_grav * Time.deltaTime * rotateAccel * (1 - rot_randomRat);
            transform.Rotate(0, 0, rotVel * Time.deltaTime);
        }
        if (isHand)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.down, Player.Instance.transform.position - transform.position);
        }
    }

    public void Chaos()
    {
        Invoke("Order", 0.2f);
        float chaosScale = .5f;
        float randRot = Random.Range(0, Mathf.PI * 2);
        xVel += Mathf.Cos(randRot) * chaosScale;
        yVel += Mathf.Sin(randRot) * chaosScale;
        rotVel += Random.Range(-1, 1) * chaosScale;
    }
    public void Order()
    {
        Vector3 grav = origPos - transform.localPosition;
        xVel = grav.x / 2;
        yVel = grav.y / 2;
    }
}
