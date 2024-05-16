using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: SpeedBoost")]
	public class SpeedBoost : PU
	{
		private const float THRUST = 10f;

		protected override void Start()
		{
			base.Start();

			//Debug.Log("SpeedBoost: Start()");
			Rigidbody rb = _owner.GetComponent<Rigidbody>();
			rb.AddForce(transform.forward * THRUST, ForceMode.Impulse);
		}
	}
}