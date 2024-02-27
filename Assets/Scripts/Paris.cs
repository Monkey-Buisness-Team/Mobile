using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Paris : MonoBehaviour
{   
    [SerializeField] QuickInputPad inputPad;
    [SerializeField] GameObject betButton;

    [Header("Odds")]
    [SerializeField] TextMeshProUGUI redRoundOddsText;
    [SerializeField] TextMeshProUGUI blueRoundOddsText;
    [Space(5)]
    [SerializeField] TextMeshProUGUI redMatchOddsText;
    [SerializeField] TextMeshProUGUI blueMatchOddsText;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;


    private float wageredBet;

    public void Bet()
    {
        wageredBet = inputPad.GetCurrentInputValue();
        if(wageredBet < 100) return;

        
    }

    #region SCORE & ODDS UPDATE
    public void UpdateRedScore()
    {
        //Get score from gameManager
    }

    public void UpdateBlueScore()
    {
        //Get score from gameManager
    }


    public void UpdateRedRoundOdd(float odd)
    {
        redRoundOddsText.text = odd.ToString("0.00");
    }

    public void UpdateRedMatchOdd(float odd)
    {
        redMatchOddsText.text = odd.ToString("0.00");
    }

    public void UpdateBlueRoundOdd(float odd)
    {
        blueRoundOddsText.text = odd.ToString("0.00");
    }
    public void UpdateBlueMatchOdd(float odd)
    {
        blueMatchOddsText.text = odd.ToString("0.00");
    }
    #endregion SCORE & ODDS UPDATE
}
