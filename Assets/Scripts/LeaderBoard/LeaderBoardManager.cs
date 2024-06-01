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
    public void Start()
    {
        FireBaseManager.i.OnFireBaseInit += UpdateAllBoard;
        FireBaseManager.i.OnFireBaseInit += Init;
    }

    private void Init()
    {
        FireBaseManager.i.DataBase.GetReference(USER_KEY).ValueChanged += (s, e) => UpdateAllBoard();
    }

    private void UpdateAllBoard()
    {
        DisplayBananasBoard();
        DisplayWinBoard();
        Debug.Log("Update LeaderBoard");
    }

    [SerializeField] int maxDisplay;

    [SerializeField, Header("Bananas")] Transform _bananaBoardContainer;
    [SerializeField] LeaderBoardDisplayer _bananaBoardDisplayPref;
    private List<LeaderBoardDisplayer> _bananaDisplayers = new List<LeaderBoardDisplayer>();

    private async void DisplayBananasBoard()
    {
        var dataList = await FireBaseManager.i.DataBase.GetReference(USER_KEY).OrderByChild("Bananas").GetValueAsync();

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
    }

    [SerializeField, Header("Win")] Transform _winBoardContainer;
    [SerializeField] LeaderBoardDisplayer _winBoardDisplayPref;
    private List<LeaderBoardDisplayer> _winDisplayers = new List<LeaderBoardDisplayer>();

    private async void DisplayWinBoard()
    {
        var dataList = await FireBaseManager.i.DataBase.GetReference(USER_KEY).OrderByChild("MatchWin").GetValueAsync();

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
    }

    private void FixedUpdate()
    {
        GameManager.Instance.UpdateLayouts(GetComponents<LayoutGroup>());
    }
}
