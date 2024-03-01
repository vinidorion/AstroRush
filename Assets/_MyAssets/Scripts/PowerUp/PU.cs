using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU : MonoBehaviour
{
    [SerializeField] private float _lifeTime = default;
    private SpaceShip _ship;
    private float _timer = 0;
    [SerializeField] private int PUtype = default; // 0 = heal, 1 = boost, other = projectile

    public float GetTimer() { return _timer; }

    private void Awake()
    {
        _ship = transform.parent.GetComponent<SpaceShip>();
    }

    void Start()
    {
        if (PUtype == 0)
        {
            _ship.SetCurrentLife(_ship.GetCurrentLife() + 10);
        }
        else if (PUtype == 1)
        {
            _ship.SetBoost(_ship.GetBoost() + 10);
        }
    }

    private void FixedUpdate()
    {
        Timer();
    }

    private void Timer()
    {
        _timer = _timer + 1 * Time.deltaTime;
        if (_timer > _lifeTime) Destroy(transform.parent.gameObject);
    }

    
}
