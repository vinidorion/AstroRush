using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public static Menu Instance; // Singleton

	private bool _isCameraMoving = false;
	private int _pos = 0;
	private Transform _cam;

	private List<Transform> _camList = new List<Transform>();

	private const float BUTTON_SPEED = 6f;
	private const float CAM_SPEED = 5f;

	private Transform _choix;
	private Vector3 _mainMenuPos;
	private Vector3 _mainMenuTargetPos;

	private Transform _retour;
	private Vector3 _retourPos;
	private Vector3 _retourTargetPos;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_cam = GameObject.Find("Main Camera").transform;

		_choix = GameObject.Find("choix").transform; // keep this as a transform
		_mainMenuPos = _choix.localPosition;
		_mainMenuTargetPos = new Vector3(-1400f, _mainMenuPos.y, _mainMenuPos.z);
		_choix.localPosition = _mainMenuTargetPos;

		_retour = GameObject.Find("bt_retour").transform;
		_retourPos = _retour.localPosition;
		_retourTargetPos = new Vector3(-1400f, _retourPos.y, _retourPos.z);
		_retour.localPosition = _retourTargetPos;

		foreach (Transform camPos in GameObject.Find("CamPosList").transform) {
			camPos.GetComponent<MeshRenderer>().enabled = false;
			_camList.Add(camPos);
		}
	}

	void Update()
	{
		if(_isCameraMoving) {
			_cam.position += (_camList[_pos].position - _cam.position) * Time.deltaTime * CAM_SPEED;
			_cam.rotation = Quaternion.Slerp(_cam.rotation, _camList[_pos].rotation, CAM_SPEED * Time.deltaTime);
		}

		_choix.localPosition += (_mainMenuTargetPos - _choix.localPosition) * Time.deltaTime * BUTTON_SPEED;
		_retour.localPosition += (_retourTargetPos - _retour.localPosition) * Time.deltaTime * BUTTON_SPEED;
	}

	public void ToggleCameraMovement(bool isCameraMoving)
	{
		_isCameraMoving = isCameraMoving;
	}

	public void SetCamPos(int pos)
	{
		_pos = pos;
	}

	public void ResetAllButtonAnimLen()
	{
		foreach(ButtonAnim button in FindObjectsOfType<ButtonAnim>()) {
			button.Reset();
		}
	}

	public void ShowMainMenu(bool showMainMenu)
	{
		if(showMainMenu) {
			_mainMenuTargetPos = _mainMenuPos;
		} else {
			_mainMenuTargetPos = new Vector3(-1400f, _mainMenuPos.y, _mainMenuPos.z);
		}
	}

	public void ShowReturnButton(bool showReturnButton)
	{
		if(showReturnButton) {
			_retourTargetPos = _retourPos;
		} else {
			_retourTargetPos = new Vector3(-1400f, _retourPos.y, _retourPos.z);
		}
	}
}
