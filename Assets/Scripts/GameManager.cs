using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
