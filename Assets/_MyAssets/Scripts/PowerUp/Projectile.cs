using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = default;
    [SerializeField] private float _acceleration = default;
    private SpaceShip _ship = default;
    private GameObject _target = null;
    private PU _pu;
    private int _wayPoint;
    private int debug = 0;
    [SerializeField] private int _dmg = default;
    [SerializeField] private float _slow = default;
    [SerializeField] private float _slowTime = default;
    [SerializeField] private int _aim = default; //0 =droit, 1 = nous meme, 2 = prochain ship

    private void Awake()
    {
        debug++;
        _pu = GetComponent<PU>();
        Debug.Log(transform.parent + " this is the parent");
        Debug.Log(debug + " this is the debug");
    }

    private void Start()
    {
        _pu = GetComponent<PU>();
        _ship = transform.parent.GetComponent<SpaceShip>();
        _wayPoint = _ship.GetWaypoint();
        transform.SetParent(null, true);
        Debug.Log(transform.parent + " this is the parent");
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
        Destroy(transform.parent.gameObject);
    }

    private void SetTarget()
    {
        if (_aim == 0)
        {
            _target = null;
        }
        if (_aim == 1)
        {
            _target = _ship.gameObject;
        }
        if (_aim == 2)
        {
            //_target = next spaceship
        }
    }
}
