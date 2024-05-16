using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Over")]
    public class Over : PU
    {
        // script pour tout les PU qui reste au dessus du joueur

        protected Vector3 _offSet = default; // offset par rapport a la position du joueur
        protected SpaceShip _ship; // spaceship sur lequel il reste

        protected override void Start()
        {

            transform.parent = _owner;
            transform.position = _owner.position + _offSet; // se met a la bonne position par rapport au spaceship
            _ship = _owner.GetComponent<SpaceShip>();
            base.Start();
        }
    }
}
