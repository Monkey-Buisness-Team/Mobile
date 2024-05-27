using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ValueUiText : MonoBehaviour
{
    TextMeshProUGUI _text;

    enum Type
    {
        Banana,
        MatchF1Odd,
        MatchF2Odd,
        RoundF1Odd,
        RoundF2Odd,
        Fighter1,
        Fighter2,
        UserName
    }

    [SerializeField] private Type _type;

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
                
                text = UserBehaviour.i.Bananas.ToString();
                _text.text = text;

                break;
            case Type.MatchF1Odd:

                if(BetManager.i.F1MatchOdds >= 100)
                    text = Mathf.Clamp(Mathf.RoundToInt(BetManager.i.F1MatchOdds), 1.01f, 999).ToString();
                else if (BetManager.i.F1MatchOdds >= 10)
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F1MatchOdds * 10f) / 10f), 1.01f, 999).ToString();
                else
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F1MatchOdds * 100f) / 100f), 1.01f, 999).ToString();
                _text.text = text;

                break; 
            case Type.MatchF2Odd:

                if (BetManager.i.F2MatchOdds >= 100)
                    text = Mathf.Clamp(Mathf.RoundToInt(BetManager.i.F2MatchOdds), 1.01f, 999).ToString();
                else if (BetManager.i.F2MatchOdds >= 10)
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F2MatchOdds * 10f) / 10f), 1.01f, 999).ToString();
                else
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F2MatchOdds * 100f) / 100f), 1.01f, 999).ToString();
                _text.text = text;

                break;
            case Type.RoundF1Odd:

                if (BetManager.i.F1RoundOdds >= 100)
                    text = Mathf.Clamp(Mathf.RoundToInt(BetManager.i.F1RoundOdds), 1.01f, 999).ToString();
                else if (BetManager.i.F1RoundOdds >= 10)
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F1RoundOdds * 10f) / 10f), 1.01f, 999).ToString();
                else
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F1RoundOdds * 100f) / 100f), 1.01f, 999).ToString();
                _text.text = text;

                break;
            case Type.RoundF2Odd:

                if (BetManager.i.F2RoundOdds >= 100)
                    text = Mathf.Clamp(Mathf.RoundToInt(BetManager.i.F2RoundOdds), 1.01f, 999).ToString();
                else if (BetManager.i.F2RoundOdds >= 10)
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F2RoundOdds * 10f) / 10f), 1.01f, 999).ToString();
                else
                    text = Mathf.Clamp((Mathf.RoundToInt(BetManager.i.F2RoundOdds * 100f) / 100f), 1.01f, 999).ToString();
                _text.text = text;

                break;

            case Type.Fighter1:
                _text.text = BetManager.i.F1Name;
                break;
            
            case Type.Fighter2:
                _text.text = BetManager.i.F2Name;
                break;

            case Type.UserName:
                _text.text = UserBehaviour.i.UserName;
                break;

            default:
                break;
        }
    }
}
