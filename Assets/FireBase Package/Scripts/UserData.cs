using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserData
{
    public string UserName;
    //public string UserId;
    public int AvatarID;
    public int Bananas;
    public int MatchWin;
    public int MatchPlay;
    public int NbBetWin;

    public string UserType;
}

public enum UserType
{
    None,
    Bettor,
    Fighter
}
