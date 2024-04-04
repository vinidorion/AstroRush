using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Over")]
    public class Over : PU
    {

        protected Vector3 _offSet = default;
        protected SpaceShip _ship;

        protected override void Start()
        {
            transform.parent = _owner;
            transform.position = _owner.position + _offSet;
            _ship = _owner.GetComponent<SpaceShip>();
        }
    }
}
