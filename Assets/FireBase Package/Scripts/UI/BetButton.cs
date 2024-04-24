using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BetButton : MonoBehaviour
{
    [SerializeField] TMP_InputField _bananaInput;
    [SerializeField] bool isBetOnMatch;
    [SerializeField] bool isBetOnFirstFighter;

    public async void OnPress()
    {
        int banana = int.Parse(_bananaInput.text);
        string fighterName = await BetManager.i.GetFighterName(isBetOnFirstFighter);

        if (isBetOnMatch)
        {
            await BetManager.i.BetOnMatch(banana, fighterName);
            return;
        }
        else if(!isBetOnMatch)
        {
            await BetManager.i.BetOnRound(banana, fighterName);
            return;
        }

    }
}
