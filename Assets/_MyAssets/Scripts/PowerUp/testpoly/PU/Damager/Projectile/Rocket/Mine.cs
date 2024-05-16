using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Mine")]
    public class Mine : Rocket
    {
        // Mine qui reste sur la piste et qui explose au contacte
        protected override void Awake()
        {

            _lifeTime = 999999; // temps de vie "infini"
            _speed = 10f;
        }

        protected override void Start()
        {
            transform.position = transform.position - _owner.forward * .8f; // met la mine juste deriere le spaceship
            base.Start();
            _direction = Vector3.zero; // l'imobilise
        }
    }
}
