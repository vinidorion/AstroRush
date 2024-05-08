using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UIElement
{
	public string obName;
	public Transform tf;
	public Vector3 obPos;
	public Vector3 obTargetPos;

	public UIElement(string obName, Transform tf, Vector3 obPos, Vector3 obTargetPos)
	{
		this.obName = obName;
		this.tf = tf;
		this.obPos = obPos;
		this.obTargetPos = obTargetPos;
	}
}
