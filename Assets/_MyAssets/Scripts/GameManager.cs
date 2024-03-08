using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton

	private bool _isIntro = true;
	// 1s pour tester plus rapidement,
	// on mettra plus quand on aura fait
	// les animations de camera pour l'intro
	private const float INTRO_TIME = 1f;
	private float _startTime = 0f; // Time.time when the race starts

	private List<GameObject> _arrPUs = new List<GameObject>();

	private List<SpaceShip> _position = new List<SpaceShip>();

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		Camera.Instance.SetCameraMode(CameraMode.Intro);
		StartCoroutine(IntroCoroutine());

		// get les PUs sans utiliser SerializeField,
		// les PUs dans l'array seront dans le même ordre
		// qu'ils étaient dans le dossier Prefabs/Resources/PUs/
		Object[] loadedObjects = Resources.LoadAll("PUs");
        foreach (Object Object in loadedObjects)
        {
            _arrPUs.Add(Object as GameObject);
        }
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
		OrderPosition();
	}

	private void OrderPosition()
	{
		for (int i = 1; i < _position.Count; i++)
		{
			if (_position[i].GetPosValue() > _position[i - 1].GetPosValue())
			{
				SpaceShip tmp = _position[i];
				_position[i] = _position[i - 1];
				_position[i - 1] = tmp;
				_position[i - 1].SetPosition(i - 1);
				_position[i].SetPosition(i);
            }
		}
	}

	public SpaceShip GetShipFormPosition(int pos)
	{
		return _position[pos];
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
		return _arrPUs[PU];
	}

	// méthode publique qui retourne le GameObject PU en fonction de l'index
	public GameObject GetGameObjectPU(int index)
	{
		if (index >= 0 && index < _arrPUs.Count) {
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
		return _arrPUs.Count;
	}

	public void AddShipToList(SpaceShip spaceship)
	{
		_position.Add(spaceship);
	}
}
