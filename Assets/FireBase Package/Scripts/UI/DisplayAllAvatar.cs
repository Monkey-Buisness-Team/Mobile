using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DisplayAllAvatar : MonoBehaviour
{
    [SerializeField] GameObject _avatarDisplay;
    [SerializeField] Transform _displayParent;

    [SerializeField] Image _avatarImage;
    public UnityEvent<int> OnAvatarSelected;

    public void Start()
    {
        for (int i = 0; i < UserManager.i.UserAvatars.Length; i++)
        {
            var display = Instantiate(_avatarDisplay, _displayParent);
            var image = display.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            image.GetComponent<SetAvatar>().displayManager = this;
            image.sprite = UserManager.i.UserAvatars[i];

            i++;

            if(i < UserManager.i.UserAvatars.Length)
            {
                var image2 = display.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
                image2.GetComponent<SetAvatar>().displayManager = this;
                image2.sprite = UserManager.i.UserAvatars[i];
            }
            else
            {
                Destroy(display.transform.GetChild(1).gameObject);
            }
        }
    }

    public void Validate(Sprite sprite)
    {
        _avatarImage.sprite = sprite;
        this.gameObject.SetActive(false);
        OnAvatarSelected?.Invoke(UserManager.i.GetId(sprite));
    }
}
