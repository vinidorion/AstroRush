using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed = default;
    private float _acceleration = default;
    private SpaceShip _ship = default;
    private GameObject _target = null;
    private PU _pu;
    private int _wayPoint = default;
    private int _dmg = default;
    private float _slow = default;
    private float _slowTime = default;

    public void SetSpeed(float speed) {_speed = speed;}
    public void Acceleration(float acceleration) {_acceleration = acceleration;}

    private void Awake()
    {
        _pu = GetComponent<PU>();
        _ship = GetComponent<SpaceShip>(); //besoin de la liste de spaceship dans l'ordre. Ceci est temporaire
        _wayPoint = _ship.GetWaypoint();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = Vector3.zero;
        if (_target == null)
        {
            direction = Vector3.forward;
        }
        else
        {
            Vector3 targetPosition = _target.transform.position;
            Vector3 nextWaypointPosition = WaypointManager.Instance.GetWaypointPos(_wayPoint + 1);
            Vector3 position = transform.position;
            if ((targetPosition - position).magnitude <= (nextWaypointPosition - position).magnitude)
            {
                direction = targetPosition - position;
            }
            else
            {
                direction = nextWaypointPosition - position;
            }
        }
        transform.rotation = Quaternion.LookRotation(direction);
        transform.Translate(Vector3.forward * Time.deltaTime * _speed * (_pu.GetTimer() * _acceleration + 1));
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpaceShip _shipTouche = collision.gameObject.GetComponent<SpaceShip>();
        if (_shipTouche != null) 
        { 
            _shipTouche.SetCurrentLife(_shipTouche.GetCurrentLife() - _dmg);
            _shipTouche.Slow(_slow, _slowTime);
        }
        Destroy(this);
    }
}
