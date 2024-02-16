using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _lifeTime = 7;

    private float timer = 0;

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);
        Timer();
    }

    private void Timer()
    {
        timer = timer + 1 * Time.deltaTime;
        if (timer > _lifeTime) Destroy(gameObject);
    }
}
