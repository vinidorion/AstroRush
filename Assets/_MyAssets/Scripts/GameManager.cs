using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton

	private bool _isIntro = true;
	// 1s pour tester plus rapidement
	// on mettra plus quand on aura fait
	// les animations de camera pour l'intro
	private const float INTRO_TIME = 1f;

	private float _startTime = 0f; // Time.time when the race starts

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
		yield return new WaitForSeconds(INTRO_TIME);
		Camera.Instance.SetCameraMode(CameraMode.ThirdPerson);
		_isIntro = false;
	}

	IEnumerator CountdownCoroutine()
	{
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

	// retourne le Time.time quand la course a commencé
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
}
