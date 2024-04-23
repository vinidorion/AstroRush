using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	public static GameData Instance; // Singleton

	private int _numBot;
	private int _plyShip; // ou passer le nom du préfab (string)?? ou le préfab lui même (gameobject)???
	private int _numLap;
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

	// méthode publique qui assigne le nombre de bot
	// utilisé dans le menu
	public void SetNumBot(int numBot)
	{
		_numBot = numBot;
	}

	// méthode publique qui retourne le nombre de bot
	// utilisé dans la scene de jeu
	public int GetNumBot()
	{
		return _numBot;
	}

	// méthode publique qui assigne le ship du joueur
	public void SetPlyShip(int plyShip)
	{
		_plyShip = plyShip;
	}

	// méthode publique qui retourne le ship du joueur
	public int GetPlyShip()
	{
		return _plyShip;
	}

	// méthode publique qui assigne le nom de la scene à loader
	// utlisé dans le menu
	public void SetTrackName(string trackName)
	{
		_trackName = trackName;
	}

	// méthode publique qui retourne le nom de la scene à loader
	// utilisé dans la scene loading screen
	public string GetTrackName()
	{
		return _trackName;
	}

	// méthode publique qui assigne le nombre de tours
	// utilisé dans le menu
	public void SetNumLap(int numLap)
	{
		_numLap = numLap;
	}

	// méthode publique qui retourne le nombre de tours
	// utilisé dans la scene de jeu
	public int GetNumLap()
	{
		return _numLap;
	}
}
