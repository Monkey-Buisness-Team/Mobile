using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserBet
{
    public string UserName;
    public string FighterName;
    public int BananaBet;
    public float Odd;
    public int AvatarId;
}

[System.Serializable]
public struct UserFighter
{
    public string UserName;
}

[System.Serializable]
public struct CrashBet
{
    public string UserName;
    public int AvatarId;
    public int BananaBet;
    public string State;
    public float Odd;
}
