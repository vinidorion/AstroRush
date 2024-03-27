using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horbar : MonoBehaviour
{
	private float _target;
	private float _speed;

	private bool _isExtending = false;
	private Vector3 _scale = new Vector3(0f, 100f, 100f);

	void Awake()
	{
		transform.localScale = _scale;
	}

	void Update()
	{
		if(_isExtending) {
			float diff = _target - transform.localScale.x;
			_scale.x += Time.deltaTime * diff * _speed;
			transform.localScale = _scale;
			if(_scale.x >= _target) {
				_isExtending = false;
			}
		}
	}

	public void Extend(float speed, float target)
	{
		_isExtending = true;
		_speed = speed;
		_target = target;
	}
}
