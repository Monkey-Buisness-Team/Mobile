using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetToggle : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image icon;
    [SerializeField] Sprite unselectedSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Color backgroundColor;
    [SerializeField] Color sideColor;

    [Space(5)]
    public Team team;
    public BetType type;
    public bool selected;

    private void OnEnable() 
    {
        EventSystem.OnBetToggleSelect += Unselect;
    }

    private void OnDisable() 
    {
        EventSystem.OnBetToggleSelect -= Unselect;
    }

    public void Select()
    {
        EventSystem.OnBetToggleSelect?.Invoke();
        GameManager.Instance.paris.OnBetTypeSelected(team, type);

        background.sprite = selectedSprite;
        icon.color = backgroundColor;
        selected = true;
    }

    public void Unselect()
    {
        if(selected)
        {
            background.sprite = unselectedSprite;
            icon.color = sideColor;
            selected = false;
        }
    }
}
