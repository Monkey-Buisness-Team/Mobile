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
        public Transform StartingTransform { get; set; }
    }

    [SerializeField] private List<PageObject> _pages;
    [SerializeField] private Page _currentPage;

    [Header("Casino")]
    [SerializeField] private Transform _casinoTransform;
    [SerializeField] private List<Image> _selectedNavBar;
    [SerializeField] private List<Page> _casinoPages;
    [SerializeField] private List<Page> _fightPages;

    [SerializeField] private GameObject _homeButton;

    public void Start()
    {
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
        _currentPage = p.Type;

        Transform trans = _casinoPages.Contains(_currentPage) ? _casinoTransform : this.transform;
        trans.DOLocalMove(new Vector3(-p.Page.transform.localPosition.x, trans.localPosition.y), 0.5f);
    }

    public void GoToPage(string page)
    {
        var p = _pages.Find(x => x.Type.ToString().Equals(page));
        _currentPage = p.Type;

        Transform trans = _casinoPages.Contains(_currentPage) ? _casinoTransform : this.transform;
        trans.DOLocalMove(new Vector3(-p.Page.transform.localPosition.x, trans.localPosition.y), 0.5f);
    }

    public void DisableAllImage()
    {
        foreach (var image in _selectedNavBar)
        {
            image.enabled = false;
        }

        if(_currentPage != Page.Fruit)
            FruitGameManager.i.DeactiveGameOverText(false);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
