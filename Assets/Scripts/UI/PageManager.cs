using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    [System.Serializable]
    public class PageObject
    {
        public Page Type;
        public GameObject Page;
        public Sprite HelpImage;
        public Transform StartingTransform { get; set; }
    }

    [SerializeField] private List<PageObject> _pages;
    [SerializeField] private Page _currentPage;

    [SerializeField] private GameObject _homeButton;
    [SerializeField] private GameObject _helpButton;
    [SerializeField] private Image _helpImageHolder;

    [Header("Casino")]
    [SerializeField] private Transform _casinoTransform;
    [SerializeField] private List<Image> _selectedNavBar;
    [SerializeField] private List<Page> _casinoPages;
    [SerializeField] private Page _lastCasinoPage;

    [Header("Pari")]
    [SerializeField] private List<Page> _fightPages;

    [Header("LeaderBoard")]
    [SerializeField] private List<Page> _leaderBoardPages;
    [SerializeField] private Transform _leaderBoardTransform;
    [SerializeField] private List<Image> _selectedNavBarL;
    [SerializeField] private List<Image> _colorNavBar;

    private Color[] leaderBoardColor = new Color[]
    {
        new Color(0.992f, 0.859f, 0.29f),
        new Color(0.224f, 0.878f, 0.376f),
        new Color(1f, 0.302f, 0.302f)
    };


    public void Start()
    {
        _lastCasinoPage = Page.Crash;
        SwitchColorLeaderBoard(0);

        for (int i = 0; i < _pages.Count; i++)
        {
            _pages[i].StartingTransform = _pages[i].Page.transform;
        }

        FirebaseAutorisationManager.i.RoomIsOpen.AddListener(async (value) =>
        {
            if(!value && _fightPages.Contains(_currentPage))
            {
                await Task.Delay(500);
                GoToPage(Page.Home);
                _homeButton.SetActive(false);
            }
        });
    }

    public void GoToPage(Page page)
    {
        var p = _pages.Find(x => x.Type.Equals(page));

        if (_currentPage == p.Type) return;

        _currentPage = p.Type;

        Transform trans = this.transform;
        
        if (_casinoPages.Contains(_currentPage))
            trans = _casinoTransform;

        if (_leaderBoardPages.Contains(_currentPage))
            trans = _leaderBoardTransform;

        trans.DOLocalMove(new Vector3(-p.Page.transform.localPosition.x, trans.localPosition.y), 0.5f);

        if (_currentPage == Page.Casino)
            _currentPage = _lastCasinoPage;
        else if (_casinoPages.Contains(_currentPage))
            _lastCasinoPage = _currentPage;

        _helpButton.SetActive(_pages.Find(x => x.Type.ToString().Equals(_currentPage.ToString())).HelpImage != null);
        _helpImageHolder.gameObject.SetActive(false);
    }

    public void GoToPage(string page)
    {
        var p = _pages.Find(x => x.Type.ToString().Equals(page));
        GoToPage(p.Type);
    }

    public void DisableAllImage(bool isCasino)
    {
        if (isCasino)
        {
            foreach (var image in _selectedNavBar)
            {
                image.enabled = false;
            }
        }
        else
        {
            foreach (var image in _selectedNavBarL)
            {
                image.enabled = false;
            }
        }

        if(_currentPage != Page.Fruit)
            FruitGameManager.i.DeactiveGameOverText(false);
    }

    public void TryDisplayHelp()
    {
        if (_helpImageHolder.isActiveAndEnabled)
        {
            _helpImageHolder.gameObject.SetActive(false);
            return;
        }

        var page = _pages.Find(x => x.Type.Equals(_currentPage));
        if (page.HelpImage != null)
        {
            _helpImageHolder.sprite = page.HelpImage;
            _helpImageHolder.gameObject.SetActive(true);
        }
    }

    public void SwitchColorLeaderBoard(int id)
    {
        if (id >= 3) return;

        foreach (var c in _colorNavBar)
        {
            c.DOColor(leaderBoardColor[id], 0.5f);
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
