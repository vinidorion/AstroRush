using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Shield")]
    public class Shield : Over
    {
        // pouclier qui protege le spaceship des attaques
        private int hpDepart; // vie du spaceship lors de l'activation du bouclier

        protected override void Start()
        {
            _lifeTime = 9999; // met le temps de vie "infini"
            base.Start();
            hpDepart = _ship.GetHP(); // prend la vie du spaceship
        }

        protected void Update()
        {
            if (hpDepart != _ship.GetHP()) // si la vie du spaceship descend
            {
                _ship.GiveHP(hpDepart - _ship.GetHP()); // remet la vie perdu
                Destroy(this.gameObject); // detruit le bouclier
            }
        }
    }
}
