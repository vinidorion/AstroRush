using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeComp : MonoBehaviour
{
	public static TimeComp Instance;

	private TMP_Text _txt;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
		
		_txt = GetComponent<TMP_Text>();
		_txt.text = "";
	}

	// bool color
	//	true:	green
	//	false:	red
	public void DrawTimeComp(string timeComp, bool boolColor)
	{
		_txt.text = "<color=#" + ColorUtility.ToHtmlStringRGB(boolColor ? Color.green : Color.red) + ">" + timeComp;
		StartCoroutine(CoroutineDraw());
	}

	IEnumerator CoroutineDraw()
	{
		yield return new WaitForSeconds(2f);
		_txt.text = "";
	}
}
