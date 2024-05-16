 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: SpeedBoost")]
	public class SpeedBoost : PU
	{
		// boost de vitesse qui fait accelerer le joueur 
		private const float THRUST = 10f; // puissance du boost

		protected override void Start()
		{
			base.Start();

			//Debug.Log("SpeedBoost: Start()");
			Rigidbody rb = _owner.GetComponent<Rigidbody>();
			rb.AddForce(transform.forward * THRUST, ForceMode.Impulse); // applique la force au Rigidbody du spaceship
		}
	}
}