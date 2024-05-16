using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fin : MonoBehaviour
{
	public static Fin Instance;	// Singleton
	
	private CanvasGroup _canvas;

	private TMP_InputField _inputField;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
		_canvas = GetComponent<CanvasGroup>();
		_canvas.alpha = 0f;

		foreach(Transform child in transform) {
			if(child.name == "InputField") {
				_inputField = child.GetComponent<TMP_InputField>();
			}
		}
	}

	public void Draw()
	{
		_inputField.text = "";
		_canvas.alpha = 1f;
	}
}
