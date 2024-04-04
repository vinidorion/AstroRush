using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Shield")]
    public class Shield : Over
    {
        private int hpDepart;

        protected override void Start()
        {
            base.Start();
            hpDepart = _ship.GetHP();
        }

        protected void Update()
        {
            if (hpDepart != _ship.GetHP())
            {
                _ship.GiveHP(hpDepart - _ship.GetHP());
                Destroy(this.gameObject);
            }
        }
    }
}
