using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton

	private bool _isIntro = true;
	// 1s pour tester plus rapidement,
	// on mettra plus quand on aura fait
	// les animations de camera pour l'intro
	private const float INTRO_TIME = 1f;
	private float _startTime = 0f; // Time.time when the race starts

	[SerializeField] private GameObject[] PUs = default;
	private Object[] _arrPUs;

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
		_arrPUs = Resources.LoadAll("PUs/", typeof(GameObject));

		foreach (Object pu in _arrPUs) {
			Debug.Log(pu);
		}
	}

	void Start()
	{
		Camera.Instance.SetCameraMode(CameraMode.Intro);
		StartCoroutine(IntroCoroutine());
	}

	void Update()
	{
		if(_isIntro) {
			if (Input.anyKeyDown) { // tout ce qui est dans ce block est called sur une seule frame (une seule fois)
				Camera.Instance.SetCameraMode(CameraMode.ThirdPerson);
				_isIntro = false;
				// call CountdownCoroutine() ici
			} else {
				// intro camera movement here
			}
		} else {
			// game
		}
	}

	IEnumerator IntroCoroutine()
	{
		// on a pas d'intro pour le moment, donc j'enlève temporairement le délai
		//yield return new WaitForSeconds(INTRO_TIME);
		
		// donc return null à la coroutine
		// (ne pas oublier de l'enlever)
		yield return null;

		Camera.Instance.SetCameraMode(CameraMode.ThirdPerson);
		_isIntro = false;
	}

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

	public GameObject PUManager(int PU)
	{
		return PUs[PU];
	}

	// méthode publique qui retourne le GameObject PU en fonction de l'index
	public GameObject GetGameObjectPU(int index)
	{
		if (index >= 0 && < _arrPUs.Length) {
			return _arrPUs[index];
		} else {
			Debug.Log("PU INDEX OUT OF RANGE OF THE PU ARRAY");
			return _arrPUs[0]; // il faut retourner qqch, donc retourner le premier par défaut
		}
	}

	// méthode publique qui get le nombre de PUs,
	// sera utile pour trouver un PU random
	public int GetNumPUs()
	{
		return _arrPUs.Length;
	}
}
