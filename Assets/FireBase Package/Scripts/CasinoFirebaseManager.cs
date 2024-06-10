using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using static OnValueBool;

public class CasinoFirebaseManager : MonoBehaviour
{
    public static CasinoFirebaseManager i;

    private void Awake()
    {
        if (i != null)
            Destroy(i);
        i = this;
    }

    private const string CASINO_KEY = "CASINO_KEY";
    private DatabaseReference CasinoDataBase;

    private const string SEED_KEY = "SEED_KEY";
    private DatabaseReference SeedDataBase;

    private const string ADMIN_KEY = "ADMIN_KEY";
    private DatabaseReference AdminDataBase;

    private const string CRASH_KEY = "Crash";
    private DatabaseReference CrashDataBase;

    public ulong Seed { get; private set; }
    public bool IsAdmin { get; private set; }
    public float CrashTimer { get; private set; }

    private void Start()
    {
        FireBaseManager.i.OnFireBaseInit += Init;
        UserManager.i.OnUserLogin += StartAdminWork;
    }

    private void Init()
    {
        CasinoDataBase = FireBaseManager.i.DataBase.GetReference(CASINO_KEY);
        SeedDataBase = CasinoDataBase.Child(SEED_KEY);
        AdminDataBase = CasinoDataBase.Child(ADMIN_KEY);
        CrashDataBase = CasinoDataBase.Child(CRASH_KEY);

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
        SeedDataBase.ValueChanged += SeedChange;

        CrashDataBase.Child("Timer").ValueChanged += CrashTimerChange;
        CrashDataBase.Child("Bet").ChildAdded += OnCrashBetAdd;
    }

    void UnRegisterEvent()
    {
        SeedDataBase.ValueChanged -= SeedChange;
        CrashDataBase.Child("Timer").ValueChanged -= CrashTimerChange;
        CrashDataBase.Child("Bet").ChildAdded -= OnCrashBetAdd;

        if (IsAdmin)
        {
            SetCrashTimer(-1);
        }
    }

    private void SeedChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            Seed = (ulong)(float)d;
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            Seed = (ulong)(float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + Seed);
    }

    async void StartAdminWork()
    {
        IsAdmin = await IsAdminUser();

        if (!IsAdmin) return;

        Debug.Log($"Is Admin : {IsAdmin}");
    }

    public async Task<bool> IsAdminUser()
    {
        var data = await AdminDataBase.GetValueAsync();
        if(data.Value is string)
        {
            return (string)data.Value == UserBehaviour.i.UserName;
        }
        return false;
    }

    public async void SetSeed(float seed) => await SeedDataBase.SetValueAsync(seed);
    public async Task<ulong> GetSeed()
    {
        var data = await SeedDataBase.GetValueAsync();
        if(data.Value is double)
        {
            //Debug.Log($"{(ulong)(float)data.Value}");
            return (ulong)((float)data.Value);
        }
        return 1;
    }

    #region Crash

    public CrashBet? _playerCrashBet = null;
    public enum CrashState
    {
        Miser,
        Encaisser
    }

    public async void SetCrashTimer(float timer) => await CrashDataBase.Child("Timer").SetValueAsync(timer);
    public async Task<float> GetCrashTimer()
    {
        var data = await CrashDataBase.Child("Timer").GetValueAsync();
        if (data.Value is double)
        {
            return (float)data.Value;
        }
        return 1;
    }
    private void CrashTimerChange(object sender, ValueChangedEventArgs value)
    {
        if (value.Snapshot.Value is double)
        {
            double d = (double)value.Snapshot.Value;
            CrashTimer = (float)d;
        }
        else if (value.Snapshot.Value is Int64)
        {
            Int64 i = (Int64)value.Snapshot.Value;
            CrashTimer = (float)i;
        }
        //Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + CrashTimer);
    }

    public async void AddBetCrash(int banana)
    {
        CrashBet bet = new CrashBet();
        bet.UserName = UserBehaviour.i.UserName;
        bet.AvatarId = UserBehaviour.i.AvatarID;
        bet.State = CrashState.Miser.ToString();
        bet.BananaBet = banana;
        bet.Odd = 0;

        await CrashDataBase.Child("Bet").Child(UserBehaviour.i.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(bet));
        _playerCrashBet = bet;
    }

    private void OnCrashBetAdd(object sender, ChildChangedEventArgs e)
    {
        var data = e.Snapshot;
        var json = data.GetRawJsonValue();
        var bet = JsonUtility.FromJson<CrashBet>(json);

        if (bet.UserName.Equals("Default")) return;

        Crash.Instance.AddPlayerBet(bet.BananaBet, bet.UserName);
        if(bet.State == CrashState.Miser.ToString())
        {
            CrashDataBase.Child("Bet").Child(bet.UserName).ValueChanged += HandleCrashBetChange;
            CrashDataBase.Child("Bet").ChildRemoved += (o, e) =>
            {
                var d = e.Snapshot;
                var j = d.GetRawJsonValue();
                var b = JsonUtility.FromJson<CrashBet>(j);

                if(b.UserName == bet.UserName)
                {
                    CrashDataBase.Child("Bet").Child(b.UserName).ValueChanged -= HandleCrashBetChange;
                }
            };
            Application.quitting += () => CrashDataBase.Child("Bet").Child(bet.UserName).ValueChanged -= HandleCrashBetChange;
        }
        else if(bet.State == CrashState.Encaisser.ToString())
        {
            Crash.Instance.MovePlayerBetToCashout(bet.UserName, bet.Odd, bet.BananaBet);
        }
    }

    public async void MoveCrashBet(float odd)
    {
        await CrashDataBase.Child("Bet").Child(UserBehaviour.i.UserName).Child("Odd").SetValueAsync(odd);
        await CrashDataBase.Child("Bet").Child(UserBehaviour.i.UserName).Child("State").SetValueAsync(CrashState.Encaisser.ToString());
    }

    private void HandleCrashBetChange(object sender, ValueChangedEventArgs value)
    {
        var data = value.Snapshot;
        var json = data.GetRawJsonValue();

        if (json == string.Empty || json == null) return;

        var bet = JsonUtility.FromJson<CrashBet>(json);

        if(bet.State == CrashState.Encaisser.ToString())
        {
            Crash.Instance.MovePlayerBetToCashout(bet.UserName, bet.Odd, bet.BananaBet);
        }
    }

    public async Task RemoveAllCrashBet()
    {
        var dataList = await CrashDataBase.Child("Bet").GetValueAsync();

        //foreach(var data in dataList.Children)
        //{
        //    Debug.Log(data.Key);
        //    await CrashDataBase.Child("Bet").Child(data.Key).RemoveValueAsync();
        //}
    }

    public async Task RemoveCrashBet()
    {
        await CrashDataBase.Child("Bet").Child(_playerCrashBet.Value.UserName).RemoveValueAsync();
        _playerCrashBet = null;
    }

    #endregion
}
