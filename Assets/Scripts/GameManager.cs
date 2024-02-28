using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Page { Signup, Home, Bets, Roulette, Crash, Mines, Cases };
public enum Team { Red, Blue };
public enum BetType { Round, Match };

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Paris paris;

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

    public void UpdateLayouts(LayoutGroup[] layouts)
    {
        foreach (var layoutGroup in layouts)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }
}
