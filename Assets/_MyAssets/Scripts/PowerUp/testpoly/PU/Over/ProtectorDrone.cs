using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: ProtectorDrone")]
    public class ProtectorDrone : Over
    {

        [SerializeField] private GameObject laser = default;
        private float timer = 0;
        [SerializeField] private float _atkSpeed = default;
        private List<SpaceShip> _listSpaceship = new List<SpaceShip>();
        private GameManager _gm;


        protected override void Start()
        {
            _offSet = new Vector3(0, .12f, 0);
            base.Start();
            timer = _atkSpeed;

            foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>())
            {
                if (spaceship != _ship) _listSpaceship.Add(spaceship);
            }
        }
        private void FixedUpdate()
        {
            timer = timer - 1 * Time.deltaTime;
            if (timer <= 0)
            {
                foreach (SpaceShip spaceship in _listSpaceship)
                {
                    Vector3 distance = spaceship.transform.position - transform.position;
                    if (distance.magnitude < 2)
                    {
                        transform.LookAt(spaceship.transform.position);
                        GameObject l = Instantiate(laser, transform.position + (transform.forward * 0.2f), Quaternion.LookRotation(transform.forward));
                        l.GetComponent<Missile>().SetTarget(spaceship.GetPosition());
                        l.GetComponent<PU>().SetOwner(_owner);
                        l.transform.localScale = l.transform.localScale / 3;
                        timer = _atkSpeed;
                        break;
                    }
                }
            }
        }
    }
}
