using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Page { Signup, Home, Bets, Roulette, Crash, Mines, Cases, Casino, Fruit, LeaderBoard, LBananas, LPari, LCombat };
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

    public static string GetBananas(int bananas)
    {
        float b = bananas;
        string sign = string.Empty;

        if (bananas > 999999999)
        {
            b = bananas / 1000000000f;
            sign = "B";
        }
        else if (bananas > 999999)
        {
            b = bananas / 1000000f;
            sign = "M";
        }
        else if (bananas > 999)
        {
            b = bananas / 1000f;
            sign = "K";
        }

        b = Mathf.RoundToInt(b * 100) /100f;
        if(b <= 0)
            b = 0;
        return $"{b}{sign}";
    }
}
