using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAllAvatar : MonoBehaviour
{
    public static DisplayAllAvatar Instance;
    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    [SerializeField] GameObject _avatarDisplay;
    [SerializeField] Transform _displayParent;

    [SerializeField] Image _avatarImage;

    public void Start()
    {
        for (int i = 0; i < UserManager.i.UserAvatars.Length; i++)
        {
            var display = Instantiate(_avatarDisplay, _displayParent);
            var image = display.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            image.sprite = UserManager.i.UserAvatars[i];

            i++;

            if(i < UserManager.i.UserAvatars.Length)
            {
                var image2 = display.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
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
    }
}
