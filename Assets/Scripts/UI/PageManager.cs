using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public void Start()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            _pages[i].StartingTransform = _pages[i].Page.transform;
        }
    }

    public void GoToPage(Page page)
    {
        var p = _pages.Find(x => x.Type.Equals(page));
        _currentPage = p.Type;

        Transform trans = _currentPage == Page.Roulette || _currentPage == Page.Crash || _currentPage == Page.Mines || _currentPage == Page.Cases ? _casinoTransform : this.transform;
        trans.DOLocalMove(new Vector3(-p.Page.transform.localPosition.x, trans.position.y), 0.5f);
    }

    public void GoToPage(string page)
    {
        var p = _pages.Find(x => x.Type.ToString().Equals(page));
        _currentPage = p.Type;

        Transform trans = _currentPage == Page.Roulette || _currentPage == Page.Crash || _currentPage == Page.Mines || _currentPage == Page.Cases ? _casinoTransform : this.transform;
        trans.DOLocalMove(new Vector3(-p.Page.transform.localPosition.x, trans.position.y), 0.5f);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
