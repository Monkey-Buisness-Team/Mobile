using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOnClick : MonoBehaviour
{
    private void ButtonClicked()
    {
        SoundManager.SharedInstance.playSoundWithId("click");
    }

    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(ButtonClicked);
        }
    }

    void OnDestroy()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(ButtonClicked);
        }
    }
}
