using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetAvatar : MonoBehaviour
{
    public void SetCurrentAvatar()
    {
        DisplayAllAvatar.Instance.Validate(GetComponent<Image>().sprite);
    }
}
