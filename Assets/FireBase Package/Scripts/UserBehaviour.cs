using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserBehaviour : MonoBehaviour
{
    public static UserBehaviour i;

    private void Awake()
    {
        if(i == null)
            i = this;
        else
            Destroy(this.gameObject);
    }

    public UserData CurrentUserData => _userData;
    [SerializeField]
    private UserData _userData;

    public string UserName => _userData.UserName;
    public int Bananas => _userData.Bananas;
    public int AvatarID => _userData.AvatarID;
    public int NbBetWin => _userData.NbBetWin;
    public UserType CurrentUserType => ConvertStringToUserType(_userData.UserType);

    public Action OnUserUpdated;

    public void AddBanana(int value)
    {
        _userData.Bananas += value;
        OnUserUpdated?.Invoke();
    }

    public void ChangeName(string name)
    {
        _userData.UserName = name;
        OnUserUpdated?.Invoke();
    }

    public void ChangeAvatar(int id)
    {
        _userData.AvatarID = id;
        OnUserUpdated?.Invoke();
    }

    public void ChangeUserType(UserType userType)
    {
        _userData.UserType = userType.ToString();
        OnUserUpdated?.Invoke();
    }

    UserType ConvertStringToUserType(string value)
    {
        switch (value)
        {
            case "None":
                return UserType.None;
            case "Bettor":
                return UserType.Bettor;
            case "Fighter":
                return UserType.Fighter;
            default:
                return UserType.None;
        }
    }

    public void UpdateUser(UserData userData)
    {
        if (!userData.Equals(CurrentUserData))
        {
            _userData = userData;
            OnUserUpdated?.Invoke();
        }
    }
}
