using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("DataBase")]
    private FirebaseDatabase _database;
    public FirebaseDatabase DataBase => _database;
    void SetDataBase() => _database = FirebaseDatabase.DefaultInstance;

    public void Start()
    {
        OnFireBaseInit += SetDataBase;
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

            OnFireBaseInit?.Invoke();
        });
    }

    private void OnDestroy()
    {
        _database = null;
    }

}
