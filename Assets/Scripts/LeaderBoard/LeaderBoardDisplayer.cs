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

    [SerializeField] protected Color userColor;

    public string UserName { get; private set; }

    public virtual void Init(Sprite a, string name, int rank)
    {
        if(name == UserBehaviour.i.UserName)
        {
            playerName.color = userColor;
            rankText.color = userColor;
        }

        UserName = name;

        avatar.sprite = a;
        playerName.text = name;
        rankText.text = $"{rank}.";
    }
}
