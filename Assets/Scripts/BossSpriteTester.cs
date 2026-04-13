using UnityEngine;

[ExecuteInEditMode]
public class BossSpriteTester : MonoBehaviour
{
    [SerializeField] private BossSpriteList spriteList;
    [SerializeField] private SpriteRenderer bossEye;
    [SerializeField] private SpriteRenderer bossEyeR;
    [SerializeField] private SpriteRenderer bossPupil;
    [SerializeField] private SpriteRenderer bossPupilR;
    [SerializeField] private SpriteRenderer bossHand;
    [SerializeField] private SpriteRenderer bossHandR;
    [SerializeField] private SpriteRenderer bossMouth;
    [SerializeField] private SpriteRenderer bossBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        BossSpriteList boss = spriteList;
        bossEye.sprite = boss.eye;
        bossEyeR.sprite = boss.eyeR ? boss.eyeR : boss.eye;
        bossEyeR.flipX = !boss.eyeR;
        bossPupil.sprite = boss.pupil;
        bossPupilR.sprite = boss.pupilR ? boss.pupilR : boss.pupil;
        bossPupilR.flipX = !boss.pupilR;
        bossHand.sprite = boss.hand;
        bossHandR.sprite = boss.handR ? boss.handR : boss.hand;
        bossHandR.flipX = !boss.handR;
        bossMouth.sprite = boss.mouth;
        bossBody.sprite = boss.body;
    }
}
