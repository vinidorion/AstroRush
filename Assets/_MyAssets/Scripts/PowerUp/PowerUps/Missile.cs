using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _lifeTime = 3;

    private float timer = 0;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _speed * ((timer * timer) + 1));
        Timer();
    }

    private void Timer()
    {
        timer = timer + 1 * Time.deltaTime;
        if (timer > _lifeTime) Destroy(gameObject);
    }
}
