using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenLogo : MonoBehaviour
{
	private List<Transform> _logoPiece = new List<Transform>();
	
	private bool _doAnim = false;
	private float _cooldown;
	private float _nextCooldown = 2f;
	private int _indexPiece = 0;

	void Awake()
	{
		foreach(Transform child in transform) {
			_logoPiece.Add(child);
			ChangeColor(child, Color.black);
		}
	}

	void Update()
	{
		if(_doAnim && Time.time > _cooldown) {
			ChangeColor(_logoPiece[_indexPiece], Color.red);
			_indexPiece++;
			if(_indexPiece >= _logoPiece.Count) {
				AsyncLoadingScreen.Instance.CanSkip();
				_doAnim = false;
			}
			_nextCooldown /= 2f;
			_cooldown = Time.time + _nextCooldown;
		}
	}

	// méthode privée qui change la couleur d'une pièce du logo
	private void ChangeColor(Transform logoPiece, Color newColor)
	{
		logoPiece.GetComponent<Renderer>().material.color = newColor;
	}

	// méthode publique qui start l'animation du logo
	public void StartLogoAnim()
	{
		_doAnim = true;
		_cooldown = Time.time + _nextCooldown;
	}

}
