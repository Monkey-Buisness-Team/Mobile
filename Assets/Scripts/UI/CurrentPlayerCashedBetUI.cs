using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrentPlayerCashedBetUI : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI cashedBananas;
    [SerializeField] TextMeshProUGUI cashedMultiplier;

    /// <summary>
    /// TODO netcode : Takes the player data in parameter and fill the current bet UI with it
    /// </summary>
    public void InitializeCashedBet(int bananas, string multiplier)
    {
        cashedBananas.text = bananas.ToString();
        cashedMultiplier.text = multiplier;
    }
}
