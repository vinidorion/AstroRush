using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Beam")]
    public class Beam : Projectile
    {
        Ray ray;
        protected override void Awake()
        {
            _lifeTime = 6f;
        }

        protected override void Start()
        {
            base.Start();
            transform.position = _owner.position + new Vector3(1, 0, 0);
        }

        protected override void Update()
        {
            //empeche le beam d'avancer par override de la fonction Update de Projectile
            RaycastHit hit;
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 3, ~(1 << 3)))
            {
                transform.localScale = new Vector3(1 , 1, hit.distance);
            }
            else transform.localScale = new Vector3(1, 1, 3);
            transform.position = _owner.position + _owner.forward;
            transform.rotation = _owner.rotation;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            //empeche le beam de destroy on hit par override de la fonction OnTriggerEnter de Projectile
            // faire les dégâts sur le other.GetComponent<Spaceship>()
            SpaceShip _ship = other.GetComponent<SpaceShip>();
            if (_ship != null)
            {
                _ship.GiveHP(-_dmg);
                _ship.Slow(_slow, _slowTime);
            }
        }
    }
}