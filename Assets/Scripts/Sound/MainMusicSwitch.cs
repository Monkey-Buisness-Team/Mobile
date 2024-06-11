using System.Collections;
using UnityEngine;

public class MainMusicSwitch : MonoBehaviour
{
    public enum MusicEnum
    {
        MenuMusic,
        CasinoMusic,
        BetMusic
    }

    [SerializeField] private MusicEnum musicSelected = MusicEnum.MenuMusic;

    private AudioClip menuMusicClip;
    private AudioClip casinoMusicClip;
    private AudioClip betMusicClip;

    private AudioSource musicSource;
    private float transitionTime = 1.5f;  // Duration of the fade in seconds

    private void Awake()
    {
        musicSource = GameObject.FindWithTag("MainMusicAudioSource").GetComponent<AudioSource>();

        menuMusicClip = MainMusicManager.SharedInstance.menuMusicClip;
        casinoMusicClip = MainMusicManager.SharedInstance.casinoMusicClip;
        betMusicClip = MainMusicManager.SharedInstance.betMusicClip;
    }

    public void ChangeMusic()
    {
        AudioClip clipToPlay = GetClipFromEnum(musicSelected);

        if (musicSource.clip != clipToPlay && this.gameObject.activeSelf)
        {
            StartCoroutine(FadeMusic(clipToPlay));
        }
    }

    private AudioClip GetClipFromEnum(MusicEnum musicType)
    {
        switch (musicType)
        {
            case MusicEnum.MenuMusic:
                return menuMusicClip;
            case MusicEnum.CasinoMusic:
                return casinoMusicClip;
            case MusicEnum.BetMusic:
                return betMusicClip;
            default:
                return null;
        }
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume -= Time.deltaTime / transitionTime;
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();
        musicSource.loop = true;

        while (musicSource.volume < 1)
        {
            musicSource.volume += Time.deltaTime / transitionTime;
            yield return null;
        }
    }
}
