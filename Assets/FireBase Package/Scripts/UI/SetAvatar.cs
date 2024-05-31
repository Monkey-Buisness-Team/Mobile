using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetAvatar : MonoBehaviour
{
    public DisplayAllAvatar displayManager {  get; set; }
    public void SetCurrentAvatar()
    {
        displayManager.Validate(GetComponent<Image>().sprite);
    }
}
