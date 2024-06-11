using DG.Tweening;
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

    private IEnumerator _coroutine;

    private void Start()
    {
        musicSource = gameObject.GetComponent<AudioSource>();
    }

    public void ChangeMusic(Page pageType)
    {
        AudioClip clipToPlay = GetClipFromEnum(pageType);
        if (clipToPlay == null) return;

        if (musicSource.clip != clipToPlay)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
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
        yield return musicSource.DOFade(0, 0.05f).WaitForCompletion();

        musicSource.clip = newClip;
        musicSource.Play();
        musicSource.loop = true;

        yield return musicSource.DOFade(0.1f, 0.05f).WaitForCompletion();

        _coroutine = null;
    }
}
