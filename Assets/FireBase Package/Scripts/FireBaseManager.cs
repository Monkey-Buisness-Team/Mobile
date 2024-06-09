using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager i;

    private void Awake()
    {
        if(i != null)
            Destroy(i);
        i = this;
    }

    public Action OnFireBaseInit;
    public UnityEvent OnFireBaseInitEvent;
    public bool IsConnected { get; private set; } = false;

    [Header("DataBase")]
    private FirebaseDatabase _database;
    public FirebaseDatabase DataBase => _database;
    void SetDataBase() => _database = FirebaseDatabase.DefaultInstance;

    public void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError("Failed to init FireBase : " + task.Exception);
                return;
            }

            Debug.Log("Sucess to init FireBase");
            SetDataBase();
            IsConnected = true;
            OnFireBaseInit?.Invoke();
            OnFireBaseInitEvent?.Invoke();
        });
    }

    private void OnDestroy()
    {
        _database = null;
    }

}
