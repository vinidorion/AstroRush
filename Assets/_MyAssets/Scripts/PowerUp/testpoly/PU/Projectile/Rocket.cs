using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Rocket")]
	public class Rocket : Projectile
	{
		protected override void Awake()
		{
			base.Awake();

			_speed = 20f;
		}

		protected override void Update()
		{
			base.Update();

			_speed += Time.deltaTime * 100f;
		}

		protected override void OnTriggerEnter(Collider other)
		{
			// instantiate une explosion ici

			// l'explosion elle aussi fait des dégâts

			base.OnTriggerEnter(other);
		}
	}
}