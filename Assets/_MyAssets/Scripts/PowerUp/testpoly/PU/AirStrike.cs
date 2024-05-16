using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: AirStrike")]
    public class AirStrike : PU
    {
        // airstrike qui fait des laser sur tout les joueurs en avant de l'utilisateur

        [SerializeField] GameObject laser = default; // va chercher le gameobject des laser

        protected override void Start()
        {
            base.Start();

            // va chercher tout les spaceship en avant et instancie un laser dessus
            for(int i = 0; i < _owner.GetComponent<SpaceShip>().GetPosition(); i++)
            {
                poly.PU l = Instantiate(laser, PosManager.Instance.GetShipFromPos(i).transform.position, Quaternion.LookRotation(transform.forward)).GetComponent<poly.PU>(); // instancie le laser
                l.SetOwner(_owner); // defini l'utilisateur pour ne pas qu'il prenne de degat
                l.SetLifeTime(.3f); // lui donne un temps de vie de .3 secondes
            }

            Destroy(this.gameObject);
        }
    }
} 
