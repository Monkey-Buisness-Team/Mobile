using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnValueBool : MonoBehaviour
{
    public enum Value
    {
        ACTIVE_KEY,
        MATCH_BET_KEY,
        ROUND_BET_KEY
    }

    [SerializeField] Value _value;
    [SerializeField] UnityEvent<bool> OnValueChange;

    public async void Start()
    {
        switch (_value)
        {
            case Value.ACTIVE_KEY:
                FirebaseAutorisationManager.i.RoomIsOpen.AddListener(OnValueChange.Invoke);
                OnValueChange?.Invoke(await FirebaseAutorisationManager.i.IsRoomOpen());
                break;
            case Value.MATCH_BET_KEY:
                FirebaseAutorisationManager.i.MatchBetIsOpen.AddListener(OnValueChange.Invoke);
                OnValueChange?.Invoke(await FirebaseAutorisationManager.i.IsMatchBetOpen());
                break;
            case Value.ROUND_BET_KEY:
                FirebaseAutorisationManager.i.RoundBetIsOpen.AddListener(OnValueChange.Invoke);
                OnValueChange?.Invoke(await FirebaseAutorisationManager.i.IsRoundBetOpen());
                break;
            default:
                break;
        }
        
    }
}
