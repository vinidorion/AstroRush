using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: PU")]
	public class PU : MonoBehaviour
	{
		// permet a tout les PU d'etre sous une meme classe

		protected Transform _owner; // proprietaire/utilisatuer du PU
		protected float _lifeTime = 1f; // temps de vie du PU avant detre destroy

		protected virtual void Start()
		{
			StartCoroutine(TimeLimitCoroutine()); // commence la coroutine pour le temps de vie du PU
		}

		// coroutine qui detruit le PU apres son temps de vie
		IEnumerator TimeLimitCoroutine()
		{
			yield return new WaitForSeconds(_lifeTime);
			Destroy(this.gameObject);
		}

		// defini le proprietaire/utilisateur du PU
		public void SetOwner(Transform owner)
		{
			_owner = owner;
		}

		// defini le temps de vie du PU
		public void SetLifeTime(float lifeTime)
		{
			_lifeTime = lifeTime;
		}
	}
}