using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: HPBoost")]
	public class HPBoost : PU
	{
		// soint pour redonner de la vie au spaceship
		protected override void Start()
		{
			base.Start();

			_owner.GetComponent<SpaceShip>().GiveHP(10); // redonne de la vie au joueur
		}
	}
}