using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	public static Camera Instance; // Singleton

	private CameraMode _currentMode;
	private Player[] _plyArray;
	private Transform _plyPos;

	private float _camRotSpeed = 10f;

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
		FindPly();

		//_currentMode = CameraMode.Intro;
		//_currentMode = CameraMode.Spectate; // pour tester
	}

	void Update()
	{
		switch (_currentMode)
		{
			case CameraMode.Intro:
				Intro();
				break;
			case CameraMode.FirstPerson:
				FirstPerson();
				break;
			case CameraMode.ThirdPerson:
				ThirdPerson();
				break;
			case CameraMode.Spectate:
				Spectate();
				break;
		}
	}

	// à mettre dans un singleton pour trouver le joueur une fois et ensuite tous les scripts accèdent à ce singleton pour avoir le player
	private void FindPly() {
		_plyArray = FindObjectsOfType<Player>();

		int _plyArrLen = _plyArray.Length;

		if(_plyArrLen == 1) {
			// player found
			_plyPos = _plyArray[0].transform;
		} else if(_plyArrLen == 0) {
			// no player found
			Debug.Log("PLAYER NOT FOUND");
		} else {
			// there's more than one player
			Debug.Log("THERE'S MORE THAN ONE PLAYER");
		}
	}

	// enum CameraMode:
		// Intro
		// FirstPerson
		// ThirdPerson
		// Spectate
	public void RotateCameraMode()
	{
        if ((int)_currentMode == 1)
		{
			_currentMode = CameraMode.ThirdPerson;
		}
		else 
		{
            _currentMode = CameraMode.FirstPerson;
        }
    }

	public void SetCameraMode(CameraMode mode)
	{
		_currentMode = mode;
	}

	public CameraMode GetCameraMode()
	{
		return _currentMode;
	}

	public void SetCamPos(Vector3 newPos)
	{
		transform.position = newPos;
	}

	// avant que la course commence, la caméra montre la map
	private void Intro() {}

	// caméra attaché juste devant le spaceship
	private void FirstPerson() {
		transform.position = _plyPos.position + (_plyPos.forward * 0.5f);
		transform.rotation = _plyPos.rotation;
	}

	// caméra attaché derrière le spaceship en hauteur
	// mouvement latéral? https://github.com/phoboslab/wipeout-rewrite/blob/90702ce17115484b6cfc1155dd4617b5fa3762cd/src/wipeout/camera.c#L42
	private void ThirdPerson() {
		transform.position = _plyPos.position + (_plyPos.forward * -1.5f) + (_plyPos.up * 0.3f);
		transform.rotation = _plyPos.rotation;
	}
	
	// quand la course est fini
	private void Spectate()
	{
		Quaternion targetRotation = Quaternion.LookRotation(_plyPos.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _camRotSpeed * Time.deltaTime);
	}

	// méthode public pour faire shaker la caméra
	public void Shake()
	{

	}
	
}
