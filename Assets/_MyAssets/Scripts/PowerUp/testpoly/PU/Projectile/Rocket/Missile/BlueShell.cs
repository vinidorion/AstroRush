using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace poly
{
    [AddComponentMenu("POLYMORPHISME: Missile")]
    public class BlueShell : Missile
    {
		void Awake()
		{
			//_lifeTime = ;
			_speed = 30f;
		}

<<<<<<< Updated upstream
		protected override void Start()
		{
			base.Start();
=======
            _speed = 20f;
        }

        protected override void Start()
        {
            base.Start();
            if (_owner)
            {
                SetTarget();
            }
        }
>>>>>>> Stashed changes

			SetTarget();
		}

        private void SetTarget()
        {
            if (_owner.GetComponent<SpaceShip>().GetPosition() != 0)
            {
                _target = PosManager.Instance.GetShipFromPos(0).transform;
            }
            else
            {
                _target = PosManager.Instance.GetLastShip().transform;
            }
        }
    }
}
