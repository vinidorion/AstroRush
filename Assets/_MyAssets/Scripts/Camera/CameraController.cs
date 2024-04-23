using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance; // Singleton

	private CameraMode _currentMode;
	private Transform _plyPos;

	/***** SPECTATE *****/
	private const float CAM_ROT_SPEED = 10f;

	/***** INTRO *****/
	private const float CAM_SPEED = 0.5f;
	private const float COOLDOWN = 2f;
	private List<Transform> _listCam = new List<Transform>();
	private int _camIndex = 0;
	private float _nextCooldown;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		foreach (Transform child in GameObject.Find("IntroAnim").transform) {
			_listCam.Add(child);
			child.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	void Start()
	{
		FindPly();

		_nextCooldown = Time.time + COOLDOWN;
		_currentMode = CameraMode.Intro;
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

	// méthode privée qui trouve le transform du joueur
	private void FindPly() {
		if(Player.Instance) {
			_plyPos = Player.Instance.transform;
		} else {
			Debug.Log("PLAYER NOT FOUND");
		}
	}

	// méthode publique qui switch entre le mode firstperson et thirdperson
	public void RotateCameraMode()
	{
		if (_currentMode == CameraMode.FirstPerson) {
			_currentMode = CameraMode.ThirdPerson;
		} else if(_currentMode == CameraMode.ThirdPerson) {
			_currentMode = CameraMode.FirstPerson;
		}
	}

	// méthode publique qui change le CameraMode
	// enum CameraMode:
		// Intro
		// FirstPerson
		// ThirdPerson
		// Spectate
	public void SetCameraMode(CameraMode mode)
	{
		_currentMode = mode;
	}

	// méthode publique qui retourne le CameraMode actuel
	public CameraMode GetCameraMode()
	{
		return _currentMode;
	}

	// avant que la course commence, la caméra montre la map
	private void Intro() {
		if(Time.time > _nextCooldown) {
			_camIndex += 2;
			if(_camIndex >= _listCam.Count) {
				//_camIndex = 0; -- uncomment to loop throught camera pairs
				GameManager.Instance.StartRace();
				return;
			}
			transform.position = _listCam[_camIndex].position;
			transform.LookAt(transform.position + _listCam[_camIndex].forward);
			_nextCooldown = Time.time + COOLDOWN;
		}
		transform.position += (_listCam[_camIndex + 1].position - transform.position) * Time.deltaTime * CAM_SPEED;
		transform.rotation = Quaternion.Slerp(transform.rotation, _listCam[_camIndex + 1].rotation, CAM_SPEED * Time.deltaTime);
	}

	// caméra attaché juste devant le spaceship
	private void FirstPerson() {
		transform.position = _plyPos.position + (_plyPos.forward * 0.5f);
		transform.rotation = _plyPos.rotation;
	}

	// caméra attaché derrière le spaceship en hauteur
	private void ThirdPerson() {
		transform.position = _plyPos.position + (_plyPos.forward * -1.5f) + (_plyPos.up * 0.3f);
		transform.rotation = _plyPos.rotation;
	}

	// quand la course est fini
	private void Spectate()
	{
		Quaternion targetRotation = Quaternion.LookRotation(_plyPos.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, CAM_ROT_SPEED * Time.deltaTime);
	}

	// méthode public pour faire shaker la caméra
	public void Shake()
	{
		if(_currentMode != CameraMode.FirstPerson && _currentMode != CameraMode.ThirdPerson) { // ne pas faire l'effet sur Intro ou Spectate
			return;
		}

		// shake ici
	}
}
