using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
	public static GameData Instance; // Singleton

	private int _numBot = -1;
	private int _numLap = -1;
	private string _trackName;

	private string saveFilePath;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
		saveFilePath = Application.persistentDataPath + "/save.json";
	}

	void Start()
	{
		//StartCoroutine(Temp()); -- uncomment to test the save system
	}

	IEnumerator Temp()
	{
		yield return new WaitForSeconds(2f);
		List<float> floatList = new List<float>() { 5f };
		SaveData("Bob", floatList);
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

	// méthode privée pour save le score du joueur
	public void SaveData(string name, List<float> lapTimes)
	{
		string sceneName = SceneManager.GetActiveScene().name;
		SaveDataList saveList = new SaveDataList();

		if(File.Exists(saveFilePath)) {
			string strJsonFile = File.ReadAllText(saveFilePath);
			saveList = JsonUtility.FromJson<SaveDataList>(strJsonFile);

			float totalCurrentScore = 0f;
			foreach (float lapTime in lapTimes) {
				totalCurrentScore += lapTime;
			}

			bool addToSaves = CompareScore(saveList, sceneName, totalCurrentScore);
			// TODO: check if empty here?
			if(!addToSaves) {
				return;
			}
		}

		saveList.saves.Add(new SaveData(name, sceneName, lapTimes));

		string newJsonFile = JsonUtility.ToJson(saveList, true);
		File.WriteAllText(saveFilePath, newJsonFile);
	}

	// méthode privée qui vérifie si notre score est plus grand que le meilleur score
	// si oui, on enlève ce meilleur score
	private bool CompareScore(SaveDataList saveList, string sceneName, float totalCurrentScore)
	{
		List<SaveData> savesCopy = new List<SaveData>(saveList.saves);

		foreach(SaveData saveData in savesCopy) {

			// skip if not the same track name
			if(saveData.trackName != sceneName) {
				continue;
			}

			// skip if not the same amount of laps
			if(saveData.lapTimes.Count != _numLap) {
				continue;
			}

			// total lap time
			float totalBestScore = 0f;
			foreach (float lapTime in saveData.lapTimes) {
				totalBestScore += Mathf.Round(lapTime * 100f) / 100f; // round lap times (floating point precision errors)
			}
			
			// check if better than best score
			if(totalBestScore < totalCurrentScore) {
				// if current score isnt < than best score, then there's no point in finishing this loop
				return false;
			}

			// remove best score from save data
			saveList.saves.Remove(saveData);
		}

		return savesCopy.Count > 0;
	}
}