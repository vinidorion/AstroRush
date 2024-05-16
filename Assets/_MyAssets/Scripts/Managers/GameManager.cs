using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton

	private bool _isIntro = true;
	private float _startTime = 0f; // Time.time when the race starts

	private GameObject[] _arrPUs;
	private int _numSpaceship;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		// get les PUs sans utiliser SerializeField,
		// les PUs dans l'array seront dans le même ordre
		// qu'ils étaient dans le dossier Prefabs/Resources/PUs/
		//_arrPUs = Resources.LoadAll("PUs/", typeof(GameObject)).Cast<GameObject>().ToArray();

		_arrPUs = Resources.LoadAll("PUs/poly/", typeof(GameObject)).Cast<GameObject>().ToArray();

		// print la liste de PU pour debug
		/*foreach (GameObject pu in _arrPUs) {
			Debug.Log(pu.name.Substring(3));
		}*/
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks); // garder cette ligne ici
	}

	void Start()
	{
		foreach(testbot.Bot bot in FindObjectsOfType<testbot.Bot>()) {
			bot.InitializeDifficulty(5);
		}
		_numSpaceship = FindObjectsOfType<SpaceShip>().Length;
		if(PauseMenu.Instance) {
			PauseMenu.Instance.SetCanPause(false);
		}
		FreezeAll(true);
		if(InGameHud.Instance) {
			InGameHud.Instance.ToggleDrawHUD(false);
		}
	}

	void Update()
	{
		if(_isIntro) { // ne pas merge ces deux check de conditions
			if (Input.anyKeyDown) { // tout ce qui est dans ce block est called sur une seule frame (une seule fois)
				StartRace();
			}
		}
	}

	// méthode publique qui start la race
	// doit être publique pour être called dans la classe CameraController (quand l'anim d'intro est fini)
	public void StartRace() {
		CameraController.Instance.SetCameraMode(CameraMode.ThirdPerson);
		StartCoroutine(CountdownCoroutine());
		if(InGameHud.Instance) {
			InGameHud.Instance.ToggleDrawHUD(true);
			InGameHud.Instance.UpdateLap();
		}
		_isIntro = false;
	}

	// coroutine du countdown
	IEnumerator CountdownCoroutine()
	{
		yield return new WaitForSeconds(2f);
		NumberCountdown.Instance.Count();

		yield return new WaitForSeconds(1f);
		NumberCountdown.Instance.Count();

		yield return new WaitForSeconds(1f);
		NumberCountdown.Instance.Count();

		yield return new WaitForSeconds(1f);
		// RACE STARTS HERE
		NumberCountdown.Instance.Go();
		FreezeAll(false);
		if(PauseMenu.Instance) {
			PauseMenu.Instance.SetCanPause(false);
		}

		// comme le _startTime est le même pour tout le monde, 
		// je le store ici pour pas le store dans chaque classe spaceship
		// utilisez la méthode publique GetStartTime() pour obtenir _startTime
		_startTime = Time.time;
		InGameHud.Instance.StartTimer();
	}

	// méthode publique qui retourne le Time.time quand la course a commencé
	public float GetStartTime()
	{
		return _startTime;
	}

	// méthode publique qui permet de freeze/unfreeze tous les spaceship
	public void FreezeAll(bool freeze)
	{
		foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>()) {
			spaceship.Freeze(freeze);
		}
	}

	// méthode publique qui retourne le GameObject PU en fonction de l'index
	public GameObject GetGameObjectPU(int index)
	{
		bool isWithinRange = index >= 0 && index < _arrPUs.Length;
		return isWithinRange ? _arrPUs[index] : _arrPUs[0];
	}

	// méthode publique qui get le nombre de PUs,
	// utile dans la sélection aléatoire de PU
	public int GetNumPUs()
	{
		return _arrPUs.Length;
	}

	public int GetNumSpaceships()
	{
		return _numSpaceship;
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("Menu");
	}
}