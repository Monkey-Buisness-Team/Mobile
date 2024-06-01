using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager i;

    private void Awake()
    {
        if (i != null)
            Destroy(i);
        i = this;
    }

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

        DisplayBananasBoard();
        DisplayWinBoard();
        Debug.Log("Update LeaderBoard");
    }

    private void UpdateAllBoard(object sender, ValueChangedEventArgs e)
    {
        UpdateAllBoard();
    }

    [SerializeField] GameObject _content;
    [SerializeField] int maxDisplay;

    [SerializeField, Header("Bananas")] Transform _bananaBoardContainer;
    [SerializeField] LeaderBoardDisplayer _bananaBoardDisplayPref;
    private List<LeaderBoardDisplayer> _bananaDisplayers = new List<LeaderBoardDisplayer>();

    private async void DisplayBananasBoard()
    {
        var dataList = await DataBase.GetReference(USER_KEY).OrderByChild("Bananas").GetValueAsync();

        foreach (var display in _bananaDisplayers)
        {
            Destroy(display.gameObject);
        }
        _bananaDisplayers.Clear();

        int i = 0;
        foreach (var data in dataList.Children.Reverse())
        {
            if (i >= maxDisplay) break;
            i++;

            var d = JsonUtility.FromJson<UserData>(data.GetRawJsonValue());

            var display = Instantiate(_bananaBoardDisplayPref, _bananaBoardContainer) as LeaderBoardDisplayerBananas;
            display.Bananas = d.Bananas;
            display.Init(UserManager.i.GetAvatar(d.AvatarID), d.UserName, i);
            _bananaDisplayers.Add(display);
        }

        if (!_bananaDisplayers.Any(x => x.UserName == UserBehaviour.i.UserName))
        {
            var display = Instantiate(_bananaBoardDisplayPref, _bananaBoardContainer) as LeaderBoardDisplayerBananas;
            display.Bananas = UserBehaviour.i.Bananas;

            var d = dataList.Children.ToList().Find(x => JsonUtility.FromJson<UserData>(x.GetRawJsonValue()).UserName == UserBehaviour.i.UserName);

            int index = -1;
            for (int y = 0; y < dataList.Children.Reverse().ToList().Count; y++)
            {
                var t = JsonUtility.FromJson<UserData>(dataList.Children.Reverse().ToList()[y].GetRawJsonValue());
                if(t.UserName == UserBehaviour.i.UserName)
                {
                    index = y + 1;
                    break;
                }
            }

            display.Init(UserManager.i.GetAvatar(UserBehaviour.i.AvatarID), UserBehaviour.i.UserName, index);
            
            _bananaDisplayers.Add(display);
        }
    }

    [SerializeField, Header("Win")] Transform _winBoardContainer;
    [SerializeField] LeaderBoardDisplayer _winBoardDisplayPref;
    private List<LeaderBoardDisplayer> _winDisplayers = new List<LeaderBoardDisplayer>();

    private async void DisplayWinBoard()
    {
        var dataList = await DataBase.GetReference(USER_KEY).OrderByChild("MatchWin").GetValueAsync();

        foreach (var display in _winDisplayers)
        {
            Destroy(display.gameObject);
        }
        _winDisplayers.Clear();

        int i = 0;
        foreach (var data in dataList.Children.Reverse())
        {
            if (i >= maxDisplay) break;
            i++;

            var d = JsonUtility.FromJson<UserData>(data.GetRawJsonValue());

            var display = Instantiate(_winBoardDisplayPref, _winBoardContainer) as LeaderBoardDisplayerValue;
            display.Value = d.MatchWin;
            display.Init(UserManager.i.GetAvatar(d.AvatarID), d.UserName, i);
            _winDisplayers.Add(display);
        }

        if (!_winDisplayers.Any(x => x.UserName == UserBehaviour.i.UserName))
        {
            var display = Instantiate(_winBoardDisplayPref, _winBoardContainer) as LeaderBoardDisplayerValue;
            display.Value = UserBehaviour.i.CurrentUserData.MatchWin;

            int index = -1;
            for (int y = 0; y < dataList.Children.Reverse().ToList().Count; y++)
            {
                var t = JsonUtility.FromJson<UserData>(dataList.Children.Reverse().ToList()[y].GetRawJsonValue());
                if (t.UserName == UserBehaviour.i.UserName)
                {
                    index = y + 1;
                    break;
                }
            }

            display.Init(UserManager.i.GetAvatar(UserBehaviour.i.AvatarID), UserBehaviour.i.UserName, index);

            _winDisplayers.Add(display);
        }
    }

    private void FixedUpdate()
    {
        GameManager.Instance.UpdateLayouts(_content.GetComponents<LayoutGroup>());
    }
}
