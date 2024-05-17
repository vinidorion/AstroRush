using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; -- ne pas importer de package inutile
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: ShockWave")]
    public class ShockWave : Over
    {
        // onde de choc qui fait du degat a tout les spaceship autour
        protected int _dmg = 0; // degat inflige
        protected int _slow = 0; // ralentissement inflige
        protected int _slowTime = 0; // duree du ralentissement

        protected override void Start()
        {
            _lifeTime = .5f; // lui donne un temps de vie
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true); // ignore les colision avec le proprietaire
            base.Start();
        }

        protected void Update()
        {
            transform.localScale = transform.localScale + Vector3.one / 5; // scale l'onde avec le spaceship
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            SpaceShip _ship = other.GetComponent<SpaceShip>(); // va chercher le spaceship de l'objet s'il existe
            PU pU = other.GetComponent<PU>(); // va chercher le PU de l'objet s'il existe
            if (_ship != null) // si c'est un spaceship
            {
                _ship.GiveHP(-_dmg); // inflige les degat
                _ship.Slow(_slow, _slowTime); // inflige le ralentissement
            }
            else if (pU != null) // si c'est un PU 
            {
                Destroy(other); // le detruit
            }
        }
    }
}
