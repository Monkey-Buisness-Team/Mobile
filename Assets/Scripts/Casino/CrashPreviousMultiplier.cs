using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrashPreviousMultiplier : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplier;
    private Image background;
    private Color color;

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    public void SetMultiplier(float mult)
    {
        //Determine wich color set depending of the multiplier
        color = GameManager.Instance.greyAccent;
        if(mult >= 2 && mult < 3) color = GameManager.Instance.yellowAccent; 
        else if(mult >= 3) color = GameManager.Instance.greenAccent; 
        
        multiplier.text = mult.ToString("0.00").Replace(',', '.');
        multiplier.color = color;
        background.color = color * new Color(1, 1, 1, 0.2f); //Set color with alpha to 20%
    }

}
