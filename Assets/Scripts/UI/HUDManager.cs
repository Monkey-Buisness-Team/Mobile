using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Text Defilant")]
    [SerializeField] List<TextMeshProUGUI> _texts;
    [SerializeField] Image _background;

    [SerializeField] DisplayAllAvatar displayAllAvatar;

    public IEnumerator Start()
    {
        displayAllAvatar.OnAvatarSelected.AddListener(UserBehaviour.i.ChangeAvatar);

        for (int i = 0; i < _texts.Count; i++)
        {
            StartCoroutine(MoveText(_texts[i]));
            yield return new WaitForSeconds(2f);
        }
    }

    public void ChangeText(string text) => ChangeText(text, Color.green);

    public void ChangeText(string text, Color color)
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            _texts[i].text = text;
        }
        _background.color = color;
    }

    IEnumerator MoveText(TextMeshProUGUI text)
    {
        RectTransform rectTransform = text.transform as RectTransform;

        while (true)
        {
            if (rectTransform.localPosition.x >= 1440f)
                rectTransform.localPosition = new Vector3(-1440f, rectTransform.localPosition.y);
            //Debug.Log($"{Screen.width} | {rectTransform.localPosition}");
            yield return rectTransform.transform.DOLocalMoveX(1440f, 4f).SetEase(Ease.Linear).WaitForCompletion();
        }
    }
}
