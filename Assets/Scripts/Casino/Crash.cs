using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Crash : MonoBehaviour
{
    [SerializeField] QuickInputPad inputPad;
    private float currentCrashMultiplier;
    [SerializeField] Color boomColor;

    [SerializeField] TextMeshProUGUI currentMultiplierText;
    [SerializeField] CrashPreviousMultiplier previousCrashPrefab;
    [SerializeField] RectTransform crashHistoryContainer;

    [Header("Bet / Cashout")]
    [SerializeField] Color betColor;
    [SerializeField] Color betDisabledColor;
    [SerializeField] Color cashoutColor;
    [SerializeField] TextMeshProUGUI betButtonLabel;
    [SerializeField] TextMeshProUGUI betButtonValueText;
    [SerializeField] Image betButtonImage;
    [SerializeField] Button betButton;

    [Header("Countdown")]
    [SerializeField] GameObject countdownGO;
    [SerializeField] Image countdownFillBar;
    [SerializeField] TextMeshProUGUI countdownText;
    float countdown;
    bool isCountingDown;

    [Header("Player Bets")]
    [SerializeField] GameObject currentBets;
    [SerializeField] CurrentPlayerBetUI playerBetUIPrefab;
    [SerializeField] Transform currentBetsContainer;
    [SerializeField] TextMeshProUGUI totalBananasBet;

    [Space(5)]
    [SerializeField] GameObject cashedBets;
    [SerializeField] CurrentPlayerCashedBetUI playerCashedBetUIPrefab;
    [SerializeField] Transform cashedBetsContainer;

    private int crashHistoryMultipliersAmount = 5;
    Queue<CrashPreviousMultiplier> crashesHistory = new Queue<CrashPreviousMultiplier>();

    [Header("Crash Modifiers")]

    [SerializeField] AnimationCurve multiplicationSpeed;
    float time;
    float current;
    bool active;
    int colorChanged;
    int countdownColorChanged;
    public float crashSpeedMultiplier;
    float wageredBet;
    public bool isWagering;

    public static Crash Instance;
    private void Awake()
    {
        Instance = this;
    }

    // private void Awake()
    // {
    //     //Resize the multipliers history depending of the screen width in order to display more values
    //     for(int i = 1; i <= 5; i++)
    //     {
    //         print(crashHistoryContainer.rect.width);
    //         print(((5+i)*230)-10);
    //         if(crashHistoryContainer.rect.width > ((5+i)*230)-10) 
    //             crashHistoryMultipliersAmount++;
    //         else break;
    //     }
    // }
    
    private void Start()
    {
        StartCountdown();
    }

    void OnEnable()
    {
        //TODO netcode : Load from database the previous crashes here and SetMultipliers of each previous crash
        for(int i = 0; i < crashHistoryMultipliersAmount; i++)
        {
            CrashPreviousMultiplier crash = Instantiate(previousCrashPrefab, crashHistoryContainer);
            crashesHistory.Enqueue(crash);
        }
    }
    
    private void Update()
    {   
        if(active) //Crash Multiplier
        {
            if(current < currentCrashMultiplier)
            {
                time += Time.deltaTime;
                current = 1 + multiplicationSpeed.Evaluate(time) * crashSpeedMultiplier;
                currentMultiplierText.text = $"x{current.ToString("0.00").Replace(',', '.')}";

                if(isWagering)
                {
                    betButtonValueText.text = Mathf.RoundToInt(wageredBet * current).ToString("0");
                }

                //Color fade
                if(colorChanged == 0 && current >= 1.95f && current < 2.95f) 
                {
                    colorChanged = 1;
                    currentMultiplierText.DOColor(GameManager.Instance.yellowAccent, 0.75f);
                }
                else if(colorChanged == 1 && current >= 2.95f) 
                {
                    colorChanged = 2;
                    currentMultiplierText.DOColor(GameManager.Instance.greenAccent, 0.75f);
                }
            }
            else 
            {
                active = false;
                StartCoroutine(CrashGameCoroutine());
            }
        }
        else if(isCountingDown) //Next crash countdown
        {
            if(countdown > 0)
            {
                countdown -= Time.deltaTime;
                countdownFillBar.fillAmount = countdown / 10;
                countdownText.text = countdown.ToString("0.00").Replace(',', '.');

                //Colorfade
                if(countdownColorChanged == 0 && countdown < 6f && countdown > 2.75f)
                {
                    countdownColorChanged = 1;
                    countdownFillBar.DOColor(GameManager.Instance.yellowAccent, 1.2f);
                }
                else if(countdownColorChanged == 1 && countdown < 2.75f)
                {
                    countdownColorChanged = 2;
                    countdownFillBar.DOColor(GameManager.Instance.redAccent, 1f);
                }
            }
            else
            {
                EndCountdown();
            }
        }
    }

    #region CRASH DISPLAY

    IEnumerator CrashGameCoroutine()
    {   
        if(isWagering)
        {
            betButton.gameObject.SetActive(false);
            inputPad.SetButtonsActive(true);
        }
        
        currentMultiplierText.text = "BOOM";
        currentMultiplierText.DOColor(boomColor, 0.1f);

        yield return new WaitForSeconds(0.6f);

        ClearAllBets();
        AddCrashToPrevious();
        StartCountdown();
    }

    public void StartCrash()
    {
        currentMultiplierText.color = Color.white;
        currentMultiplierText.text = $"x{current}";
        colorChanged = 0;
        time = 0;
        current = 0;

        GenerateCrashMultiplier();
        inputPad.Hide();
        active = true;

        
        if(isWagering)
        {
            betButton.interactable = true;
            betButtonImage.color = cashoutColor;            
            betButtonLabel.text = "Encaisser";
        }
        else
        {
            betButton.gameObject.SetActive(false);
        }
    }

    public void AddCrashToPrevious()
    {   
        CrashPreviousMultiplier crash = crashesHistory.Dequeue();
        crash.SetMultiplier(currentCrashMultiplier);
        crash.transform.SetSiblingIndex(0);
        crashesHistory.Enqueue(crash);
    }
    #endregion CRASH DISPLAY

    #region COUNTDOWN

    private void StartCountdown()
    {
        countdownColorChanged = 0;
        countdownFillBar.DOColor(GameManager.Instance.greenAccent, 0.1f);
        countdownGO.SetActive(true);
        isCountingDown = true;
        countdown = 10;
        countdownFillBar.fillAmount = 1;
        countdownFillBar.color = GameManager.Instance.greenAccent;
        
        //Reset input & bet button
        ResetBetButton();
        inputPad.UpdateBetButtonValue();
        inputPad.Show();
    }

    private void EndCountdown()
    {
        isCountingDown = false;
        countdownGO.SetActive(false);
        StartCrash();
    }

    #endregion COUNTDOWN

    #region BET/CASHOUT

    public void OnBetOrCashout() //Button
    {
        if(isWagering)
        {
            Cashout();
        }
        else 
        {
            Bet();   
        }
    }

    private void Bet()
    {
        wageredBet = inputPad.GetCurrentInputValue();
        if(wageredBet < 100 || wageredBet > UserBehaviour.i.Bananas) return;

        //GameManager.Instance.bananas -= wageredBet;
        UserBehaviour.i.AddBanana(-Mathf.RoundToInt(wageredBet));

        isWagering = true;
        inputPad.SetButtonsActive(false);
        betButton.interactable = false;
        betButtonImage.color = betDisabledColor;
        betButtonLabel.text = "Montant misé";
        GameManager.Instance.bananas -= wageredBet;
        AddPlayerBet(wageredBet);
    }

    private void Cashout()
    {
        isWagering = false;
        inputPad.SetButtonsActive(true);
        betButton.interactable = false;
        betButton.gameObject.SetActive(false);
        UserBehaviour.i.AddBanana(Mathf.RoundToInt(wageredBet * current));
        //GameManager.Instance.bananas += Mathf.RoundToInt(wageredBet * current);
        MovePlayerBetToCashout();
        wageredBet = 0;
    }

    private void ResetBetButton()
    {
        betButton.gameObject.SetActive(true);
        betButton.interactable = true;
        betButtonImage.color = betColor;
        betButtonLabel.text = "Miser";
        isWagering = false;
    }

    #endregion BET/CASHOUT

    #region BETS DISPLAY

    /// <summary>
    /// TODO netcode : Takes the player data in parameter and adds it the current bets
    /// </summary>
    public void AddPlayerBet(float bananas)
    {
        CurrentPlayerBetUI bet = Instantiate(playerBetUIPrefab, currentBetsContainer);
        bet.InitializeBet(Mathf.RoundToInt(bananas));
        totalBananasBet.text = bananas.ToString();
        currentBets.SetActive(true);
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    /// <summary>
    /// TODO netcode : Moves player's bet to the cashed bets
    /// </summary>
    public void MovePlayerBetToCashout()
    {
        Destroy(currentBetsContainer.GetChild(0).gameObject);
        CurrentPlayerCashedBetUI bet = Instantiate(playerCashedBetUIPrefab, cashedBetsContainer);
        bet.InitializeCashedBet(Mathf.RoundToInt(wageredBet * current), $"x{current.ToString("0.00").Replace(',', '.')}", UserBehaviour.i.UserName);
        currentBets.SetActive(false);
        cashedBets.SetActive(true);
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    /// <summary>
    /// Clears all bets UI
    /// </summary>
    public void ClearAllBets()
    {
        foreach(Transform t in currentBetsContainer)
            Destroy(t.gameObject);
        foreach(Transform t in cashedBetsContainer)
            Destroy(t.gameObject);

        currentBets.SetActive(false);
        cashedBets.SetActive(false);
    }

    #endregion BETS DISPLAY
    int edge;
    float sub15;
    float sub2;
    #region CRASH MATHS
    void GenerateCrashMultiplier()
    {
        ulong e = (ulong)Math.Pow(2, 32);

        // Generating a random 32-bit unsigned integer
        ulong h = GetRandomUInt32();

        // House edge
        if (h % 16 == 0) // 16 => ~6% house edge
        {
            currentCrashMultiplier = 1.00f;
            return;
        }

        float r = (float)(Math.Floor((100.0 * e - h) / (e - h)) / 100.0);

        currentCrashMultiplier = r;
    }

    ulong GetRandomUInt32()
    {
        byte[] buffer = BitConverter.GetBytes(RandomNumberGenerator.GetInt32(0, 2147483647));
        if (BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        return BitConverter.ToUInt32(buffer, 0);
    }

    #endregion CRASH MATHS
}