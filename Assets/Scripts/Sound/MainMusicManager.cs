using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMusicManager : MonoBehaviour
{
    private static MainMusicManager _instance;
    public static MainMusicManager SharedInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainMusicManager>();
            }
            return _instance;
        }
    }

    public AudioClip menuMusicClip;
    public AudioClip casinoMusicClip;
    public AudioClip betMusicClip;

    private AudioSource musicSource;

    private float transitionTime = 1f;  // Duration of the fade in seconds

    private void Start()
    {
        musicSource = gameObject.GetComponent<AudioSource>();
    }

    public void ChangeMusic(Page pageType)
    {
        AudioClip clipToPlay = GetClipFromEnum(pageType);

        if (musicSource.clip != clipToPlay)
        {
            StartCoroutine(FadeMusic(clipToPlay));
        }
    }

    private AudioClip GetClipFromEnum(Page pageType)
    {
        switch (pageType)
        {
            case Page.Home:
                return menuMusicClip;
            case Page.Casino:
                return casinoMusicClip;
            case Page.Bets:
                return betMusicClip;
            default:
                return null;
        }
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        while (musicSource.volume > 0.02)
        {
            musicSource.volume -= Time.deltaTime / transitionTime;
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();
        musicSource.loop = true;

        while (musicSource.volume < 0.98)
        {
            musicSource.volume += Time.deltaTime / transitionTime;
            yield return null;
        }
    }
}
