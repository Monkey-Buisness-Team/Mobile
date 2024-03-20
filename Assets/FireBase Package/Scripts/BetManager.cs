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
    private DatabaseReference MatchDataBase;

    private const string ROUND_KEY = "ROUND_KEY";
    private DatabaseReference RoundDataBase;

    public float MatchOdds { get; private set; }
    public float RoundOdds { get; private set; }

    public UnityEvent OnEnterChoice;
    public UnityEvent OnJoinAction;

    private async void Start()
    {
        FireBaseManager.i.OnFireBaseInit += Init;
    }

    private void Init()
    {
        BetDataBase = FireBaseManager.i.DataBase.GetReference(BET_KEY);
        FighterDataBase = BetDataBase.Child(FIGHTER_KEY);
        UserBetDataBase = BetDataBase.Child(USERBET_KEY);
        ActiveDataBase = BetDataBase.Child(ACTIVE_KEY);
        OddsDataBase = BetDataBase.Child(ODDS_KEY);
        MatchDataBase = OddsDataBase.Child(MATCH_KEY);
        RoundDataBase = OddsDataBase.Child(ROUND_KEY);

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
        MatchDataBase.ValueChanged += OddsMatchChange;
        RoundDataBase.ValueChanged += OddsRoundChange;
    }

    void UnRegisterEvent()
    {
        MatchDataBase.ValueChanged -= OddsMatchChange;
        RoundDataBase.ValueChanged -= OddsRoundChange;
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
        if(await CheckIfFighterNull(FIGHTER_ONE) || await CheckIfFighterNull(FIGHTER_TWO))
        {
            Debug.LogError("No Place for Fighter");
            JoinAsBettor();
            return;
        }

        string key = await CheckIfFighterNull(FIGHTER_ONE) ? FIGHTER_TWO : FIGHTER_ONE;
        await FighterDataBase.Child(key).SetValueAsync(UserBehaviour.i.UserName);

        UserBehaviour.i.ChangeUserType(UserType.Fighter);
        OnJoinAction?.Invoke();
    }

    void JoinAsBettor()
    {
        UserBehaviour.i.ChangeUserType(UserType.Bettor);
        OnJoinAction?.Invoke();
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
        if(value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            RoundOdds = (float)d;
        }
        else if(value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            RoundOdds = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + RoundOdds);
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
        userBet.Odd = MatchOdds;
        userBet.FighterName = fighterName;

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
        userBet.Odd = RoundOdds;
        userBet.FighterName = fighterName;

        UserBehaviour.i.AddBanana(-banana);

        await UserBetDataBase.Child(ROUND_BET).Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userBet));
        //Debug.Log("Bet : " + userBet.BananaBet + " at Odd : " + userBet.Odd + " on " + userBet.FighterName);
        return true;
    }

    #region Utils

    async Task<bool> CheckIfFighterNull(string key)
    {
        DataSnapshot dataSnapshot = await FighterDataBase.Child(key).GetValueAsync();
        return (dataSnapshot.Value as string) != string.Empty;
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
        return !tasksFighters.ToList().TrueForAll(x => !JsonUtility.FromJson<UserFighter>(x.Result.GetRawJsonValue()).UserName.Equals(string.Empty));
    }

    #endregion
}
