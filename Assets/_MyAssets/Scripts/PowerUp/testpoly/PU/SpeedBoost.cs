using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: SpeedBoost")]
	public class SpeedBoost : PU
	{
		private float _thrust = 10f;

		void Update()
		{
			if(_owner) {
				Rigidbody rb = _owner.GetComponent<Rigidbody>();
				rb.AddForce(transform.forward * _thrust, ForceMode.Impulse);
				_owner = null;
			}
		}
	}
}