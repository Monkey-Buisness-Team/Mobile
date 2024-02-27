using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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

    public Action UserLogin;

    private async void Start()
    {
        await Task.Run(async () =>
        {
            while (FireBaseManager.i.DataBase == null)
                await Task.Delay(50);
        });
        
        OnUserUpdated += HandleUserSaveUpdated;
        UserLogin += GoToUserPage;

        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            bool exist = await SaveExist(PlayerPrefs.GetString(SAVE_KEY));
            if (exist)
            {
                Task task = LogInUser(PlayerPrefs.GetString(SAVE_KEY));
                await task;
                return;
            }
        }

        _logInPage.SetActive(true);
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

    void HandleUserUpdated()
    {
        SaveUserData(_userBehaviour.CurrentUserData);
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
            string key = username.Replace("USER_", string.Empty);
            PlayerPrefs.SetString(SAVE_KEY, key);
            PlayerPrefs.Save();
        }

        UserData data = d.GetValueOrDefault();
        _userBehaviour.OnUserUpdated += HandleUserUpdated;
        _userBehaviour.UpdateUser(data);
        UserLogin?.Invoke();
    }

    public async Task RegisterUser(string username, bool remember = false, int avatarID = 0)
    {
        bool exist = await SaveExist(username);

        if (exist)
        {
            Debug.LogError("User already Exist");
            return;
        }

        UserData userData = new();

        //Default UserData Config
        userData.UserName = username;
        userData.Bananas = 1000;
        userData.AvatarID = avatarID;
        userData.NbBetWin = 0;
        userData.UserType = "None";

        await SaveUserData(userData);
        _userBehaviour.UpdateUser(userData);
        await LogInUser(username, remember);
    }

    [SerializeField]
    TextMeshProUGUI _debugText;
    public void DebugText(string text) => _debugText.text = text;

    [Header("Register")]
    [SerializeField]
    GameObject _registerPage;

    [SerializeField]
    TMP_InputField _registerInputField;

    [SerializeField]
    Button _registerButton;

    [SerializeField]
    Toggle _registerToggle;

    [Header("Log In")]
    [SerializeField]
    GameObject _logInPage;

    [SerializeField]
    TMP_InputField _logInInputField;

    [SerializeField]
    Button _logInButton;

    [SerializeField]
    Toggle _logInToggle;

    [Header("User Page")]
    [SerializeField]
    GameObject _userPage;

    [SerializeField]
    TextMeshProUGUI _nbBananaText;

    [SerializeField]
    TextMeshProUGUI _userNameText;

    public async void TryRegister()
    {
        _registerInputField.interactable = false;
        _registerToggle.interactable = false;
        _registerButton.interactable = false;
        bool remember = _registerToggle.isOn;
        string username = _registerInputField.text;

        bool exist = await SaveExist(username);
        if (exist)
        {
            _registerInputField.interactable = true;
            _registerToggle.interactable = true;
            _registerButton.interactable = true;
            return;
        }

        await RegisterUser(username, remember);
        _registerInputField.interactable = true;
        _registerToggle.interactable = true;
        _registerButton.interactable = true;
    }

    public async void TryLogIn()
    {
        _logInInputField.interactable = false;
        _logInToggle.interactable = false;
        _logInButton.interactable = false;
        bool remember = _logInToggle.isOn;
        string username = _logInInputField.text;

        bool exist = await SaveExist(username);
        if (!exist)
        {
            _logInInputField.interactable = true;
            _logInToggle.interactable = true;
            _logInButton.interactable = true;
            return;
        }

        await LogInUser(username, remember);
        _logInInputField.interactable = true;
        _logInToggle.interactable = true;
        _logInButton.interactable = true;
    }

    public void GoToUserPage()
    {
        _logInPage.SetActive(false);
        _registerPage.SetActive(false);
        _userPage.SetActive(true);

        _userNameText.text = _userBehaviour.UserName;
        _nbBananaText.text = _userBehaviour.CurrentUserData.Bananas.ToString() + " Banana(s)";

        _userBehaviour.OnUserUpdated += () => _userNameText.text = _userBehaviour.UserName;
        _userBehaviour.OnUserUpdated += () => _nbBananaText.text = _userBehaviour.CurrentUserData.Bananas.ToString() + " Banana(s)";
    }
}
