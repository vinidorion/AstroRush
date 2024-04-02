using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Damager")]
    public class Damager : PU
    {
        protected int _dmg = 0;
        protected int _slow = 0;
        protected int _slowTime = 0;

        protected override void Start()
        {
            base.Start();
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            SpaceShip _ship = other.GetComponent<SpaceShip>();
            if (_ship != null)
            {
                _ship.GiveHP(-_dmg);
                _ship.Slow(_slow, _slowTime);
            }
        }
    }
}
