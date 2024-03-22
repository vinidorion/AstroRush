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

    void Awake()
    {
        _ship = transform.parent.GetComponent<SpaceShip>();
    }

    void Start()
    {
        Physics.IgnoreCollision(_ship.GetComponent<Collider>(), GetComponent<Collider>(), true);
        if (PUtype == 0)
        {
            _ship.GiveHP(10);
        }
        else if (PUtype == 1)
        {
            _ship.SetBoost(_ship.GetBoost() + 10);
        }
    }

    void Update() 
    {
        Timer();
    }

    private void Timer()
    {
        _timer = _timer + 1 * Time.deltaTime;
        if (_timer > _lifeTime) Destroy(transform.gameObject);
    }
}
