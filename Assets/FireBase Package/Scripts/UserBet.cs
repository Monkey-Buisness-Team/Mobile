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
