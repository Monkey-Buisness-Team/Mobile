using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    void UnRegisterEvent()
    {
        SeedDataBase.ValueChanged -= SeedChange;
        CrashDataBase.Child("Timer").ValueChanged -= CrashTimerChange;

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
        Debug.Log(value.Snapshot.Value.GetType().ToString() + " : " + value.Snapshot.Value.ToString() + " | " + Seed);
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
            Debug.Log($"{(ulong)(float)data.Value}");
            return (ulong)((float)data.Value);
        }
        return 1;
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
}
