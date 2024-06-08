using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardDisplayerValue : LeaderBoardDisplayer
{
    [SerializeField] TextMeshProUGUI valueText;
    public string Value { get; set; }

    public override void Init(Sprite a, string name, int rank, Color textColor, Color bgColor, TMP_SpriteAsset asset)
    {
        base.Init(a, name, rank, textColor, bgColor, asset);

        valueText.spriteAsset = asset;
        valueText.color = textColor;
        valueText.text = $"<sprite=0>{Value}";
    }
}
