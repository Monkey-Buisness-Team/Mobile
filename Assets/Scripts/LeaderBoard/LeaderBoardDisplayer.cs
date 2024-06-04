using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderBoardDisplayer : MonoBehaviour
{
    [SerializeField] protected Image avatar;
    [SerializeField] protected TextMeshProUGUI playerName;
    [SerializeField] protected TextMeshProUGUI rankText;
    [SerializeField] protected Image bg;
    [SerializeField] protected TMP_SpriteAsset spriteAsset;

    public string UserName { get; private set; }

    public virtual void Init(Sprite a, string name, int rank, Color textColor, Color bgColor, TMP_SpriteAsset asset)
    {
        UserName = name;

        bg.color = bgColor;
        playerName.color = textColor;
        rankText.color = textColor;

        avatar.sprite = a;
        playerName.text = name;
        rankText.text = $"{rank}";
    }
}
