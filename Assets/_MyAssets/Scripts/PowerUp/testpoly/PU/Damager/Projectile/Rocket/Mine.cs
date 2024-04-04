using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Mine")]
    public class Mine : Rocket
    {
        protected override void Awake()
        {

            _lifeTime = 999999;
            _speed = 10f;
        }

        protected override void Start()
        {
            transform.position = transform.position - _owner.forward * .8f;
            base.Start();
            _direction = Vector3.zero;
        }
    }
}
