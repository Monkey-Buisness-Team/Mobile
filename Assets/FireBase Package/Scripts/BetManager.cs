using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    private const string BET_KEY = "BET_KEY";
    private DatabaseReference BetDataBase;

    private const string FIGHTER_KEY = "FIGHTER_KEY";
    private const string FIGHTER_ONE = "FIGHTER_1";
    private const string FIGHTER_TWO = "FIGHTER_2";
    private DatabaseReference FighterDataBase;

    private const string USERBET_KEY = "USERBET_KEY";
    private const string MATCH_BET = "MATCH_BET";
    private const string ROUND_BET = "ROUND_BET";
    private DatabaseReference UserBetDataBase;

    private const string ACTIVE_KEY = "ACTIVE_KEY";
    private DatabaseReference ActiveDataBase;
    private Action OnActiveBet;
    private Action OnDeActiveBet;

    private const string ODDS_KEY = "ODDS_KEY";
    private DatabaseReference OddsDataBase;

    private const string MATCH_KEY = "MATCH_KEY";
    private DatabaseReference MatchDataBase;

    private const string ROUND_KEY = "ROUND_KEY";
    private DatabaseReference RoundDataBase;

    private Action OnDestroyAction;
    private Action OnApplicationQuitAction;

    public float MatchOdds { get; private set; }
    public float RoundOdds { get; private set; }

    private async void Start()
    {
        await Task.Run(async () =>
        {
            while (FireBaseManager.i.DataBase == null)
                await Task.Delay(50);
        });

        BetDataBase = FireBaseManager.i.DataBase.GetReference(BET_KEY);
        FighterDataBase = BetDataBase.Child(FIGHTER_KEY);
        UserBetDataBase = BetDataBase.Child(USERBET_KEY);
        ActiveDataBase = BetDataBase.Child(ACTIVE_KEY);
        OddsDataBase = BetDataBase.Child(ODDS_KEY);
        MatchDataBase = OddsDataBase.Child(MATCH_KEY);
        RoundDataBase = OddsDataBase.Child(ROUND_KEY);

        UserManager.i.OnUserUpdated += (userdata) => _userTypeText.text = userdata.UserType;

        ActiveDataBase.ValueChanged += ActiveChange;
        OnActiveBet += RegisterEvent;
        OnDeActiveBet += UnRegisterEvent;


    }
    private void OnDestroy()
    {
        if(!UserBehaviour.i.CurrentUserType.Equals(UserType.Fighter))
            UserBehaviour.i.ChangeUserType(UserType.None);

        OnDestroyAction?.Invoke();

        OnDestroyAction -= RemoveFighterOneData;
        OnDestroyAction -= RemoveFighterTwoData;

        ActiveDataBase.ValueChanged -= ActiveChange;

        MatchDataBase.ValueChanged -= OddsMatchChange;
        RoundDataBase.ValueChanged -= OddsRoundChange;
    }

    private void OnApplicationQuit()
    {
        if (!UserBehaviour.i.CurrentUserType.Equals(UserType.Fighter))
            UserBehaviour.i.ChangeUserType(UserType.None);

        OnApplicationQuitAction?.Invoke();

        OnApplicationQuitAction -= RemoveFighterOneData;
        OnApplicationQuitAction -= RemoveFighterTwoData;

    }

    void RegisterEvent()
    {
        MatchDataBase.ValueChanged += OddsMatchChange;
        RoundDataBase.ValueChanged += OddsRoundChange;
    }

    void UnRegisterEvent()
    {
        UserBehaviour.i.ChangeUserType(UserType.None);

        OnDeActiveBet -= RemoveFighterOneData;
        OnDeActiveBet -= RemoveFighterTwoData;

        MatchDataBase.ValueChanged -= OddsMatchChange;
        RoundDataBase.ValueChanged -= OddsRoundChange;
    }

    private void ActiveChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value.Equals(true))
        {
            OnActiveBet?.Invoke();
        }
        else if(value.Snapshot.Value.Equals(false))
        {
            OnDeActiveBet?.Invoke();
        }
    }

    public async Task<bool> CheckActiveBet()
    {
        DataSnapshot data = await ActiveDataBase.GetValueAsync();
        return data.Value.Equals(true);
    }

    private async Task<bool> CheckAlreadyBet(string key)
    {
        DataSnapshot data = await UserBetDataBase.Child(key).Child(UserBehaviour.i.UserName).GetValueAsync();
        return data.Exists;
    }

    private async Task<bool> CheckJoinFighter()
    {
        Task<DataSnapshot>[] tasksFighters = new Task<DataSnapshot>[2];
        tasksFighters[0] = FighterDataBase.Child(FIGHTER_ONE).GetValueAsync();
        tasksFighters[1] = FighterDataBase.Child(FIGHTER_TWO).GetValueAsync();
        await Task.WhenAll(tasksFighters);
        return !tasksFighters.ToList().TrueForAll(x => !JsonUtility.FromJson<UserFighter>(x.Result.GetRawJsonValue()).UserName.Equals(string.Empty));
    }

    private void OddsMatchChange(object sender, ValueChangedEventArgs value)
    {
        if(value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            MatchOdds = (float)d;
        }
        else if(value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            MatchOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + MatchOdds);
    }

    private void OddsRoundChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            RoundOdds = (float)d;
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            RoundOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + RoundOdds);
    }

    public async Task<bool> BetOnMatch(int banana, string fighterName)
    {
        if (!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
        {
            Debug.LogError("UserType is not Bettor");
            return false;
        }

        bool isActive = await CheckActiveBet();
        if (!isActive)
        {
            Debug.LogError("Bet is not Active");
            return false;
        }

        bool exist = await CheckAlreadyBet(MATCH_BET);
        if(exist)
        {
            Debug.LogError("Already Bet");
            return false;
        }

        UserBet userBet = new();
        userBet.BananaBet = banana;
        userBet.UserName = UserBehaviour.i.UserName;
        userBet.Odd = MatchOdds;
        userBet.FighterName = fighterName;

        UserBehaviour.i.AddBanana(-banana);

        await UserBetDataBase.Child(MATCH_BET).Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userBet));

        Debug.Log("Bet : " + userBet.BananaBet + " at Odd : " + userBet.Odd + " on " + userBet.FighterName);
        return true;
    }
    public async Task<bool> BetOnRound(int banana, string fighterName)
    {
        if (!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
        {
            Debug.LogError("UserType is not Bettor");
            return false;
        }

        bool isActive = await CheckActiveBet();
        if (!isActive)
        {
            Debug.LogError("Bet is not Active");
            return false;
        }

        bool exist = await CheckAlreadyBet(ROUND_BET);
        if (exist)
        {
            Debug.LogError("Already Bet");
            return false;
        }

        UserBet userBet = new();
        userBet.BananaBet = banana;
        userBet.UserName = UserBehaviour.i.UserName;
        userBet.Odd = RoundOdds;
        userBet.FighterName = fighterName;

        UserBehaviour.i.AddBanana(-banana);

        await UserBetDataBase.Child(ROUND_BET).Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userBet));

        return true;
    }

    private async Task<bool> EnterBetRoom(UserType userType)
    {
        bool isActive = await CheckActiveBet();
        if (!isActive)
        {
            Debug.LogError("Bet is not Active");
            return false;
        }

        switch (userType)
        {
            case UserType.Fighter:
                if (!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
                {
                    bool canAccess = await CheckJoinFighter();
                    if (canAccess)
                    {
                        await JoinBetAsFighter();
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Fighter Already Full");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Already a Bettor");
                    return false;
                }
                

            case UserType.Bettor:
                if (!UserBehaviour.i.CurrentUserType.Equals(UserType.Fighter))
                {
                    await JoinBetAsBettor();
                    return true;
                }
                else
                {
                    Debug.LogError("Already a Fighter");
                    return false;
                }
        }
        Debug.LogError("User Type is not Correct");
        return false;
    }

    private async Task JoinBetAsBettor()
    {
        UserBehaviour.i.ChangeUserType(UserType.Bettor);
    }

    private async Task JoinBetAsFighter()
    {

        Task<DataSnapshot>[] tasksFighters = new Task<DataSnapshot>[2];
        tasksFighters[0] = FighterDataBase.Child(FIGHTER_ONE).GetValueAsync();
        tasksFighters[1] = FighterDataBase.Child(FIGHTER_TWO).GetValueAsync();
        await Task.WhenAll(tasksFighters);

        if(tasksFighters.ToList().Exists(x => JsonUtility.FromJson<UserFighter>(x.Result.GetRawJsonValue()).UserName.Equals(UserBehaviour.i.UserName)))
        {
            Debug.LogError("Already Register as Fighter");
            return;
        }

        UserFighter fighter = new UserFighter();
        fighter.UserName = UserBehaviour.i.UserName;

        if (JsonUtility.FromJson<UserFighter>(tasksFighters[0].Result.GetRawJsonValue()).UserName.Equals(string.Empty))
        {
            UserBehaviour.i.ChangeUserType(UserType.Fighter);
            //OnDestroyAction += RemoveFighterOneData;
            //OnApplicationQuitAction += RemoveFighterOneData;
            OnDeActiveBet += RemoveFighterOneData;

            await FighterDataBase.Child(FIGHTER_ONE).SetRawJsonValueAsync(JsonUtility.ToJson(fighter));
        }
        else if (JsonUtility.FromJson<UserFighter>(tasksFighters[1].Result.GetRawJsonValue()).UserName.Equals(string.Empty))
        {
            UserBehaviour.i.ChangeUserType(UserType.Fighter);
            //OnDestroyAction += RemoveFighterTwoData;
            //OnApplicationQuitAction += RemoveFighterTwoData;
            OnDeActiveBet += RemoveFighterTwoData;

            await FighterDataBase.Child(FIGHTER_TWO).SetRawJsonValueAsync(JsonUtility.ToJson(fighter));
        }
        else
        {
            Debug.LogError("All Fighter's Place are Take");
            return;
        }
    }

    private void RemoveFighterOneData()
    {
        UserFighter json = new();
        json.UserName = string.Empty;
        FighterDataBase.Child(FIGHTER_ONE).SetRawJsonValueAsync(JsonUtility.ToJson(json));
        Debug.Log("Remove Data Fighter 1");
    }

    private void RemoveFighterTwoData()
    {
        UserFighter json = new();
        json.UserName = string.Empty;
        FighterDataBase.Child(FIGHTER_TWO).SetRawJsonValueAsync(JsonUtility.ToJson(json));
        Debug.Log("Remove Data Fighter 2");
    }

    [Header("Général")]
    [SerializeField]
    TextMeshProUGUI _userTypeText;

    [Header("Fighter")]

    [SerializeField]
    Button _fighterJoinButton;

    [SerializeField]
    Button _fighterOneBetButton;

    [SerializeField]
    Button _fighterTwoBetButton;

    [Header("Bettor")]

    [SerializeField]
    Button _bettorJoinButton;



    public async void TryEnterAsFighter()
    {
        _fighterJoinButton.interactable = false;

        bool isEnter = await EnterBetRoom(UserType.Fighter);
        if (!isEnter)
        {
            Debug.LogError("Cant enter as Fighter");
        }

        _fighterJoinButton.interactable = true;
    }

    public async void TryEnterAsBettor()
    {
        _bettorJoinButton.interactable = false;

        bool isEnter = await EnterBetRoom(UserType.Bettor);
        if (!isEnter)
        {
            Debug.LogError("Cant enter as Bettor");
        }

        _bettorJoinButton.interactable = true;
    }

    public async void TryBetOnFighterOne(int bananas)
    {
        _fighterOneBetButton.interactable = false;

        DataSnapshot json = await FighterDataBase.Child(FIGHTER_ONE).GetValueAsync();
        var data = JsonUtility.FromJson<UserFighter>(json.GetRawJsonValue());
        if (data.UserName.Equals(string.Empty))
        {
            Debug.LogError("Fighter dont Exist");
            _fighterOneBetButton.interactable = true;
            return;
        }

        bool hasBet = await BetOnMatch(bananas, data.UserName);
        if (!hasBet)
        {
            Debug.LogError("You dont Bet");
            _fighterOneBetButton.interactable = true;
            return;
        }

        _fighterOneBetButton.interactable = true;
    }

    public async void TryBetOnFighterTwo(int bananas)
    {
        _fighterTwoBetButton.interactable = false;

        DataSnapshot json = await FighterDataBase.Child(FIGHTER_TWO).GetValueAsync();
        var data = JsonUtility.FromJson<UserFighter>(json.GetRawJsonValue());
        if (data.UserName.Equals(string.Empty))
        {
            Debug.LogError("Fighter dont Exist");
            _fighterTwoBetButton.interactable = true;
            return;
        }

        bool hasBet = await BetOnMatch(bananas, data.UserName);
        if (!hasBet)
        {
            Debug.LogError("You dont Bet");
            _fighterTwoBetButton.interactable = true;
            return;
        }

        _fighterTwoBetButton.interactable = true;
    }
}
