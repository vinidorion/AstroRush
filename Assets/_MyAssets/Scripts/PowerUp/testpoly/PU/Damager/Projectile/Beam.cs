using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: Beam")]
    public class Beam : Projectile
    {
        // rayon laser devant le spaceship qui fait des degat au autres spaceship qui le touche
        Ray ray; // ray pour la longueur du laser
        float shipSize; // distance a laquelle le laser commence pour ne pas etre dans le spaceship
        
        protected override void Awake()
        {
            _lifeTime = 6f;
        }

        protected override void Start()
        {
            base.Start();
            shipSize = _owner.GetComponent<SpaceShip>().GetSize(); // prend la distance a laquelle le laser doit commencer selon le SpaceShip
            transform.position = _owner.position + (_owner.forward * shipSize); // prend la position du spaceship pour mettre le laser au bon endroit
        }

        protected override void Update()
        {
            //empeche le beam d'avancer par override de la fonction Update de Projectile
            RaycastHit hit; 
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 3, ~(1 << 3))) // racourcit le laser si un objet le traverse comme si il le touchait
            {
                transform.localScale = new Vector3(1 , 1, hit.distance);
            }
            else transform.localScale = new Vector3(1, 1, 3); // sinon le met pleine grandeur
            transform.position = _owner.position + (_owner.forward * shipSize); // garde le laseer devant le spaceship
            transform.rotation = _owner.rotation; // garde le laser dans la meme orientation que le spaceshi^p
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