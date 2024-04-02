using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Over")]
    public class Over : PU
    {
        private int hpDepart;
        private SpaceShip _ship;

        protected override void Start()
        {
            transform.parent = _owner;
            transform.position = _owner.position;
            _ship = _owner.GetComponent<SpaceShip>();
            hpDepart = _ship.GetHP();
        }

        protected void Update()
        {
            if(hpDepart != _ship.GetHP())
            {
                _ship.GiveHP(hpDepart - _ship.GetHP());
                Destroy(this.gameObject);
            }
        }
    }
}
