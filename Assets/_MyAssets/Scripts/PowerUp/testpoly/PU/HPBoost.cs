using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: HPBoost")]
	public class HPBoost : PU
	{
		protected override void Start()
		{
			base.Start();

			//Debug.Log("HPBoost: Start()");
			_owner.GetComponent<SpaceShip>().GiveHP(10);
		}
	}
}