using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: ShockWave")]
    public class ShockWave : Over
    {
        protected int _dmg = 0;
        protected int _slow = 0;
        protected int _slowTime = 0;
        [SerializeField] private GameObject _track = default;

        protected override void Start()
        {
            _lifeTime = .5f;
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true);
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true);
            base.Start();
        }

        protected void Update()
        {
            transform.localScale = transform.localScale + Vector3.one / 5;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            SpaceShip _ship = other.GetComponent<SpaceShip>();
            PU pU = other.GetComponent<PU>();
            if (_ship != null)
            {
                _ship.GiveHP(-_dmg);
                _ship.Slow(_slow, _slowTime);
            }
            else if (pU != null)
            {
                Destroy(other);
            }
        }
    }
}
