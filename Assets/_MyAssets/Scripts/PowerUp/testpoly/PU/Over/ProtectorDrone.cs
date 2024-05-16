using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: ProtectorDrone")]
    public class ProtectorDrone : Over
    {
        // drone protecteur qui flote au dessus du spaceship et tire les enemies autour
        [SerializeField] private GameObject laser = default; // laser tire par le drone
        private float timer = 0; // temps depuis la derniere attaque
        [SerializeField] private float _atkSpeed = default; // vitesse a laquelle le drone attaque
        private List<SpaceShip> _listSpaceship = new List<SpaceShip>(); // liste de tout les spaceship
        private GameManager _gm; // GameManager


        protected override void Start()
        {
            _offSet = new Vector3(0, .12f, 0); // met le offset a .12 de haut
            base.Start();
            timer = _atkSpeed; // met le timer a atkspeed pour ne pas qu'il tire en apparaissant

            // va chercher tout les autres spaceship en jeu
            foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>())
            {
                if (spaceship != _ship) _listSpaceship.Add(spaceship);
            }
        }
        private void FixedUpdate()
        {
            // regarde si un vaisseau est dans la zone de tire et le tire si le cooldown est fini 
            timer = timer - 1 * Time.deltaTime; // reduit timer
            if (timer <= 0) // si le timer est au plus a 0
            {
                foreach (SpaceShip spaceship in _listSpaceship) // fait le tour des spaceship pour savoir si ils sont ''in range''
                {
                    Vector3 distance = spaceship.transform.position - transform.position; // regarde la distance entre le drone et le spaceship
                    if (distance.magnitude < 2) // si le spaceship est suffisament proche
                    {
                        transform.LookAt(spaceship.transform.position); // tourne le drone dans la bonne direction
                        GameObject l = Instantiate(laser, transform.position + (transform.forward * 0.2f), Quaternion.LookRotation(transform.forward)); // fait apparaitre un laser vers le spaceship
                        l.GetComponent<Missile>().SetTarget(spaceship.GetPosition()); // met la cible du laser sur le spaceship
                        l.GetComponent<PU>().SetOwner(_owner); // met le owner du laser
                        l.transform.localScale = l.transform.localScale / 3; // scale le laser avec le spaceship
                        timer = _atkSpeed; // reset le timer
                        break;
                    }
                }
            }
        }
    }
}
