using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPlayerBetUI : MonoBehaviour
{
    [SerializeField] protected Image avatar;
    [SerializeField] protected TextMeshProUGUI playerName;
    [SerializeField] protected TextMeshProUGUI betBananas;

    public int Bananas { get; protected set; }
    public BetType Type { get; protected set; }
    
    /// <summary>
    /// TODO netcode : Takes the player data in parameter and fill the current bet UI with it
    /// </summary>
    public void InitializeBet(int bananas)
    {
        betBananas.text = bananas.ToString();
        playerName.text = UserBehaviour.i.UserName;
        avatar.sprite = UserManager.i.GetAvatar();
        Bananas = bananas;
    }

    public async void InitializeBet(int bananas, string userName, BetType betType)
    {
        betBananas.text = bananas.ToString();
        playerName.text = userName;
        avatar.sprite = await UserManager.i.GetAvatar(userName);
        Bananas = bananas;
        Type = betType;
    }
}
