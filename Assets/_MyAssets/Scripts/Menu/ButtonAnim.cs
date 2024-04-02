using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim : MonoBehaviour, IPointerEnterHandler
{
	private float _currTargetLen = 0f;
	private float _targetLen = 0f;

	private RectTransform _blackBG;
	private Vector2 _blackBGlen = new Vector2(0f, 0f);
	private float _speed = 20f;

	void Awake()
	{
		foreach(Transform child in transform) {
			if(child.name == "Image") {
				_blackBG = child.GetComponent<RectTransform>();
				_blackBGlen = _blackBG.sizeDelta;
				_blackBGlen.x = 0f;
			}
		}

		_targetLen = GetComponent<RectTransform>().sizeDelta.x;
	}

	void Update()
	{
		_blackBGlen.x += (_currTargetLen - _blackBGlen.x) * Time.deltaTime * _speed;
		_blackBG.sizeDelta = _blackBGlen;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Menu.Instance.ResetAllButtonAnimLen();
		_currTargetLen = _targetLen;
	}

	public void Reset()
	{
		_currTargetLen = 0f;
	}
}
