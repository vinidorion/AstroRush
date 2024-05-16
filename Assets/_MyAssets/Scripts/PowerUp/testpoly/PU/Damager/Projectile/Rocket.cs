using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Rocket")]
	public class Rocket : Projectile
	{
		// scripte utilise pour tout les PU de type rocket qui cree une explosion et qui accelere avec le temps
		[SerializeField] private GameObject _explosion = default; // gameobject de lexplosion a creer

		protected override void Awake()
		{
			//_lifeTime = ;
			_speed = 10f;
		}

		protected override void Update()
		{
			base.Update();
			_speed += Time.deltaTime * 100f; // vitesse dependente du temps de vol
		}

		protected override void OnTriggerEnter(Collider other)
		{
			if (_explosion != null) 
			{
                poly.PU l = Instantiate(_explosion, transform.position, transform.rotation).GetComponent<poly.PU>(); // instancie l'explosion
                l.SetLifeTime(.6f); // lui donne un temps de vie de .6 sec
            }
            base.OnTriggerEnter(other);
		}
	}
}