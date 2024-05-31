using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UserManager : MonoBehaviour
{
    public static UserManager i;

    private void Awake()
    {
        if (i == null)
            i = this;
        else
            Destroy(this.gameObject);
    }

    private const string SAVE_KEY = "SAVE_KEY";
    private const string USER_KEY = "USER_KEY";

    [SerializeField]
    private UserBehaviour _userBehaviour;
    public UserData LastUserData { get; private set; }
    public Action<UserData> OnUserUpdated;
    public DatabaseReference UserDataBaseRef => _userDataBaseRef;
    private DatabaseReference _userDataBaseRef;

    public Action OnUserLogin;
    public UnityEvent OnUserLoginUE;

    public Sprite[] UserAvatars => _userAvatars;
    [SerializeField] Sprite[] _userAvatars;

    private void Start()
    {
        FireBaseManager.i.OnFireBaseInit += Init;
        OnUserLogin += () => _registerPage.SetActive(false);
    }

    private async void Init()
    {
        OnUserUpdated += HandleUserSaveUpdated;

        if(PlayerPrefs.HasKey(SAVE_KEY))
        {
            bool exist = await SaveExist(PlayerPrefs.GetString(SAVE_KEY));
            if(exist)
            {
                _registerInputField.interactable = false;
                _registerButton.interactable = false;
                Task task = LogInUser(PlayerPrefs.GetString(SAVE_KEY));
                await task;
                _registerInputField.interactable = true;
                _registerButton.interactable = true;
                return;
            }
        }
    }

    public void ErasePlayerPref()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteAll();
    }

    public async Task SaveUserData(UserData userData)
    {
        if (!userData.Equals(LastUserData))
            await FireBaseManager.i.DataBase.GetReference(USER_KEY).Child(userData.UserName).SetRawJsonValueAsync(JsonUtility.ToJson(userData));
    }

    public async Task<UserData?> LoadUserData(string username)
    {
        var dataSnapshot = await FireBaseManager.i.DataBase.GetReference(USER_KEY).Child(username).GetValueAsync();
        if (!dataSnapshot.Exists)
        {
            return null;
        }
        return JsonUtility.FromJson<UserData>(dataSnapshot.GetRawJsonValue());
    }

    public async Task<bool> SaveExist(string key)
    {
        DataSnapshot dataSnapshot = await FireBaseManager.i.DataBase.GetReference(USER_KEY).Child(key).GetValueAsync();
        return dataSnapshot.Exists;
    }

    async void HandleUserUpdated()
    {
        await SaveUserData(_userBehaviour.CurrentUserData);
    }

    void HandleUserSaveUpdated(UserData userData)
    {
        _userBehaviour.UpdateUser(userData);
    }

    void HandleValueChange(object sender, ValueChangedEventArgs value)
    {
        var json = value.Snapshot.GetRawJsonValue();
        if (!string.IsNullOrEmpty(json))
        {
            var playerData = JsonUtility.FromJson<UserData>(json);
            LastUserData = playerData;
            OnUserUpdated?.Invoke(playerData);
        }
    }

    private void OnDestroy()
    {
        if(_userDataBaseRef != null)
            _userDataBaseRef.ValueChanged -= HandleValueChange;
        _userDataBaseRef = null;
        if (UserBehaviour.i.CurrentUserType == UserType.Bettor)
            UserBehaviour.i.ChangeUserType(UserType.None);
    }

    private void OnApplicationQuit()
    {
        if (UserBehaviour.i.CurrentUserType == UserType.Bettor)
            UserBehaviour.i.ChangeUserType(UserType.None);
    }

    public async Task LogInUser(string username, bool remember = false)
    {
        bool exist = await SaveExist(username);

        if (!exist)
        {
            Debug.LogError("User dont Exist");
            return;
        }

        _userDataBaseRef = FireBaseManager.i.DataBase.GetReference(USER_KEY).Child(username);
        _userDataBaseRef.ValueChanged += HandleValueChange;

        UserData? d = await LoadUserData(username);

        if (d == null)
        {
            Debug.LogError("UserData is null");
            return;
        }

        if (remember)
        {
            string key = username;
            PlayerPrefs.SetString(SAVE_KEY, key);
            PlayerPrefs.Save();
        }

        UserData data = d.GetValueOrDefault();
        _userBehaviour.OnUserUpdated += HandleUserUpdated;
        _userBehaviour.UpdateUser(data);
        OnUserLogin?.Invoke();
        OnUserLoginUE?.Invoke();
        Debug.Log($"User {data.UserName} is LogIn");
    }

    public async Task RegisterUser(string username, bool remember = false, int avatarID = 0)
    {
        if (!username.All(x => char.IsLetterOrDigit(x)))
        {
            Debug.LogError("Spécial Char Detect");
            return;
        }

        bool exist = await SaveExist(username);

        if (exist)
        {
            Debug.LogError("User already Exist");
            return;
        }

        UserData userData = new();

        //Default UserData Config
        userData.UserName = username;
        userData.Bananas = 5000;
        userData.AvatarID = avatarID;
        userData.NbBetWin = 0;
        userData.UserType = "None";

        await SaveUserData(userData);
        _userBehaviour.UpdateUser(userData);
        await LogInUser(username, remember);
    }

    [Header("Register")]
    [SerializeField]
    GameObject _registerPage;

    [SerializeField]
    TMP_InputField _registerInputField;

    [SerializeField]
    Button _registerButton;

    [SerializeField] Image _registerAvatar;

    public async void TryRegister()
    {
        _registerInputField.interactable = false;
        _registerButton.interactable = false;
        bool remember = true;
        string username = _registerInputField.text;

        bool exist = await SaveExist(username);
        if (exist)
        {
            _registerInputField.interactable = true;
            _registerButton.interactable = true;
            Debug.LogError("User already Exist");
            return;
        }

        await RegisterUser(username, remember, GetId(_registerAvatar.sprite));
        _registerInputField.interactable = true;
        _registerButton.interactable = true;
    }

    public int GetId(Sprite sprite) => _userAvatars.ToList().IndexOf(sprite);
    public Sprite GetAvatar() => _userAvatars[UserBehaviour.i.AvatarID];
    public async Task<Sprite> GetAvatar(string username)
    {
        if (FireBaseManager.i == null) return null;
        var data = await FireBaseManager.i.DataBase.GetReference(USER_KEY).Child(username).Child("AvatarID").GetValueAsync();
        Debug.Log(data.Value);

        float id = -1;
        if (data.Value is double)
        {
            double d = (double)data.Value;
            id = (float)d;
        }
        else if (data.Value is Int64)
        {
            Int64 i = (Int64)data.Value;
            id = (float)i;
        }

        if (id >= _userAvatars.Length || id < 0) return null;

        return _userAvatars[Mathf.RoundToInt(id)];
    }
}
