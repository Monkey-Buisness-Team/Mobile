using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    private const string USER_KEY = "USER_KEY";
    private FirebaseDatabase DataBase;
    public void Start()
    {
        FireBaseManager.i.OnFireBaseInit += UpdateAllBoard;
        FireBaseManager.i.OnFireBaseInit += Init;
    }

    private void Init()
    {
        DataBase = FireBaseManager.i.DataBase;
        DataBase.GetReference(USER_KEY).ValueChanged += UpdateAllBoard;
        UserBehaviour.i.OnUserUpdated += UpdateAllBoard;
    }

    private void OnDestroy()
    {
        FireBaseManager.i.OnFireBaseInit -= UpdateAllBoard;
        FireBaseManager.i.OnFireBaseInit -= Init;
        UserBehaviour.i.OnUserUpdated -= UpdateAllBoard;
        DataBase.GetReference(USER_KEY).ValueChanged -= UpdateAllBoard;
    }

    private void OnApplicationQuit()
    {
        FireBaseManager.i.OnFireBaseInit -= UpdateAllBoard;
        FireBaseManager.i.OnFireBaseInit -= Init;
        UserBehaviour.i.OnUserUpdated -= UpdateAllBoard;
        DataBase.GetReference(USER_KEY).ValueChanged -= UpdateAllBoard;
    }

    private void UpdateAllBoard()
    {
        if (FireBaseManager.i == null || DataBase == null) return;

        DisplayBoard();
    }

    private void UpdateAllBoard(object sender, ValueChangedEventArgs e)
    {
        UpdateAllBoard();
    }

    public enum LeaderBoardType
    {
        Banana,
        Pari,
        Combat
    }
    [SerializeField] private LeaderBoardType type;

    [SerializeField] GameObject _content;
    [SerializeField] string dataKey;

    [SerializeField] Transform _boardContainer;
    [SerializeField] LeaderBoardDisplayerValue _boardDisplayPref;
    private List<LeaderBoardDisplayerValue> _displayers = new List<LeaderBoardDisplayerValue>();
    private LeaderBoardDisplayerValue _userDisplay;
    [SerializeField] Transform _userContainer;

    [SerializeField] Color _bgColor;
    [SerializeField] Color _textColor;
    [SerializeField] TMP_SpriteAsset _spriteAsset;

    private async void DisplayBoard()
    {
        var list = _displayers;
        var dataList = await DataBase.GetReference(USER_KEY).OrderByChild(dataKey).GetValueAsync();

        foreach (var display in _displayers)
        {
            if(display == null) continue;
            Destroy(display.gameObject);
        }
        _displayers.Clear();

        Color bgColor = new Color(0.15f, 0.15f, 0.15f);
        Color textColor = Color.white;

        int index = -1; 
        int i = 0;
        foreach (var data in dataList.Children.Reverse())
        {
            i++;

            var d = JsonUtility.FromJson<UserData>(data.GetRawJsonValue());

            var display = Instantiate(_boardDisplayPref, _boardContainer);

            switch (type)
            {
                case LeaderBoardType.Banana:
                    display.Value = GameManager.GetBananas(d.Bananas);
                    break;

                case LeaderBoardType.Pari:
                    display.Value = d.NbBetWin.ToString();
                    break;

                case LeaderBoardType.Combat:
                    display.Value = d.MatchWin.ToString();
                    break;
            }

            bgColor = new Color(0.15f, 0.15f, 0.15f);
            textColor = Color.white;

            if(d.UserName == UserBehaviour.i.UserName)
            {
                bgColor = new Color(0.24f, 0.24f, 0.24f);
                index = i;
            }
            else if(i <= 3)
            {
                bgColor = _bgColor;
                textColor = _textColor;
            }
            
            display.Init(UserManager.i.GetAvatar(d.AvatarID), d.UserName, i, textColor, bgColor, _spriteAsset);
            _displayers.Add(display);
        }

        var dis = _userDisplay;
        if (_userDisplay != null)
        {
            Destroy(_userDisplay.gameObject);
        }

        bgColor = new Color(0.24f, 0.24f, 0.24f);
        textColor = Color.white;

        _userDisplay = Instantiate(_boardDisplayPref, _userContainer);
        //_userDisplay.transform.SetAsFirstSibling();

        switch (type)
        {
            case LeaderBoardType.Banana:
                _userDisplay.Value = GameManager.GetBananas(UserBehaviour.i.Bananas);
                break;

            case LeaderBoardType.Pari:
                _userDisplay.Value = UserBehaviour.i.NbBetWin.ToString();
                break;

            case LeaderBoardType.Combat:
                _userDisplay.Value = UserBehaviour.i.MatchWin.ToString();
                break;
        }

        _userDisplay.Init(UserManager.i.GetAvatar(), UserBehaviour.i.UserName, index, textColor, bgColor, _spriteAsset);
    }

    private void FixedUpdate()
    {
        GameManager.Instance.UpdateLayouts(_content.GetComponents<LayoutGroup>());
    }
}
