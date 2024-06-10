using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    public static BetManager i;

    private void Awake()
    {
        if (i != null)
            Destroy(i);
        i = this;
    }

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

    private const string ODDS_KEY = "ODDS_KEY";
    private DatabaseReference OddsDataBase;

    private const string MATCH_KEY = "MATCH_KEY";
    private const string ROUND_KEY = "ROUND_KEY";

    private const string SCORE_KEY = "SCORE_KEY";
    private DatabaseReference ScoreDataBase;

    public float F1MatchOdds { get; private set; }
    public float F1RoundOdds { get; private set; }

    public float F2MatchOdds { get; private set; }
    public float F2RoundOdds { get; private set; }

    public string F1Name { get; private set; } = string.Empty;
    public string F2Name { get; private set; } = string.Empty;

    public int F1Score { get; private set; } = 0;
    public int F2Score { get; private set; } = 0;

    public UnityEvent OnEnterChoice;
    public UnityEvent OnJoinAction;
    public UnityEvent OnJoinAsBettor;
    public UnityEvent OnJoinAsFighter;
    public UnityEvent OnResetRoom;

    public Action<string, string> OnFighterChange;
    public Action<Team, float, string, BetType> OnBetReceive;
    public Action<BetType> OnClearBet;

    private void Start()
    {
        FireBaseManager.i.OnFireBaseInit += Init;
        OnClearBet += (type) => F1Score = 0;
        OnClearBet += (type) => F2Score = 0;
    }

    private void Init()
    {
        BetDataBase = FireBaseManager.i.DataBase.GetReference(BET_KEY);
        FighterDataBase = BetDataBase.Child(FIGHTER_KEY);
        UserBetDataBase = BetDataBase.Child(USERBET_KEY);
        ActiveDataBase = BetDataBase.Child(ACTIVE_KEY);
        OddsDataBase = BetDataBase.Child(ODDS_KEY);
        ScoreDataBase = BetDataBase.Child(SCORE_KEY);

        RegisterEvent();
    }

    private void OnDestroy()
    {
        UnRegisterEvent();
    }

    private void OnApplicationQuit()
    {
        UnRegisterEvent();
    }

    void RegisterEvent()
    {
        OddsDataBase.Child(FIGHTER_ONE).Child(MATCH_KEY).ValueChanged += OddsF1MatchChange;
        OddsDataBase.Child(FIGHTER_ONE).Child(ROUND_KEY).ValueChanged += OddsF1RoundChange;

        OddsDataBase.Child(FIGHTER_TWO).Child(MATCH_KEY).ValueChanged += OddsF2MatchChange;
        OddsDataBase.Child(FIGHTER_TWO).Child(ROUND_KEY).ValueChanged += OddsF2RoundChange;

        FighterDataBase.Child(FIGHTER_ONE).ValueChanged += OnFighter1Change;
        FighterDataBase.Child(FIGHTER_TWO).ValueChanged += OnFighter2Change;

        ScoreDataBase.Child(FIGHTER_ONE).ValueChanged += OnScoreFighter1Change;
        ScoreDataBase.Child(FIGHTER_TWO).ValueChanged += OnScoreFighter2Change;

        UserBetDataBase.Child(MATCH_BET).ChildAdded += OnMatchBetAdd;
        UserBetDataBase.Child(MATCH_BET).ChildRemoved += OnMatchBetRemove;
        UserBetDataBase.Child(ROUND_BET).ChildAdded += OnRoundBetAdd;
        UserBetDataBase.Child(ROUND_BET).ChildRemoved += OnRoundBetRemove;

        OnFighterChange += async (f1, f2) =>
        {
            if(UserBehaviour.i.CurrentUserType == UserType.Fighter && f1 != UserBehaviour.i.UserName && f2 != UserBehaviour.i.UserName && await CheckActiveBet())
            {
                JoinAsBettor();
            }
        };

        FirebaseAutorisationManager.i.RoomIsOpen.AddListener((value) =>
        {
            if (!value)
            {
                OnResetRoom?.Invoke();
            }
        });
    }

    void UnRegisterEvent()
    {
        OddsDataBase.Child(FIGHTER_ONE).Child(MATCH_KEY).ValueChanged -= OddsF1MatchChange;
        OddsDataBase.Child(FIGHTER_ONE).Child(ROUND_KEY).ValueChanged -= OddsF1RoundChange;

        OddsDataBase.Child(FIGHTER_TWO).Child(MATCH_KEY).ValueChanged -= OddsF2MatchChange;
        OddsDataBase.Child(FIGHTER_TWO).Child(ROUND_KEY).ValueChanged -= OddsF2RoundChange;

        FighterDataBase.Child(FIGHTER_ONE).ValueChanged -= OnFighter1Change;
        FighterDataBase.Child(FIGHTER_TWO).ValueChanged -= OnFighter2Change;

        ScoreDataBase.Child(FIGHTER_ONE).ValueChanged -= OnScoreFighter1Change;
        ScoreDataBase.Child(FIGHTER_TWO).ValueChanged -= OnScoreFighter2Change;

        UserBetDataBase.Child(MATCH_BET).ChildAdded -= OnMatchBetAdd;
        UserBetDataBase.Child(MATCH_BET).ChildRemoved -= OnMatchBetRemove;
        UserBetDataBase.Child(ROUND_BET).ChildAdded -= OnRoundBetAdd;
        UserBetDataBase.Child(ROUND_BET).ChildRemoved -= OnRoundBetRemove;
    }

    private async Task<bool> EnterBetRoom(UserType userType)
    {
        bool isActive = await CheckActiveBet();
        if(!isActive)
        {
            Debug.LogError("Bet is not Active");
            return false;
        }

        switch(userType)
        {
            case UserType.Fighter:

                if(!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
                {
                    bool canAccess = await CheckJoinFighter();
                    if(canAccess)
                    {
                        await JoinAsFighter();
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Fighter Already Full");
                        JoinAsBettor();
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Already a Bettor");
                    return false;
                }

            case UserType.Bettor:

                if(!UserBehaviour.i.CurrentUserType.Equals(UserType.Fighter))
                {
                    JoinAsBettor();
                    return true;
                }
                else
                {
                    Debug.LogError("Already a Fighter");
                    return false;
                }

            default:
                if (await CheckIfFighterEqualUser(FIGHTER_ONE) || await CheckIfFighterEqualUser(FIGHTER_TWO))
                {
                    await JoinAsFighter();
                    return true;
                }

                if (await CheckIfFighterNull(FIGHTER_ONE) && await CheckIfFighterNull(FIGHTER_TWO))
                {
                    Debug.LogError("No Place for Fighter");
                    JoinAsBettor();
                    return true;
                }
                break;
        }

        Debug.LogError("User Type is not Correct");
        return false;
    }

    public async void OnTryJoin()
    {
        if(await EnterBetRoom(UserBehaviour.i.CurrentUserType)) return;

        OnEnterChoice?.Invoke();
    }

    public async void TryToJoinAsFighter()
    {
        await JoinAsFighter();
    }

    public void TryToJoinAsBettor()
    {
        JoinAsBettor();
    }

    async Task JoinAsFighter()
    {
        if(await CheckIfFighterEqualUser(FIGHTER_ONE) || await CheckIfFighterEqualUser(FIGHTER_TWO))
        {
            UserBehaviour.i.ChangeUserType(UserType.Fighter);
            OnJoinAction?.Invoke();
            OnJoinAsFighter?.Invoke();
            return;
        }

        if(await CheckIfFighterNull(FIGHTER_ONE) && await CheckIfFighterNull(FIGHTER_TWO))
        {
            Debug.LogError("No Place for Fighter");
            JoinAsBettor();
            return;
        }

        string key = await CheckIfFighterNull(FIGHTER_ONE) ? FIGHTER_TWO : FIGHTER_ONE;
        await FighterDataBase.Child(key).SetValueAsync(UserBehaviour.i.UserName);

        UserBehaviour.i.ChangeUserType(UserType.Fighter);
        OnJoinAction?.Invoke();
        OnJoinAsFighter?.Invoke();
    }

    void JoinAsBettor()
    {
        UserBehaviour.i.ChangeUserType(UserType.Bettor);
        OnJoinAction?.Invoke();
        OnJoinAsBettor?.Invoke();
    }

    private void OddsF1MatchChange(object sender, ValueChangedEventArgs value)
    {
        if(value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F1MatchOdds = (float)d;
        }
        else if(value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F1MatchOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + F1MatchOdds);
    }

    private void OddsF2MatchChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F2MatchOdds = (float)d;
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F2MatchOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + F2MatchOdds);
    }

    private void OddsF1RoundChange(object sender, ValueChangedEventArgs value)
    {
        if(value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F1RoundOdds = (float)d;
        }
        else if(value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F1RoundOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + F1RoundOdds);
    }

    private void OddsF2RoundChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F2RoundOdds = (float)d;
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F2RoundOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + F2RoundOdds);
    }

    private void OnScoreFighter1Change(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F1Score = Mathf.RoundToInt((float)d);
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F1Score = Mathf.RoundToInt((float)i);
        }
    }

    private void OnScoreFighter2Change(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            F2Score = Mathf.RoundToInt((float)d);
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            F2Score = Mathf.RoundToInt((float)i);
        }
    }

    public async Task<bool> BetOnMatch(int banana, string fighterName)
    {
        if(!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
        {
            Debug.LogError("UserType is not Bettor");
            return false;
        }

        bool isActive = await CheckActiveBet();
        if(!isActive)
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

        if(UserBehaviour.i.Bananas < banana)
        {
            Debug.LogError("Not Enough Banana");
            return false;
        }

        UserBet userBet = new();
        userBet.BananaBet = banana;
        userBet.UserName = UserBehaviour.i.UserName;
        userBet.Odd = await GetFighterName(true) == fighterName ? F1MatchOdds : F2MatchOdds;
        userBet.FighterName = fighterName;
        userBet.AvatarId = UserBehaviour.i.AvatarID;

        UserBehaviour.i.AddBanana(-banana);

        await UserBetDataBase.Child(MATCH_BET).Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userBet));
        //Debug.Log("Bet : " + userBet.BananaBet + " at Odd : " + userBet.Odd + " on " + userBet.FighterName);
        return true;
    }

    public async Task<bool> BetOnRound(int banana, string fighterName)
    {
        if(!UserBehaviour.i.CurrentUserType.Equals(UserType.Bettor))
        {
            Debug.LogError("UserType is not Bettor");
            return false;
        }

        bool isActive = await CheckActiveBet();
        if(!isActive)
        {
            Debug.LogError("Bet is not Active");
            return false;
        }

        bool exist = await CheckAlreadyBet(ROUND_BET);
        if(exist)
        {
            Debug.LogError("Already Bet");
            return false;
        }

        if(UserBehaviour.i.Bananas < banana)
        {
            Debug.LogError("Not Enough Banana");
            return false;
        }

        UserBet userBet = new();
        userBet.BananaBet = banana;
        userBet.UserName = UserBehaviour.i.UserName;
        userBet.Odd = await GetFighterName(true) == fighterName ? F1RoundOdds : F2RoundOdds;
        userBet.FighterName = fighterName;
        userBet.AvatarId = UserBehaviour.i.AvatarID;

        UserBehaviour.i.AddBanana(-banana);

        await UserBetDataBase.Child(ROUND_BET).Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userBet));
        //Debug.Log("Bet : " + userBet.BananaBet + " at Odd : " + userBet.Odd + " on " + userBet.FighterName);
        return true;
    }

    private async void OnMatchBetAdd(object sender, ChildChangedEventArgs e)
    {
        var data = e.Snapshot;
        var json = data.GetRawJsonValue();
        var bet = JsonUtility.FromJson<UserBet>(json);

        if (bet.UserName.Equals("Default")) return;

        //Debug.Log($"ADD [Bettor : {bet.UserName} | Fighter : {bet.FighterName} | Bananas : {bet.BananaBet} | Odd : {bet.Odd}]");
        Team t = await GetFighterName(true) == bet.FighterName ? Team.Red : Team.Blue;
        OnBetReceive?.Invoke(t, bet.BananaBet, bet.UserName, BetType.Match);
    }

    private void OnMatchBetRemove(object sender, ChildChangedEventArgs e)
    {
        var data = e.Snapshot;
        var json = data.GetRawJsonValue();
        var bet = JsonUtility.FromJson<UserBet>(json);

        if (bet.UserName.Equals("Default")) return;

        //Debug.Log($"REMOVE [Bettor : {bet.UserName} | Fighter : {bet.FighterName} | Bananas : {bet.BananaBet} | Odd : {bet.Odd}]");
        OnClearBet?.Invoke(BetType.Match);
    }

    private async void OnRoundBetAdd(object sender, ChildChangedEventArgs e)
    {
        var data = e.Snapshot;
        var json = data.GetRawJsonValue();
        var bet = JsonUtility.FromJson<UserBet>(json);

        if (bet.UserName.Equals("Default")) return;

        //Debug.Log($"ADD [Bettor : {bet.UserName} | Fighter : {bet.FighterName} | Bananas : {bet.BananaBet} | Odd : {bet.Odd}]");
        Team t = await GetFighterName(true) == bet.FighterName ? Team.Red : Team.Blue;
        OnBetReceive?.Invoke(t, bet.BananaBet, bet.UserName, BetType.Round);
    }

    private void OnRoundBetRemove(object sender, ChildChangedEventArgs e)
    {
        var data = e.Snapshot;
        var json = data.GetRawJsonValue();
        var bet = JsonUtility.FromJson<UserBet>(json);

        if (bet.UserName.Equals("Default")) return;

        //Debug.Log($"REMOVE [Bettor : {bet.UserName} | Fighter : {bet.FighterName} | Bananas : {bet.BananaBet} | Odd : {bet.Odd}]");
        OnClearBet?.Invoke(BetType.Round);
    }

    private void OnFighter1Change(object sender, ValueChangedEventArgs value)
    {
        F1Name = (string)value.Snapshot.Value;
        OnFighterChange?.Invoke(F1Name, F2Name);
    }

    private void OnFighter2Change(object sender, ValueChangedEventArgs value)
    {
        F2Name = (string)value.Snapshot.Value;
        OnFighterChange?.Invoke(F1Name, F2Name);
    }

    #region Utils

    async Task<bool> CheckIfFighterNull(string key)
    {
        DataSnapshot dataSnapshot = await FighterDataBase.Child(key).GetValueAsync();
        //Debug.Log((dataSnapshot.Value as string) != string.Empty);
        return (dataSnapshot.Value as string) != string.Empty;
    }

    async Task<bool> CheckIfFighterEqualUser(string key)
    {
        DataSnapshot dataSnapshot = await FighterDataBase.Child(key).GetValueAsync();
        //Debug.Log((dataSnapshot.Value as string) != string.Empty);
        return (dataSnapshot.Value as string) == UserBehaviour.i.UserName;
    }

    async Task<bool> CheckActiveBet()
    {
        DataSnapshot data = await ActiveDataBase.GetValueAsync();
        return data.Value.Equals(true);
    }

    async Task<bool> CheckAlreadyBet(string key)
    {
        DataSnapshot data = await UserBetDataBase.Child(key).Child(UserBehaviour.i.UserName).GetValueAsync();
        return data.Exists;
    }

    async Task<bool> CheckJoinFighter()
    {
        Task<DataSnapshot>[] tasksFighters = new Task<DataSnapshot>[2];
        tasksFighters[0] = FighterDataBase.Child(FIGHTER_ONE).GetValueAsync();
        tasksFighters[1] = FighterDataBase.Child(FIGHTER_TWO).GetValueAsync();
        await Task.WhenAll(tasksFighters);
        DataSnapshot[] datas = new DataSnapshot[2];
        datas[0] = tasksFighters[0].Result;
        datas[1] = tasksFighters[1].Result;

        string f1 = null;
        string f2 = null;

        if (datas[0].Exists)
             f1 = datas[0].Value as string;
        if (datas[1].Exists)
             f2 = datas[1].Value as string;

        return f1 == UserBehaviour.i.UserName || f2 == UserBehaviour.i.UserName;
        
        //return tasksFighters.ToList().Exists(x => JsonUtility.FromJson<UserFighter>(x.Result.GetRawJsonValue()).UserName == UserBehaviour.i.UserName);
    }

    public async Task<string> GetFighterName(bool first)
    {
        DataSnapshot dataSnapshot = await FighterDataBase.Child(first ? FIGHTER_ONE : FIGHTER_TWO).GetValueAsync();
        //Debug.Log(dataSnapshot.Value as string);
        return dataSnapshot.Value as string;
    }

    #endregion
}
