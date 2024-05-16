using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Projectile")]
	public class Projectile : Damager
	{
		// script pour tout les projectiles des PU
		protected Vector3 _direction = Vector3.zero; // direction du projectile
		protected float _speed = 30f; // vitesse du projectile

        protected virtual void Awake()
		{
			_lifeTime = 3f;
		}

		protected override void Start()
		{
			base.Start();
			_direction = transform.forward; // direction par defaut est devant
		}

		protected virtual void Update()
		{
			transform.position += _direction * _speed * Time.deltaTime; // fait avancer le projectile dans la direction
		}

		protected override void OnTriggerEnter(Collider other)
		{
			base.OnTriggerEnter(other);
			Destroy(this.gameObject); // detruit le projectile avec la collision
		}

		// defini la direction dans laquelle le PU se deplace et le tourne dans cette direction
		public void SetDirection(Vector3 direction)
		{
			_direction = direction;
			transform.LookAt(transform.position + direction);
		}
	}
}