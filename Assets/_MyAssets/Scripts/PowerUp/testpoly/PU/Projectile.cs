using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Projectile")]
	public class Projectile : PU
	{
		protected Vector3 _direction = Vector3.zero;
		protected float _speed = 30f;

		protected override void Awake()
		{
			_direction = transform.forward;
			_lifeTime = 3f;

			base.Awake();
		}

		protected virtual void Update()
		{
			transform.position += _direction * _speed * Time.deltaTime;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			// faire les dégâts sur le other.GetComponent<Spaceship>()

			Destroy(this.gameObject);
		}

		public void SetDirection(Vector3 direction)
		{
			_direction = direction;
			//transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
			transform.LookAt(transform.position + direction);
		}
	}
}