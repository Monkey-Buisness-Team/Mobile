using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{

    public static EventSystem Instance;
    private void Awake() 
    {
        if(Instance) 
            Destroy(Instance);
        Instance = this;
    }

    public static Action OnBetToggleSelect;
}
