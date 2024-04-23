using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShineText : MonoBehaviour
{
	private const float SPEED = 2f;
	private bool _isShining = false;
	private float _decalage;
	private float _startTime;

	private TextMeshProUGUI _text;

	void Awake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		_text.color = new Color(1f, 1f, 1f, 0f);
	}

	void Update()
	{
		if(_isShining) {
			Color color = _text.color;
			color.a = (Mathf.Cos((SPEED * (Time.time - _startTime)) - _decalage) / 2f) + 0.5f;
			_text.color = color;
		}
	}

	public void Shine()
	{
		_startTime = Time.time;
		_isShining = true;
		_decalage = Mathf.PI;
	}
}
