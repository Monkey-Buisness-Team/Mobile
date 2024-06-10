using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
using System.Data;

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
    private List<CurrentPlayerBetUI> bets = new List<CurrentPlayerBetUI>();

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
        //if (!UserManager.i.UserIsLogin) return;

        //if(active) //Crash Multiplier
        //{
        //    if(current < currentCrashMultiplier)
        //    {
        //        time += Time.deltaTime;
        //        current = 1 + multiplicationSpeed.Evaluate(time) * crashSpeedMultiplier;
        //        currentMultiplierText.text = $"x{current.ToString("0.00").Replace(',', '.')}";

        //        if(isWagering)
        //        {
        //            betButtonValueText.text = Mathf.RoundToInt(wageredBet * current).ToString("0");
        //        }

        //        //Color fade
        //        if(colorChanged == 0 && current >= 1.95f && current < 2.95f) 
        //        {
        //            colorChanged = 1;
        //            currentMultiplierText.DOColor(GameManager.Instance.yellowAccent, 0.75f);
        //        }
        //        else if(colorChanged == 1 && current >= 2.95f) 
        //        {
        //            colorChanged = 2;
        //            currentMultiplierText.DOColor(GameManager.Instance.greenAccent, 0.75f);
        //        }
        //    }
        //    else 
        //    {
        //        active = false;
        //        StartCoroutine(CrashGameCoroutine());
        //    }
        //}
        //else if(isCountingDown) //Next crash countdown
        //{
        //    if(countdown > 0)
        //    {
        //        countdown -= Time.deltaTime;
        //        countdownFillBar.fillAmount = countdown / 10;
        //        countdownText.text = countdown.ToString("0.00").Replace(',', '.');

        //        //Colorfade
        //        if(countdownColorChanged == 0 && countdown < 6f && countdown > 2.75f)
        //        {
        //            countdownColorChanged = 1;
        //            countdownFillBar.DOColor(GameManager.Instance.yellowAccent, 1.2f);
        //        }
        //        else if(countdownColorChanged == 1 && countdown < 2.75f)
        //        {
        //            countdownColorChanged = 2;
        //            countdownFillBar.DOColor(GameManager.Instance.redAccent, 1f);
        //        }

        //        CasinoFirebaseManager.i.SetCrashTimer(countdown);
        //    }
        //    else
        //    {
        //        CasinoFirebaseManager.i.SetCrashTimer(-1);
        //        EndCountdown();
        //    }
        //}
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

    IEnumerator CountDownTimer()
    {
        while (!UserManager.i.UserIsLogin)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSecondsRealtime(0.5f);


        if (CasinoFirebaseManager.i.IsAdmin)
        {
            GenerateCrashMultiplier();
            countdown = 10;
            int i = 0;
            while (countdown > 0)
            {
                countdown -= 0.03333f;
                countdownFillBar.fillAmount = countdown / 10;
                countdownText.text = countdown.ToString("0.00").Replace(',', '.');

                //Colorfade
                if (countdownColorChanged == 0 && countdown < 6f && countdown > 2.75f)
                {
                    countdownColorChanged = 1;
                    countdownFillBar.DOColor(GameManager.Instance.yellowAccent, 1.2f);
                }
                else if (countdownColorChanged == 1 && countdown < 2.75f)
                {
                    countdownColorChanged = 2;
                    countdownFillBar.DOColor(GameManager.Instance.redAccent, 1f);
                }


                i++;
                if(i >= 10)
                {
                    i = 0;
                    CasinoFirebaseManager.i.SetCrashTimer(countdown);
                }
                yield return null;
            }

            CasinoFirebaseManager.i.SetCrashTimer(-1);
            EndCountdown();
        }
        else
        {
            while(CasinoFirebaseManager.i.CrashTimer < 0)
            {
                countdown = 10;
                countdownFillBar.fillAmount = countdown / 10;
                countdownText.text = countdown.ToString("0.00").Replace(',', '.');

                betButton.interactable = false;
                betButtonImage.color = betDisabledColor;
                betButtonLabel.text = "En attente du serveur";

                yield return new WaitForSeconds(0.01f);
            }

            betButton.gameObject.SetActive(true);
            betButton.interactable = true;
            betButtonImage.color = betColor;
            betButtonLabel.text = "Miser";
            countdown = CasinoFirebaseManager.i.CrashTimer;
            Debug.Log(countdown);

            while (countdown > 0)
            {
                countdown -= 0.03333f;
                countdownFillBar.fillAmount = countdown / 10;
                countdownText.text = countdown.ToString("0.00").Replace(',', '.');

                //Colorfade
                if (countdownColorChanged == 0 && countdown < 6f && countdown > 2.75f)
                {
                    countdownColorChanged = 1;
                    countdownFillBar.DOColor(GameManager.Instance.yellowAccent, 1.2f);
                }
                else if (countdownColorChanged == 1 && countdown < 2.75f)
                {
                    countdownColorChanged = 2;
                    countdownFillBar.DOColor(GameManager.Instance.redAccent, 1f);
                }

                yield return null;
            }

            GenerateCrashMultiplier();
            EndCountdown();
        }
    }

    IEnumerator CrashActive()
    {
        while (current < currentCrashMultiplier)
        {
            time += 0.03333f;
            current = 1 + multiplicationSpeed.Evaluate(time) * crashSpeedMultiplier;
            currentMultiplierText.text = $"x{current.ToString("0.00").Replace(',', '.')}";

            if (isWagering)
            {
                betButtonValueText.text = Mathf.RoundToInt(wageredBet * current).ToString("0");
            }

            //Color fade
            if (colorChanged == 0 && current >= 1.95f && current < 2.95f)
            {
                colorChanged = 1;
                currentMultiplierText.DOColor(GameManager.Instance.yellowAccent, 0.75f);
            }
            else if (colorChanged == 1 && current >= 2.95f)
            {
                colorChanged = 2;
                currentMultiplierText.DOColor(GameManager.Instance.greenAccent, 0.75f);
            }

            yield return null;
        }

        StartCoroutine(CrashGameCoroutine());
    }

    public void StartCrash()
    {
        currentMultiplierText.color = Color.white;
        currentMultiplierText.text = $"x{current}";
        colorChanged = 0;
        time = 0;
        current = 0;

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

        StartCoroutine(CrashActive());
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
        countdown = 10;
        countdownFillBar.fillAmount = 1;
        countdownFillBar.color = GameManager.Instance.greenAccent;
        
        //Reset input & bet button
        ResetBetButton();
        inputPad.UpdateBetButtonValue();
        inputPad.Show();

        StartCoroutine(CountDownTimer());
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
        if(wageredBet > UserBehaviour.i.Bananas) return;

        UserBehaviour.i.AddBanana(-Mathf.RoundToInt(wageredBet));

        isWagering = true;
        inputPad.SetButtonsActive(false);
        betButton.interactable = false;
        betButtonImage.color = betDisabledColor;
        betButtonLabel.text = "Montant misé";

        CasinoFirebaseManager.i.AddBetCrash(Mathf.RoundToInt(wageredBet));
        //AddPlayerBet(wageredBet);
    }

    private void Cashout()
    {
        isWagering = false;
        inputPad.SetButtonsActive(true);
        betButton.interactable = false;
        betButton.gameObject.SetActive(false);
        UserBehaviour.i.AddBanana(Mathf.RoundToInt(wageredBet * current));

        CasinoFirebaseManager.i.MoveCrashBet(current);

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
    public void AddPlayerBet(float bananas, string userName)
    {
        CurrentPlayerBetUI bet = Instantiate(playerBetUIPrefab, currentBetsContainer);
        bet.InitializeCrashBet(Mathf.RoundToInt(bananas), userName);
        totalBananasBet.text = bananas.ToString();
        currentBets.SetActive(true);
        bets.Add(bet);
        GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
    }

    /// <summary>
    /// TODO netcode : Moves player's bet to the cashed bets
    /// </summary>
    public void MovePlayerBetToCashout(string username, float odd, int bananas)
    {
        var b = bets.Find(x => x.UserName == username);
        if(b != null)
        {
            bets.Remove(b);
            if(bets.Count <= 0)
                currentBets.SetActive(false);
            Destroy(b.gameObject);
            CurrentPlayerCashedBetUI bet = Instantiate(playerCashedBetUIPrefab, cashedBetsContainer);
            bet.InitializeCashedBet(Mathf.RoundToInt(bananas * odd), $"x{odd.ToString("0.00").Replace(',', '.')}", username);

            cashedBets.SetActive(true);
            GameManager.Instance.UpdateLayouts(GetComponentsInChildren<LayoutGroup>());
        }
    }

    /// <summary>
    /// Clears all bets UI
    /// </summary>
    public async void ClearAllBets()
    {
        foreach(Transform t in currentBetsContainer)
            Destroy(t.gameObject);
        foreach(Transform t in cashedBetsContainer)
            Destroy(t.gameObject);

        if (CasinoFirebaseManager.i.IsAdmin)
        {
            await CasinoFirebaseManager.i.RemoveAllCrashBet();
        }

        if (CasinoFirebaseManager.i._playerCrashBet != null)
            await CasinoFirebaseManager.i.RemoveCrashBet();

        bets.Clear();
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
        ulong h = CasinoFirebaseManager.i.Seed;

        if (CasinoFirebaseManager.i.IsAdmin)
        {
            h = GetRandomUInt32();
            CasinoFirebaseManager.i.SetSeed(h);
            Debug.Log($"{(float)h} | {h}");

            // House edge
            if (h % 16 == 0) // 16 => ~6% house edge
            {
                currentCrashMultiplier = 1.00f;
                return;
            }
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