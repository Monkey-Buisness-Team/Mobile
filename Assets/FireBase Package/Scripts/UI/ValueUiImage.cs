using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ValueUiImage : MonoBehaviour
{
    public enum Type
    {
        CurrentUser,
        Fighter1,
        Fighter2
    }
    [SerializeField] private Type type;

    Image image;
    public void Start()
    {
        image = GetComponent<Image>();

        switch (type)
        {
            case Type.CurrentUser:
                UserBehaviour.i.OnUserUpdated += UpdateImage;
                break;

            case Type.Fighter1:
                BetManager.i.OnFighterChange += (f1, f2) => UpdateFighterImage(f1);
                UpdateFighterImage(BetManager.i.F1Name);
                break;

            case Type.Fighter2:
                BetManager.i.OnFighterChange += (f1, f2) => UpdateFighterImage(f2);
                UpdateFighterImage(BetManager.i.F2Name);
                break;
        }
    }

    private void UpdateImage()
    {
        image.sprite = UserManager.i.GetAvatar();
    }

    private async void UpdateFighterImage(string name)
    {
        if(name != string.Empty)
            image.sprite = await UserManager.i.GetAvatar(name);
    }
}
