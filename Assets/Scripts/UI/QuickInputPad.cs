using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class QuickInputPad : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI betBtnValue;

    public void UpdateBetButtonValue()
    {
        // if(!Crash.Instance.isWagering) betBtnValue.text = inputField.text;
        betBtnValue.text = inputField.text;
    }

    public float GetCurrentInputValue()
    {
        float value;
        if(float.TryParse(inputField.text, out value))
            return value;
        return 0;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetButtonsActive(bool param)
    {
        //GetComponentsInChildren<Button>().ForEach(x => x.interactable = param);
        foreach (var item in GetComponentsInChildren<Button>())
        {
            item.interactable = param;
        }
    }

    #region Inputs

    public void QuickInputButtonAddition(float value)
    {
        if(UserBehaviour.i.Bananas < float.Parse(inputField.text) + value) inputField.text = UserBehaviour.i.Bananas.ToString();
        else inputField.text = (float.Parse(inputField.text) + value).ToString();
    }

    public void QuickInputButtonMultiplication(float value)
    {
        if(float.Parse(inputField.text) == 0) return;

        if(UserBehaviour.i.Bananas < float.Parse(inputField.text) * value) inputField.text = UserBehaviour.i.Bananas.ToString();
        else inputField.text = Mathf.RoundToInt(float.Parse(inputField.text) * value).ToString();
    }

    public void QuickInputButtonSplit()
    {
        if(float.Parse(inputField.text) < 2) return;

        inputField.text = Math.Round(float.Parse(inputField.text) / 2).ToString();
    }

    public void QuickInputButtonClear()
    {
        inputField.text = "0";
    }

    public void QuickInputButtonMax()
    {
        inputField.text = UserBehaviour.i.Bananas.ToString();
    }

    #endregion Inputs

}
