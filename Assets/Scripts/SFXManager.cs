using UnityEngine;

[System.Serializable]
public class SoundList
{
    public AudioClip[] list;
}

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;
    public AudioSource sfxSource;
    public AudioSource bossSource;

    [SerializeField] private SoundList[] soundLists;
    [SerializeField] private AudioClip[] playerShootSounds;
    [SerializeField] private AudioClip[] playerHurtSounds;
    [SerializeField] private SoundList[] bossHurtSounds;
    private AudioClip[] sounds;
    private AudioClip[] bossHurtSound;
    public enum AttackType
    {
        whoosh,
        click,
        boom,
        none
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
        sounds = new AudioClip[soundLists.Length];
        Invoke("RandomizeSounds", .1f);
    }

    public void RandomizeSounds()
    {
        for (int i=0; i<sounds.Length; i++)
        {
            sounds[i] = soundLists[i].list[Random.Range(0, soundLists[i].list.Length)];
        }
        Player.Instance.shootSound = playerShootSounds[Random.Range(0, playerShootSounds.Length)];
        Player.Instance.hurtSound = playerHurtSounds[Random.Range(0, playerHurtSounds.Length)];
        bossHurtSound = bossHurtSounds[Random.Range(0, bossHurtSounds.Length)].list;
    }

    public void HurtBoss()
    {
        bossSource.pitch = Random.Range(.8f, 1.2f);
        bossSource.PlayOneShot(bossHurtSound[Random.Range(0, bossHurtSound.Length)]);
    }

    public void PlayAttack(AttackType type)
    {
        if (type == AttackType.none) return;
        sfxSource.pitch = Random.Range(.8f, 1.2f);
        Debug.Log(sounds.Length);
        sfxSource.PlayOneShot(sounds[(int)type]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
