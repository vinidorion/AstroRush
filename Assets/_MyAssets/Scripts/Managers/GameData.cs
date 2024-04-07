using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	public static GameData Instance; // Singleton

	private int _numBot;
	private int _plyShip; // ou passer le nom du préfab (string)?? ou le préfab lui même (gameobject)???
	private string _trackName;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
	}

	public void SetNumBot(int numBot)
	{
		_numBot = numBot;
	}

	public int GetNumBot()
	{
		return _numBot;
	}

	public void SetPlyShip(int plyShip)
	{
		_plyShip = plyShip;
	}

	public int GetPlyShip()
	{
		return _plyShip;
	}

	public void SetTrackName(string trackName)
	{
		_trackName = trackName;
	}

	public string GetTrackName()
	{
		return _trackName;
	}
}
