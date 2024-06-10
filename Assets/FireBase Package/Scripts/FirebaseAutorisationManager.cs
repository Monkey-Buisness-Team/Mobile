using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseAutorisationManager : MonoBehaviour
{
    public static FirebaseAutorisationManager i;

    private void Awake()
    {
        if (i != null)
            Destroy(i);
        i = this;
    }

    private const string BET_KEY = "BET_KEY";
    private DatabaseReference BetDataBase;

    private const string ACTIVE_KEY = "ACTIVE_KEY";
    private DatabaseReference ActiveDataBase;

    private const string ROUND_BET_OPEN = "ROUND_BET_OPEN";
    private DatabaseReference RoundBetOpenDataBase;
    private const string MATCH_BET_OPEN = "MATCH_BET_OPEN";
    private DatabaseReference MatchBetOpenDataBase;

    private const string USERBET_KEY = "USERBET_KEY";
    private const string MATCH_BET = "MATCH_BET";
    private const string ROUND_BET = "ROUND_BET";
    private DatabaseReference UserBetDataBase;

    public UnityEvent<bool> RoomIsOpen;
    public UnityEvent<bool> RoundBetIsOpen;
    public UnityEvent<bool> MatchBetIsOpen;

    public Action OnRoomChange;

    private void Start()
    {
        FireBaseManager.i.OnFireBaseInit += Init;
    }

    private void Init()
    {
        BetDataBase = FireBaseManager.i.DataBase.GetReference(BET_KEY);
        ActiveDataBase = BetDataBase.Child(ACTIVE_KEY);
        RoundBetOpenDataBase = BetDataBase.Child(ROUND_BET_OPEN);
        MatchBetOpenDataBase = BetDataBase.Child(MATCH_BET_OPEN);
        UserBetDataBase = BetDataBase.Child(USERBET_KEY);
        
        RoomIsOpen.AddListener((value) => 
        {
            if (!value)
                UserBehaviour.i.ChangeUserType(UserType.None);
            //Debug.Log(UserBehaviour.i.CurrentUserType);
        });


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
        ActiveDataBase.ValueChanged += OnFightRoomOpenClose;
        RoundBetOpenDataBase.ValueChanged += OnRoundBetOpenClose;
        MatchBetOpenDataBase.ValueChanged += OnMatchBetOpenClose;
    }

    void UnRegisterEvent()
    {
        ActiveDataBase.ValueChanged -= OnFightRoomOpenClose;
        RoundBetOpenDataBase.ValueChanged -= OnRoundBetOpenClose;
        MatchBetOpenDataBase.ValueChanged -= OnMatchBetOpenClose;
    }

    private void OnFightRoomOpenClose(object sender, ValueChangedEventArgs value)
    {
        if(value.Snapshot.Value is bool)
        {
            OnRoomChange?.Invoke();
            RoomIsOpen?.Invoke((bool)value.Snapshot.Value);
            //Debug.Log((bool)value.Snapshot.Value);
        }
        else
        {
            Debug.LogError("Not a Bool");
        }
    }

    private void OnRoundBetOpenClose(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is bool)
        {
            OnRoomChange?.Invoke();
            RoundBetIsOpen?.Invoke((bool)value.Snapshot.Value);
            //Debug.Log((bool)value.Snapshot.Value);
        }
        else
        {
            Debug.LogError("Not a Bool");
        }
    }

    private void OnMatchBetOpenClose(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is bool)
        {
            OnRoomChange?.Invoke();
            MatchBetIsOpen?.Invoke((bool)value.Snapshot.Value);
            //Debug.Log((bool)value.Snapshot.Value);
        }
        else
        {
            Debug.LogError("Not a Bool");
        }
    }

    public async Task<bool> IsRoomOpen()
    {
        if(ActiveDataBase == null) return false;
        DataSnapshot data = await ActiveDataBase.GetValueAsync();
        if (data.Value is bool)
        {
            return (bool)data.Value;
        }
        return false;
    }

    public async Task<bool> IsRoundBetOpen()
    {
        if (RoundBetOpenDataBase == null) return false;
        DataSnapshot data = await RoundBetOpenDataBase.GetValueAsync();
        if(data.Value is bool)
        {
            return (bool)data.Value;
        }
        return false;
    }

    public async Task<bool> IsMatchBetOpen()
    {
        if (MatchBetOpenDataBase == null) return false;
        DataSnapshot data = await MatchBetOpenDataBase.GetValueAsync();
        if (data.Value is bool)
        {
            return (bool)data.Value;
        }
        return false;
    }

    public async Task<bool> CheckAlreadyBet(string key)
    {
        DataSnapshot data = await UserBetDataBase.Child(key).Child(UserBehaviour.i.UserName).GetValueAsync();
        return data.Exists;
    }

}
