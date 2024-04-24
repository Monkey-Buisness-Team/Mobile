using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Paris : MonoBehaviour
{   
    [SerializeField] QuickInputPad inputPad;
    [SerializeField] Transform redCurrentBetsContainer;
    [SerializeField] Transform blueCurrentBetsContainer;
    [SerializeField] CurrentPlayerBetUI playerBetUIPrefab;

    [Header("Bet Button")]
    [SerializeField] Button betButton;
    [SerializeField] Image betButtonImage;
    [SerializeField] TextMeshProUGUI betButtonLabel;
    [SerializeField] Color betButtonColor;
    [SerializeField] Color betButtonDisabledColor;

    [Header("Odds")]
    [SerializeField] TextMeshProUGUI redRoundOddsText;
    [SerializeField] TextMeshProUGUI blueRoundOddsText;
    [Space(5)]
    [SerializeField] TextMeshProUGUI redMatchOddsText;
    [SerializeField] TextMeshProUGUI blueMatchOddsText;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;

    List<CurrentPlayerBetUI> playerBets = new List<CurrentPlayerBetUI>();
    Team currentBetTeam;
    BetType currentBetType;

    private float wageredBet;

    public async void Bet()
    {
        wageredBet = inputPad.GetCurrentInputValue();
        if(wageredBet < 100 || wageredBet > UserBehaviour.i.Bananas) return;
        
        GameManager.Instance.bananas -= wageredBet;
        AddPlayerBet(currentBetTeam, wageredBet);

        betButton.interactable = false;
        betButtonImage.color = betButtonDisabledColor;
        betButtonLabel.text = "Montant parié";

        // TODO : bet sur round ou match a l'aide de currentbetType
        int banana = Mathf.RoundToInt(wageredBet);
        string fighterName = await BetManager.i.GetFighterName(currentBetTeam == Team.Red);

        switch (currentBetType)
        {
            case BetType.Round:
                await BetManager.i.BetOnRound(banana, fighterName);
                break;
            case BetType.Match:
                await BetManager.i.BetOnMatch(banana, fighterName);
                break;
            default:
                break;
        }
    }

    public void OnBetTypeSelected(Team team, BetType type)
    {
        currentBetTeam = team;
        currentBetType = type;
    }

    /// <summary>
    /// TODO netcode : Takes the player data in parameter and adds it the current bets
    /// </summary>
    public void AddPlayerBet(Team team, float bananas)
    {
        CurrentPlayerBetUI bet = Instantiate(playerBetUIPrefab, team == Team.Red ? redCurrentBetsContainer : blueCurrentBetsContainer);
        bet.InitializeBet(Mathf.RoundToInt(bananas));
        playerBets.Add(bet);

        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    public void RemovePlayerBet(CurrentPlayerBetUI bet)
    {
        playerBets.Remove(bet);        
        Destroy(bet.gameObject);
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    public void ClearAllBets()
    {
        foreach(CurrentPlayerBetUI t in playerBets)
            Destroy(t.gameObject);
        playerBets.Clear();
    }

    public void UpdateScore()
    {
        //TODO : Get score from gameManager
    }

    #region ODDS UPDATE
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
    #endregion ODDS UPDATE
}
