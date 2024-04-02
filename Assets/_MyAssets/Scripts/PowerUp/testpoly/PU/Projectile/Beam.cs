using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Beam")]
    public class Beam : Projectile
    {
        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            transform.position = _owner.position + new Vector3(1, 0, 0);
            transform.parent = _owner;
        }

        protected override void Update()
        {
            //empeche le beam d'avancer en override la fonction Update de Projectile
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            // faire les dégâts sur le other.GetComponent<Spaceship>()
            SpaceShip _ship = other.GetComponent<SpaceShip>();
            if (_ship != null)
            {
                _ship.GiveHP(-_dmg);
                _ship.Slow(_slow, _slowTime);
            }
        }
    }
}