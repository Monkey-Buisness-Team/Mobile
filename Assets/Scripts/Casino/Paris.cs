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
        
        //GameManager.Instance.bananas -= wageredBet;
        //AddPlayerBet(currentBetTeam, wageredBet);

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

    public void Start()
    {
        FirebaseAutorisationManager.i.OnRoomChange += UpdateBetButton;
        BetManager.i.OnBetReceive += AddPlayerBet;
        BetManager.i.OnClearBet += ClearAllBets;
        SetBetButton(false);
    }

    public void OnDestroy()
    {
        FirebaseAutorisationManager.i.OnRoomChange -= UpdateBetButton;
        BetManager.i.OnBetReceive -= AddPlayerBet;
        BetManager.i.OnClearBet -= ClearAllBets;
    }

    public async void UpdateBetButton()
    {
        bool open = false;
        switch (currentBetType)
        {
            case BetType.Round:
                open = await FirebaseAutorisationManager.i.IsRoundBetOpen();
                break;

            case BetType.Match:
                open = await FirebaseAutorisationManager.i.IsMatchBetOpen();
                break;
        }

        SetBetButton(open);
    }

    public async void OnBetTypeSelected(Team team, BetType type)
    {
        currentBetTeam = team;
        currentBetType = type;

        bool open = false;
        switch (type)
        {
            case BetType.Round:
                open = await FirebaseAutorisationManager.i.IsRoundBetOpen() && await FirebaseAutorisationManager.i.CheckAlreadyBet("ROUND_BET");
                break;

            case BetType.Match:
                open = await FirebaseAutorisationManager.i.IsMatchBetOpen() && await FirebaseAutorisationManager.i.CheckAlreadyBet("MATCH_BET"); ;
                break;
        }

        SetBetButton(open);
    }

    private void SetBetButton(bool isActive)
    {
        betButton.interactable = isActive;
        betButtonImage.color = isActive ? betButtonColor : betButtonDisabledColor;
        betButtonLabel.text = isActive ? "Parier" : "Pari Indisponible";
    }

    /// <summary>
    /// TODO netcode : Takes the player data in parameter and adds it the current bets
    /// </summary>
    //public void AddPlayerBet(Team team, float bananas)
    //{
    //    AddPlayerBet(team, bananas, UserBehaviour.i.UserName, BetType.Round);
    //}

    public void AddPlayerBet(Team team, float bananas, string userName, BetType betType)
    {
        CurrentPlayerBetUI bet = Instantiate(playerBetUIPrefab, team == Team.Red ? redCurrentBetsContainer : blueCurrentBetsContainer);
        bet.InitializeBet(Mathf.RoundToInt(bananas), userName, betType);
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
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    public void ClearAllBets(BetType type)
    {
        foreach (CurrentPlayerBetUI t in playerBets)
        {
            if(t.Type == type)
            {
                playerBets.Remove(t);
                Destroy(t.gameObject);
            }
        }
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
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
