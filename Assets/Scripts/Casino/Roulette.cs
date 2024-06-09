using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roulette : MonoBehaviour
{
    [SerializeField] private Transform _roll;
    private float _speed;
    private bool _isScrolling;

    private void Start() 
    {
        // Scroll();
    }
    public void Scroll()
    {
        if(_isScrolling)
            return;

        _speed = 50;
        // _speed = Random.Range(4,5);
        _isScrolling = true; 
        Debug.Log("Scroll");
    }


    void Update()
    {
        // _roll.position = Vector3.MoveTowards(_roll.position, _roll.position + Vector3.left * 100, _speed * Time.deltaTime) * 100;

        // if(_speed < 0){
        //     _speed -= Time.deltaTime;
        // }
        // else
        // {
        //     _speed = 0;
        //     _isScrolling = false;
        // }
    }
}
