using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Rocket")]
	public class Rocket : Projectile
	{
		[SerializeField] private GameObject _explosion = default;

		protected override void Awake()
		{
			//_lifeTime = ;
			_speed = 10f;
		}

		protected override void Update()
		{
			base.Update();
			_speed += Time.deltaTime * 100f;
		}

		protected override void OnTriggerEnter(Collider other)
		{
			if (_explosion != null) 
			{
                poly.PU l = Instantiate(_explosion, transform.position, transform.rotation).GetComponent<poly.PU>();
                l.SetLifeTime(.6f);
            }
            base.OnTriggerEnter(other);
		}
	}
}