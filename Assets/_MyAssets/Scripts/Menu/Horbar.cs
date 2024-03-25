using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horbar : MonoBehaviour
{
	private const float _target = 100f;
	private const float _speed = 4f;

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
				Debug.Log("stopped extending");			
			}
		}
	}

	public void Extend()
	{
		_isExtending = true;
	}
}
