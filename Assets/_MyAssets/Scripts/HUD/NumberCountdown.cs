using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberCountdown : MonoBehaviour
{
	public static NumberCountdown Instance; // Singleton

	private const float SPEED = 0.5f;
	private int _number = 4;
	private TMP_Text _txt;
	private CanvasGroup _canvasGroup;
	private float _scale = 0f;

	void Awake() {
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_txt = GetComponent<TMP_Text>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_txt.text = "";
		_txt.transform.localScale = Vector3.zero;
	}

	void Update()
	{
		if(_scale > 0f) {
			_scale -= Time.deltaTime;
			_canvasGroup.alpha = _scale;
			if(_txt.text == "GO") {
				return;
			}
			_txt.transform.localScale = Vector3.one * _scale * SPEED;
		}
	}

	// méthode publique qui affiche le chiffre (3, 2, 1)
	public void Count()
	{
		_number--;
		_txt.text = _number.ToString();
		_scale = 1f;
	}

	// méthode publique qui affiche le "GO"
	public void Go()
	{
		_txt.text = "GO";
		_txt.transform.localScale = Vector3.one;
		_scale = 1f;
	}

	// méthode publique qui affiche la position à la fin du jeu (1st, 2nd, etc.)
	public void Position()
	{
		_txt.text = InGameHud.Instance.GetPosOrdinal();
		_txt.transform.localScale = Vector3.one;
		_scale = 1f;
	}
}
