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
}
