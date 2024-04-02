using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
	private const float LEN = 0.3f;

	private bool _isFading = false;
	private float _startTime = Mathf.Infinity;
	private Color _color = new Color(1f, 0f, 0f);

	void Update()
	{
		if(_isFading) {
			float ratio = Mathf.Clamp01((Time.time - _startTime) / LEN);
			_color.g = ratio;
			_color.b = ratio;
			GetComponent<Renderer>().material.color = _color;
			if(ratio == 1f) {
				_isFading = false;
			}
		}
	}

	public void FadeWhite()
	{
		_startTime = Time.time;
		_isFading = true;
	}
}
