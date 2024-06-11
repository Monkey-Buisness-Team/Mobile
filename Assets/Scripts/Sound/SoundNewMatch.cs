using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundNewMatch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FirebaseAutorisationManager.i.RoomIsOpen.AddListener(TriggerNewMatchSound);
    }

    void TriggerNewMatchSound(bool roomIsOpen)
    {
        if (roomIsOpen)
            SoundManager.SharedInstance.playSoundWithId("combatStart");
    }
}
