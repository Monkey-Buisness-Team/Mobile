using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardDisplayerBananas : LeaderBoardDisplayer
{
    [SerializeField] TextMeshProUGUI bananasText;
    public int Bananas { get; set; }

    public override void Init(Sprite a, string name, int rank)
    {
        base.Init(a, name, rank);
        bananasText.text = $"<sprite=0>{Bananas}";
    }
}
