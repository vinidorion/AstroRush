using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU : MonoBehaviour
{
    private float _lifeTime = default;
    private float _timer = 0;

    public float GetTimer() { return _timer; }

    private void FixedUpdate()
    {
        Timer();
    }

    private void Timer()
    {
        _timer = _timer + 1 * Time.deltaTime;
        if (_timer > _lifeTime) Destroy(gameObject);
    }
}
