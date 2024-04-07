using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
	private bool _fadeIn = true;
	private bool _isFading = false;
	private AudioSource _audio;
	private float _speed = 1f;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();
	}

	void Update()
	{
		if(_isFading) {
			if(_fadeIn) {
				_audio.volume = Mathf.Clamp(_audio.volume + (Time.deltaTime * _speed), 0f, 1f);
				if(_audio.volume >= 1f) {
					_isFading = false;
				}
			} else {
				_audio.volume = Mathf.Clamp(_audio.volume - (Time.deltaTime * _speed), 0f, 1f);
				if(_audio.volume <= 0f) {
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
