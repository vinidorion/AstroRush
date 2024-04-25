using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

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
		_numSpaceship = FindObjectsOfType<SpaceShip>().Length;
		PauseMenu.Instance.SetCanPause(false);
		FreezeAll(true);
		if(InGameHud.Instance) {
			InGameHud.Instance.ToggleDrawHUD(false);
		}
		if(GameData.Instance) {
			// TODO: NE PAS OUBLIER D'ENLEVER
			int nbdelap = 3;
			GameData.Instance.SetNumLap(nbdelap);
			Debug.Log($"nombre de lap: {nbdelap}");
		} else {
			Debug.Log("NO GAMEDATA OBJECT");
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
		// start race ici
		NumberCountdown.Instance.Go();
		FreezeAll(false);
		PauseMenu.Instance.SetCanPause(true);

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
		if (index >= 0 && index < _arrPUs.Length) {
			return _arrPUs[index];
		} else {
			Debug.Log("PU INDEX OUT OF RANGE OF THE PU ARRAY");
			return _arrPUs[0]; // il faut retourner qqch, donc retourner le premier par défaut
		}
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

}
