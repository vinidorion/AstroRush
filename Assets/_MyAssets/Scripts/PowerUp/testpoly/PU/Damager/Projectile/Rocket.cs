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

			_speed += Time.deltaTime * 100f; // voir le commentaire dessous pour choisir quelle classe peut accelerer ou non
		}

/*	-- comment skip l'héritage d'une classe

		class A
		{
			public virtual void Method1()
			{
				AMethod1();
			}

			protected void AMethod1()
			{
				// faire qqch
			}
		}

		class B : A
		{
			public override void Method1()
			{
				base.Method1();
				// faire autre chose
			}
		}

		class C : B
		{   
			public override void Method1()
			{
				// ne pas call base.Method1() pour overrride class B
				// call Method1() de la classe A
				AMethod1();
			}
		}
*/

		protected override void OnTriggerEnter(Collider other)
		{
			poly.PU l = Instantiate(_explosion, transform.position, transform.rotation).GetComponent<poly.PU>();
			l.SetLifeTime(.6f);

            base.OnTriggerEnter(other);
		}
	}
}