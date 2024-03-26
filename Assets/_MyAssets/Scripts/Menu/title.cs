using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour
{
	private const float DELAY_START = 4f;
	private const float DELAY_WHITE = 0.04f;
	private const float DELAY_RED = 0.03f;
	private const float DELAY_BACKTOWHITE = 0.02f;
	
	private List<Transform> _titlePiece = new List<Transform>();
	private Transform _blackBG;
	private Horbar _horbar;

	void Awake()
	{
		foreach(Transform child in transform) {
			if(child.name == "title") {
				foreach(Transform titlePiece in child) {
					ChangeColor(titlePiece, Color.black);
					_titlePiece.Add(titlePiece);
				}
			} else if(child.name == "black_bg") {
				_blackBG = child;
			} else if(child.name == "Opening") {
				child.GetComponent<AudioSource>().time = 25f;
			} else if(child.name == "horbar") {
				_horbar = child.GetComponent<Horbar>();
			}
		}
	}

	void Start()
	{
		StartCoroutine(temp());
	}

	IEnumerator temp()
	{
		yield return new WaitForSeconds(DELAY_START);
		for(int i = 0; i < _titlePiece.Count; i++) {
			StartCoroutine(AnimCoroutine(_titlePiece[i], i, Color.white));
		}
		yield return new WaitForSeconds(((_titlePiece.Count - 1) * DELAY_WHITE) + 0.25f);
		for(int i = 0; i < _titlePiece.Count; i++) {
			StartCoroutine(AnimCoroutine(_titlePiece[i], i, Color.red));
		}
		yield return new WaitForSeconds(0.5f);
		_horbar.Extend();
	}

	IEnumerator AnimCoroutine(Transform titlePiece, int index, Color color)
	{
		if(color == Color.red) {
			yield return new WaitForSeconds(index * DELAY_RED);
			ChangeColor(titlePiece, color);
			if(titlePiece.name[0] == 'L') {
				yield return new WaitForSeconds(index * DELAY_BACKTOWHITE);
				titlePiece.GetComponent<Letter>().FadeWhite();
			}
		} else {
			yield return new WaitForSeconds(index * DELAY_WHITE);
			ChangeColor(titlePiece, color);
		}
	}

	private void ChangeColor(Transform titlePiece, Color newColor)
	{
		titlePiece.GetComponent<Renderer>().material.color = newColor;
	}
}
