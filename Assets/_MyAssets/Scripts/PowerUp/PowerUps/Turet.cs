using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turet : MonoBehaviour
{
    //[SerializeField] float _atkSpeed = default;
    //[SerializeField] GameObject laser = default;
    //private float timer = 0;
    //private List<SpaceShip> _listSpaceship = new List<SpaceShip>();
    //private GameObject _ship = default;


    //private void Start()
    //{
    //    timer = _atkSpeed;
    //    foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>())
    //    {
    //        _listSpaceship.Add(spaceship);
    //    }
    //    _ship = transform.GetComponent<Projectile>().GetTarget();
    //}

    //private void FixedUpdate()
    //{
    //    timer = timer - 1 * Time.deltaTime;
    //    if (timer <= 0)
    //    {
    //        foreach (SpaceShip spaceship in _listSpaceship)
    //        {
    //            if (spaceship.gameObject != _ship)
    //            {
    //                Vector3 v = spaceship.gameObject.transform.position - transform.position;
    //                if ((v).magnitude < 2)
    //                {
    //                    transform.LookAt(spaceship.gameObject.transform.position);
    //                    GameObject x = Instantiate(laser, transform.position + transform.forward / 3, transform.rotation, transform);
    //                    Debug.Log(spaceship.gameObject);
    //                    x.GetComponent<Projectile>().SetTarget(spaceship.gameObject);
    //                    timer = _atkSpeed;
    //                }
    //            }
    //        }
    //    }
    //}

    //public SpaceShip GetShip() { return _ship.GetComponent<SpaceShip>(); }
}
