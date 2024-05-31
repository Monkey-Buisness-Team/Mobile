using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TotalUiText : MonoBehaviour
{
    TextMeshProUGUI _text;

    enum Type
    {
        Banana,
        Paris,
        Mises, 
        Encaissement
    }

    [SerializeField] private Type _type;
    [SerializeField] private Transform _parent;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string text = "";

        switch (_type)
        {
            case Type.Banana:
                int bananas = 0;
                for (int i = 0; i < _parent.childCount; i++)
                {
                    bananas += _parent.GetChild(i).GetComponent<CurrentPlayerBetUI>().Bananas;
                }
                text = $"<size=100%><sprite=0><size=80%>{bananas}";
                break;

            case Type.Paris:
                text = $"{_parent.childCount} Pari{(_parent.childCount > 1 ? "s" : string.Empty)}";
                break;

            case Type.Mises:
                text = $"{_parent.childCount} Mise{(_parent.childCount > 1 ? "s" : string.Empty)} en cours";
                break;

            case Type.Encaissement:
                text = $"{_parent.childCount} Encaissement{(_parent.childCount > 1 ? "s" : string.Empty)}";
                break;

            default:
                break;
        }

        _text.text = text;
    }
}
