using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: PU")]
	public class PU : MonoBehaviour
	{
		protected Transform _owner;
		protected float _lifeTime = 1f;

        private void Start()
        {
            StartCoroutine(TimeLimitCoroutine());
        }

        IEnumerator TimeLimitCoroutine()
		{
			yield return new WaitForSeconds(_lifeTime);
			Destroy(this.gameObject);
		}

		public void SetOwner(Transform owner)
		{
			_owner = owner;
            Physics.IgnoreCollision(owner.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
	}
}