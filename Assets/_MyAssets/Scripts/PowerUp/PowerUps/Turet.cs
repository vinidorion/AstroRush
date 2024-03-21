using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turet : MonoBehaviour
{
    [SerializeField] float _atkSpeed = default;
    private float timer = 0;
    private List<SpaceShip> _listSpaceship = new List<SpaceShip>();

    private void Start()
    {
        foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>())
        {
            _listSpaceship.Add(spaceship);
        }
    }

    private void FixedUpdate()
    {
        if (timer <= 0)
        {
            foreach (SpaceShip spaceship in _listSpaceship)
            {
                if ((spaceship.gameObject.transform.position - transform.position).magnitude < 2)
                {

                }
            }
        }
    }
}
