using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Damager")]
    public class Damager : PU
    {
        // script pour tout les PU qui peuvent enlever de la vie au contact

        protected int _dmg = 0; // degat inflige par le PU
        protected int _slow = 0; // ralentissement inflige par le PU
        protected int _slowTime = 0; // duree du ralentissement

        protected override void Start()
        {
            base.Start();
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true); // empeche le PU de faire des degats au proprietaire
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            SpaceShip _ship = other.GetComponent<SpaceShip>(); // va chercher le SpaceShip de lobjet si il existe
            if (_ship != null)
            {
                _ship.GiveHP(-_dmg); // inflige les degat
                _ship.Slow(_slow, _slowTime); // inflige le slow
            }
        }
    }
}
