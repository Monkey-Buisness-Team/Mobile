using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrashPreviousMultiplier : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private Image background;

    public void SetMultiplier(float mult)
    {
        multiplier.text = "x" + mult.ToString().Replace(',', '.');
    }

}
