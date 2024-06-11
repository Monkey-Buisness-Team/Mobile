using DG.Tweening;
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

    private IEnumerator _coroutine;

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
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = FadeMusic(clipToPlay);
            StartCoroutine(_coroutine);
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
        yield return musicSource.DOFade(0, 0.05f).WaitForCompletion();

        musicSource.clip = newClip;
        musicSource.Play();
        musicSource.loop = true;

        yield return musicSource.DOFade(0.1f, 0.05f).WaitForCompletion();

        _coroutine = null;
    }
}
