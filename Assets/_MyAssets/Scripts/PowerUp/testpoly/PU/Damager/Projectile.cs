using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Projectile")]
	public class Projectile : Damager
	{
		protected Vector3 _direction = Vector3.zero;
		protected float _speed = 30f;

        protected virtual void Awake()
		{
			_lifeTime = 3f;
		}

		protected override void Start()
		{
			base.Start();
			_direction = transform.forward;
		}

		protected virtual void Update()
		{
			transform.position += _direction * _speed * Time.deltaTime;
		}

		protected override void OnTriggerEnter(Collider other)
		{
			base.OnTriggerEnter(other);
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