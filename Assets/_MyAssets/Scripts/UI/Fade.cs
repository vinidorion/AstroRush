using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
	private bool _fadeIn = true;
	private bool _isFading = false;
	private CanvasGroup _canvas;
	private float _speed = 1f;

	void Awake()
	{
		_canvas = GetComponent<CanvasGroup>();
	}

	void Update()
	{
		if(_isFading) {
			if(_fadeIn) {
				_canvas.alpha = Mathf.Clamp(_canvas.alpha - (Time.deltaTime * _speed), 0f, 1f);
				if(_canvas.alpha <= 0f) {
					_isFading = false;
				}
			} else {
				_canvas.alpha = Mathf.Clamp(_canvas.alpha + (Time.deltaTime * _speed), 0f, 1f);
				if(_canvas.alpha >= 1f) {
					_isFading = false;
				}
			}
		}
	}

	// true:	fade in
	// false:	fade out
	public void ToggleFade(bool fadeIn)
	{
		_isFading = true;
		_fadeIn = fadeIn;
	}

	public void SetSpeed(float speed)
	{
		_speed = speed;
	}
}
