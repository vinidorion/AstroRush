using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataList
{
	public List<SaveData> saves = new List<SaveData>();
}

[System.Serializable]
public class SaveData
{
	public string plyName;
	public string trackName;
	public List<float> lapTimes;

	public SaveData(string plyName, string trackName, List<float> lapTimes)
	{
		this.plyName = plyName;
		this.trackName = trackName;
		this.lapTimes = lapTimes;
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public static SaveData FromJson(string json)
	{
		return JsonUtility.FromJson<SaveData>(json);
	}
}
