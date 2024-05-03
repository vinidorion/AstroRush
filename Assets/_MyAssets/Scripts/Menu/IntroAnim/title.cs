using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour
{
	private const float DELAY_START = 4f;
	private const float DELAY_WHITE = 0.04f;
	private const float DELAY_RED = 0.03f;
	private const float DELAY_BACKTOWHITE = 0.02f;
	private const float BLACK_BARS_TARGET = 200f;

	private Transform _titleParent;
	private List<Transform> _titlePiece = new List<Transform>();
	private Transform _blackBG;
	private Horbar _blackFG;
	private Horbar _horbar;

	private RectTransform _topBar;
	private RectTransform _botBar;
	private Vector2 _blackBarsSize = new Vector2(1920f, 0f);
	private bool _blackBarsAreMoving = false;
	private float _speed = 2f;

	void Awake()
	{
		foreach(Transform child in transform) {
			switch (child.name)
			{
				case "title":
					_titleParent = child;
					foreach (Transform titlePiece in child) {
						ChangeColor(titlePiece, Color.black);
						_titlePiece.Add(titlePiece);
					}
					break;
				case "black_bg":
					_blackBG = child;
					break;
				case "black_fg":
					_blackFG = child.GetComponent<Horbar>();
					break;
				case "Opening":
					child.GetComponent<AudioSource>().time = 25f;
					break;
				case "horbar":
					_horbar = child.GetComponent<Horbar>();
					break;
			}
		}
		_topBar = GameObject.Find("blackbar_top").GetComponent<RectTransform>();
		_botBar = GameObject.Find("blackbar_bottom").GetComponent<RectTransform>();
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
		_horbar.Extend(4f, 100f);
		yield return new WaitForSeconds(2f);
		_blackFG.Extend(0.1f, 50f);
		yield return new WaitForSeconds(1f);
		_blackBarsAreMoving = true;
		_blackBarsSize.y = 1080f;
		yield return new WaitForSeconds(0.1f);
		Destroy(_titleParent.gameObject);
		Destroy(_horbar.gameObject);
		Destroy(_blackFG.gameObject);
		Destroy(_blackBG.gameObject);
		yield return new WaitForSeconds(0.5f);
		Menu.Instance.ToggleCameraMovement(true);
		yield return new WaitForSeconds(1f);
		Menu.Instance.ShowMainMenu(true);
		Menu.Instance.FadeBackground();
	}

	void Update()
	{
		if(_blackBarsAreMoving) {
			SetBlackBarHeight();
		}
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

	private void SetBlackBarHeight()
	{
		if(_blackBarsSize.y <= 200.1f) {
			_blackBarsAreMoving = false;
		}
		_blackBarsSize.y += (BLACK_BARS_TARGET - _blackBarsSize.y) * Time.deltaTime * _speed;
		_topBar.sizeDelta = _blackBarsSize;
		_botBar.sizeDelta = _blackBarsSize;
	}
}
