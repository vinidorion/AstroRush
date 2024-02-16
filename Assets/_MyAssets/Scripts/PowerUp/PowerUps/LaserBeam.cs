using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 7;

    private float timer = 0;

    void Update()
    {
        Timer();
    }

    private void Timer()
    {
        timer = timer + 1 * Time.deltaTime;
        if (timer > _lifeTime) Destroy(gameObject);
    }
}
