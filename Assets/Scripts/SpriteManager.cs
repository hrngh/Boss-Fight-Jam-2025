using UnityEngine;

[System.Serializable]
public class SpriteList
{
    public Sprite[] list;
}
[System.Serializable]
public class BossSpriteList
{
    public Sprite eye;
    public Sprite eyeR;
    public Sprite pupil;
    public Sprite pupilR;
    public Sprite hand;
    public Sprite handR;
    public Sprite mouth;
    public Sprite body;
    public float eyeStrength;
    public float eyeStrengthR;
}

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;
    [SerializeField] private SpriteRenderer bossEye;
    [SerializeField] private SpriteRenderer bossEyeR;
    [SerializeField] private SpriteRenderer bossPupil;
    [SerializeField] private SpriteRenderer bossPupilR;
    [SerializeField] private SpriteRenderer bossHand;
    [SerializeField] private SpriteRenderer bossHandR;
    [SerializeField] private SpriteRenderer bossMouth;
    [SerializeField] private SpriteRenderer bossBody;
    [SerializeField] private BossSpriteList[] bossLists;
    [SerializeField] private SpriteList[] spriteLists;
    private Sprite[] sprites;
    [SerializeField] private Sprite[] playerSprites;
    public enum AttackType
    {
        sharp,
        blunt,
        magic
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
        sprites = new Sprite[spriteLists.Length];
        Invoke("RandomizeSprites", .1f);
    }


    public void RandomizeSprites()
    {
        // Player
        GameManager.Instance.playerSprite.sprite = playerSprites[Random.Range(0, playerSprites.Length)];

        // Boss
        BossSpriteList boss = bossLists[Random.Range(0, bossLists.Length)];
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
        bossPupil.gameObject.GetComponent<BossPartMover>().eyeStrength = boss.eyeStrength;
        bossPupilR.gameObject.GetComponent<BossPartMover>().eyeStrength = boss.eyeStrengthR != 0 ? boss.eyeStrengthR : boss.eyeStrength;

        // Attacks
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = spriteLists[i].list[Random.Range(0, spriteLists[i].list.Length)];
        }
    }

    public Sprite GrabAttack(AttackType type)
    {
        return sprites[(int)type];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
