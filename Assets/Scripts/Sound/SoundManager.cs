using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FruitGameManager;

public class SoundManager : MonoBehaviour
{
    public static readonly SoundManager sharedInstance = new SoundManager();
    [Serializable] public struct Sound
    {
        public string id;
        public AudioClip audioClip;
    }

    public AudioSource audioSource;
    public List<Sound> soundList = new List<Sound>();

    private SoundManager() { } //Private init

    public void playSoundWithId(string id)
    {
        Sound soundFound = soundList.Find(x => x.id == id);
        audioSource.PlayOneShot(soundFound.audioClip);
    }
}
