using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
    [AddComponentMenu("POLYMORPHISM: AirStrike")]
    public class AirStrike : PU
    {
        [SerializeField] GameObject laser = default;
        private SpaceShip _ship = null;

        protected override void Start()
        {
            base.Start();

            for(int i = 0; i < _owner.GetComponent<SpaceShip>().GetPosition(); i++)
            {
                poly.PU l = Instantiate(laser, PosManager.Instance.GetShipFromPos(i).transform.position, Quaternion.LookRotation(transform.forward)).GetComponent<poly.PU>();
                l.SetOwner(_owner);
                l.SetLifeTime(.3f);
            }

            Destroy(this.gameObject);
        }
    }
} 
