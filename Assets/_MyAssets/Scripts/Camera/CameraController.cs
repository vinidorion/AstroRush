using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance; // Singleton

	private Camera _cam;
	private CameraMode _currentMode;
	private Transform _plyPos;

	/***** INTRO *****/
	private const float CAM_SPEED = 0.5f;
	private const float COOLDOWN = 2f;
	private List<Transform> _listCamIntro = new List<Transform>();
	private int _camIntroIndex = 0;
	private float _nextCooldown;

	/***** SPECTATE *****/
	private const float CAM_ROT_SPEED = 10f;
	private List<Vector3> _listCamSpecPos = new List<Vector3>();
	private int _camSpecIndex = 0;


	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_cam = GetComponent<Camera>();

		foreach (Transform child in GameObject.Find("IntroAnim").transform) {
			_listCamIntro.Add(child);
			child.GetComponent<MeshRenderer>().enabled = false;
		}
		foreach (Transform child in GameObject.Find("SpectateAnim").transform) {
			_listCamSpecPos.Add(child.position);
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
			Debug.Log(Player.Instance.gameObject.name);
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
			_camIntroIndex += 2;
			if(_camIntroIndex >= _listCamIntro.Count) {
				//_camIntroIndex = 0; -- uncomment to loop throught camera pairs
				GameManager.Instance.StartRace();
				return;
			}
			transform.position = _listCamIntro[_camIntroIndex].position;
			transform.LookAt(transform.position + _listCamIntro[_camIntroIndex].forward);
			_nextCooldown = Time.time + COOLDOWN;
		}
		transform.position += (_listCamIntro[_camIntroIndex + 1].position - transform.position) * Time.deltaTime * CAM_SPEED;
		transform.rotation = Quaternion.Slerp(transform.rotation, _listCamIntro[_camIntroIndex + 1].rotation, CAM_SPEED * Time.deltaTime);
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
		// rotation
		Quaternion targetRotation = Quaternion.LookRotation(_plyPos.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, CAM_ROT_SPEED * Time.deltaTime);
		
		// zoom
		float newFov = (Player.Instance.transform.position - transform.position).sqrMagnitude;
		newFov = 0.000663f * Mathf.Pow(newFov -300f, 2f);
		newFov = Mathf.Clamp(newFov, 7f, 60f);
		_cam.fieldOfView += (newFov - _cam.fieldOfView) * Time.deltaTime * 2f;

		// position
		if(_listCamSpecPos.Count == 0) {
			return;
		}

		int nextCamIndex = _camSpecIndex + 1;
		nextCamIndex = nextCamIndex >= _listCamSpecPos.Count ? 0 : nextCamIndex;

		float distCurrCam = (_plyPos.position - _listCamSpecPos[_camSpecIndex]).sqrMagnitude;
		float distNextCam = (_plyPos.position - _listCamSpecPos[nextCamIndex]).sqrMagnitude;

		//Debug.DrawLine(_plyPos.position, _listCamSpecPos[_camSpecIndex], Color.green, Time.deltaTime);
		//Debug.DrawLine(_plyPos.position, _listCamSpecPos[nextCamIndex], Color.blue, Time.deltaTime);

		if (distNextCam < distCurrCam) {
			_camSpecIndex++;
			if (_camSpecIndex >= _listCamSpecPos.Count) {
				_camSpecIndex = 0;
			}
			transform.position = _listCamSpecPos[_camSpecIndex];
			transform.LookAt(_plyPos.position, Vector3.up);
			_cam.fieldOfView = 60f;
		}
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
