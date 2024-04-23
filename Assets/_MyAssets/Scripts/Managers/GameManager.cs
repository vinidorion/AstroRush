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
	}

	void Start()
	{
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
		PauseMenu.Instance.SetCanPause(false);
		// freeze tout les spaceships ici avec FreezeAll(true)
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
	// doit être publique pour être called dans la classe Camera (quand l'anim d'intro est fini)
	public void StartRace() {
		Camera.Instance.SetCameraMode(CameraMode.ThirdPerson);
		StartCoroutine(CountdownCoroutine());
		_isIntro = false;
	}

	// coroutine du countdown
	IEnumerator CountdownCoroutine()
	{
		// draw le in game hud ici

		yield return new WaitForSeconds(2f);
		// 3

		yield return new WaitForSeconds(1f);
		// 2

		yield return new WaitForSeconds(1f);
		// 1

		yield return new WaitForSeconds(1f);
		// start race ici
		// unfreeze tout les spaceships ici avec FreezeAll(false)

		PauseMenu.Instance.SetCanPause(true);

		// comme le _startTime est le même pour tout le monde, 
		// je le store ici pour pas le store dans chaque classe spaceship
		// utilisez la méthode publique GetStartTime() pour obtenir _startTime
		_startTime = Time.time;
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

}
