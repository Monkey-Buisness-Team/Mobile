using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardDisplayerValue : LeaderBoardDisplayer
{
    [SerializeField] TextMeshProUGUI valueText;
    public int Value { get; set; }

    public override void Init(Sprite a, string name, int rank)
    {
        base.Init(a, name, rank);
        valueText.text = $"{Value}";
    }
}
