using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireBaseInit : MonoBehaviour
{
    [SerializeField]
    GameObject go;

    private void Start()
    {
        FireBaseManager.i.OnFireBaseInit += () => go.SetActive(true);
    }

}
