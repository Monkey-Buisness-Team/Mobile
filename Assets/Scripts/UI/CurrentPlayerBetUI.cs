using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPlayerBetUI : MonoBehaviour
{
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI betBananas;
    
    /// <summary>
    /// TODO netcode : Takes the player data in parameter and fill the current bet UI with it
    /// </summary>
    public void InitializeBet(int bananas)
    {
        betBananas.text = bananas.ToString();
    }
}
