using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
	private const float LEN = 0.3f;

	private bool _isFading = false;
	private float _colorValue = 0f;
	private float _startTime = Mathf.Infinity;

	void Update()
	{
		if(_isFading) {
			float ratio = Mathf.Clamp01((Time.time - _startTime) / LEN);
			GetComponent<Renderer>().material.color = new Color(1f, ratio, ratio);
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
