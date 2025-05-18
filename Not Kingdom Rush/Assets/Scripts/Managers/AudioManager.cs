using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Managers")]
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    [Header("Select Level Starting Music")]
    [SerializeField] private bool playBGM;
    [SerializeField] private int levelStartBGM;

    [Header("List of SFX")]
    [SerializeField] private AudioSource bombExplosion;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        if (!playBGM)
            StopAllBGM();
        else
        {
            if (!bgm[levelStartBGM].isPlaying)
                PlayBGM(levelStartBGM);
        }
    }

    public void PlaySFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Play();
        }
    }

    public void PlaySFXRandomPitch(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.8f, 1.2f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    public void PlayBGM(int _bgmIndex)
    {
        StopAllBGM();

        bgm[_bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    public void SFXBombExplosion()
    {
        bombExplosion.Play();
    }
}
