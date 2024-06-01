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

    public virtual void Init(Sprite a, string name, int rank)
    {
        avatar.sprite = a;
        playerName.text = name;
        rankText.text = $"{rank}.";
    }
}
