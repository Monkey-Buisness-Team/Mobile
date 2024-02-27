using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Page { Signup, Home, Bets, Roulette, Crash, Mines, Cases };

public class GameManager : MonoBehaviour
{
    [Header("Accent Colors")]
    public Color redAccent;
    public Color greenAccent;
    public Color yellowAccent;
    public Color greyAccent;

    [Header("Player Data")]
    public float bananas;

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        bananas = 10000;
    }
}
