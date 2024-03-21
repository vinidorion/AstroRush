using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = default;
    [SerializeField] private float _acceleration = default;

    private SpaceShip _ship = default;
    private GameObject _target = null;
    private PU _pu;
    private int _wayPoint;
    [SerializeField] private int _dmg = default;
    [SerializeField] private float _slow = default;
    [SerializeField] private float _slowTime = default;
    [SerializeField] private int _aim = default; //0 = droit, 1 = nous meme, 2 = prochain ship, 3 = premier joueur
    [SerializeField] private GameObject _explosion = default;
    private GameManager _gm;

    private void Awake()
    {
        _pu = GetComponent<PU>();
        _gm = GameManager.Instance;
    }

    private void Start()
    {
        _pu = GetComponent<PU>();
        _ship = transform.parent.GetComponent<SpaceShip>();
        _wayPoint = _ship.GetWaypoint();
        transform.SetParent(null, true);
        SetTarget();
    }

    void Update()
    {
        Move();
        Debug.Log(_target.name);
    }

    private void Move()
    {
        Vector3 direction = Vector3.zero;
        if (_target == null)
        {
            direction = transform.forward;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Translate(Vector3.forward * Time.deltaTime * _speed * (_pu.GetTimer() * _acceleration + 1));
        }
        else if (_target != _ship.gameObject)
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
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Translate(Vector3.forward * Time.deltaTime * _speed * (_pu.GetTimer() * _acceleration + 1));
        }
        else
        {
            Vector3 targetPosition = _target.transform.position + Vector3.up/4;
            Vector3 nextWaypointPosition = Vector3.left * 10000000;
            Vector3 position = transform.position;direction = targetPosition - position;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Translate(Vector3.forward * Time.deltaTime * _speed * (_pu.GetTimer() * _acceleration + 1) * (targetPosition - position).magnitude);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpaceShip _shipTouche = collision.gameObject.GetComponent<SpaceShip>();
        if (_shipTouche != null) 
        { 
            _shipTouche.SetCurrentLife(_shipTouche.GetHP() - _dmg);
            _shipTouche.Slow(_slow, _slowTime);
            Instantiate(_explosion, transform.position, transform.rotation);
        }
        if (_shipTouche == _target) Destroy(transform.parent.gameObject);
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
            if (_ship.GetPosition() != 0)
            {
                _target = PosManager.Instance.GetShipFromPos(_ship.GetPosition() - 1).gameObject;
            }
            else
            {
                _target = PosManager.Instance.GetShipFromPos(_ship.GetPosition() + 1).gameObject;
            }
        }
        if (_aim == 3)
        {
            if (_ship.GetPosition() != 0)
            {
                _target = PosManager.Instance.GetShipFromPos(0).gameObject;
                Debug.Log(_ship.GetPosition() - 1);
            }
            else
            {
                _target = PosManager.Instance.GetShipFromPos(_ship.GetPosition() + 1).gameObject;
            }
        }
    }

    public void SetWaypoint(int waypoint)
    {
        _wayPoint = waypoint;
    }
}
