using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrentPlayerCashedBetUI : CurrentPlayerBetUI
{
    [SerializeField] TextMeshProUGUI cashedMultiplier;

    /// <summary>
    /// TODO netcode : Takes the player data in parameter and fill the current bet UI with it
    /// </summary>
    public async void InitializeCashedBet(int bananas, string multiplier, string userName)
    {
        betBananas.text = GameManager.GetBananas(bananas);
        cashedMultiplier.text = multiplier;
        playerName.text = userName;
        avatar.sprite = await UserManager.i.GetAvatar(userName);
        Bananas = bananas;
    }
}
