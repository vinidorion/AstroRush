using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUBox : MonoBehaviour
{
	private const float COOLDOWN = 2f;
	private bool _isActive = true;
	private Material _mat;
	private const float COLOR_SPEED = 1f;
	private int _nbColors;
	private int _colorIndex;
	private float _interpolant;
	private Color[] _colors = {
		Color.red,
		Color.blue,
		Color.green,
		Color.red // même que le premier
	};

	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
		foreach(Transform child in transform) {
			if(child.name == "pubox") {
				_mat = child.GetComponent<MeshRenderer>().material;
			}
		}
		_nbColors = _colors.Length - 1;
	}

	void Update()
	{
		if(_isActive) {
			ColorEffect();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// éviter de GetComponent si le PUBox n'est même pas active
		if(!_isActive) {
			return;
		}

		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (!spaceship) {
			return;
		}

		// ne pas merge ce check de condition
		if(spaceship.GetPU() != -1) {  
			return;
		}

		spaceship.GivePU();
		_isActive = false;
		_mat.color = Color.black;
		StartCoroutine(CooldownCoroutine());
	}

	// coroutine pour reset l'état actif après un cooldown
	IEnumerator CooldownCoroutine()
	{
		yield return new WaitForSeconds(COOLDOWN);
		_isActive = true;
	}

	// apparence quand _isActive est true
	private void ColorEffect()
	{
		_mat.color = Color.Lerp(_colors[_colorIndex], _colors[_colorIndex + 1], _interpolant);

		if(_interpolant >= 1f) {
			_interpolant = 0f;
			_colorIndex++;
			if(_colorIndex >= _nbColors) {
				_colorIndex = 0;
			}
		}
		
		_interpolant += COLOR_SPEED * Time.deltaTime;
	}
}
