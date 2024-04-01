using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Projectile")]
	public class Projectile : PU
	{
		protected Vector3 _direction = Vector3.zero;
		protected float _speed = 30f;
		protected int _dmg = 0;
		protected int _slow = 0;
		protected int _slowTime = 0;

		void Awake()
		{
			_lifeTime = 3f;
		}

		protected override void Start()
		{
			base.Start();
			_direction = transform.forward;
			Debug.Log("Projectile: Start()");

			// HPBoost et SpeedBoost sont des objets vides
			Physics.IgnoreCollision(_owner.GetComponent<Collider>(), GetComponent<Collider>(), true);
		}

		protected virtual void Update()
		{
			transform.position += _direction * _speed * Time.deltaTime;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			SpaceShip _ship = other.GetComponent<SpaceShip>();
			if (_ship != null)
			{
				_ship.GiveHP(-_dmg);
				_ship.Slow(_slow, _slowTime);
			}
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