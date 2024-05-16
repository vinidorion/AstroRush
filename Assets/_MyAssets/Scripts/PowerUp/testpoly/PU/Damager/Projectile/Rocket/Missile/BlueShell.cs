using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace poly
{
    [AddComponentMenu("POLYMORPHISME: BlueShell")]
    public class BlueShell : Missile
    {
        // drone qui va chercher le premier joueur pour lui exploser dessus
		protected override void Awake()
		{
			//_lifeTime = ;
			_speed = 30f;
		}

		protected override void Start()
		{
			base.Start();

			SetTarget();
		}

        // met le premier joueur comme cible a viser
        private void SetTarget()
        {
            if (_owner.GetComponent<SpaceShip>().GetPosition() != 0)
            {
                _target = PosManager.Instance.GetShipFromPos(0).transform;
            }
            else
            {
                _target = PosManager.Instance.GetLastShip().transform; // si le SpaceShip est dea premier vise le dernier
            }

        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.transform == _target) base.OnTriggerEnter(other); // explose que si le SpaceShip touchee est la cible 
            else
            {
                SpaceShip _ship = other.GetComponent<SpaceShip>(); // fait des degat quand meme si cest la mauvaise cible
                if (_ship != null)
                {
                    _ship.GiveHP(-_dmg);
                    _ship.Slow(_slow, _slowTime);
                }
            }
        }
    }
}
