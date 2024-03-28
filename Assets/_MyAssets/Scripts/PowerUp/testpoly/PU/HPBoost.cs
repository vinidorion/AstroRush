using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: HPBoost")]
	public class HPBoost : PU
	{

		void Update()
		{
			if(_owner) {
				_owner.GetComponent<SpaceShip>().GiveHP(10);
				_owner = null;
			}
		}
	}
}