using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private Transform _play;
	private Vector3 _playPos;
	private Vector3 _playTargetPos;

	private bool _playCalledOnce = true;

	private Fade _fadeOut;
	private AudioFade _music;

	private Camera _camBackgroundColor;
	private bool _isFadingBackground = false;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_cam = GameObject.Find("Main Camera").transform;
		_fadeOut = GameObject.Find("FadeOut").GetComponent<Fade>();
		_music = GameObject.Find("Opening").GetComponent<AudioFade>();

		_camBackgroundColor = _cam.GetComponent<Camera>();
		_camBackgroundColor.clearFlags = CameraClearFlags.SolidColor;
		_camBackgroundColor.backgroundColor = Color.white;

		_choix = GameObject.Find("choix").transform; // keep this as a transform
		_mainMenuPos = _choix.localPosition;
		_mainMenuTargetPos = new Vector3(-1400f, _mainMenuPos.y, _mainMenuPos.z);
		_choix.localPosition = _mainMenuTargetPos;

		_retour = GameObject.Find("bt_retour").transform;
		_retourPos = _retour.localPosition;
		_retourTargetPos = new Vector3(-1400f, _retourPos.y, _retourPos.z);
		_retour.localPosition = _retourTargetPos;

		_play = GameObject.Find("bt_play").transform;
		_playPos = _play.localPosition;
		_playTargetPos = new Vector3(_playPos.x, -700f, _playPos.z);
		_play.localPosition = _playTargetPos;

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
		_play.localPosition += (_playTargetPos - _play.localPosition) * Time.deltaTime * BUTTON_SPEED;

		if(_isFadingBackground) {
			Color color = _camBackgroundColor.backgroundColor;
			float ratio = color.r * Time.deltaTime;
			color.r -= ratio;
			color.g -= ratio;
			color.b -= ratio;
			_camBackgroundColor.backgroundColor = color;

			if(color.r <= 0f) {
				_isFadingBackground = false;
			}
		}
	}

	public void ToggleCameraMovement(bool isCameraMoving)
	{
		_isCameraMoving = isCameraMoving;
	}

	public void FadeBackground()
	{
		_isFadingBackground = true;
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

	public void Quitter()
	{
		Application.Quit();
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

	public void ShowPlayMenu(bool showPlayMenu)
	{
		if(showPlayMenu) {
			_playTargetPos = _playPos;
		} else {
			_playTargetPos = new Vector3(_playPos.x, -700f, _playPos.z);
		}
	}

	public void PlayButton()
	{
		if(!_playCalledOnce) {
			return;
		}
		_playCalledOnce = false;

		StartCoroutine(FadeOutPlayCoroutine());
	}

	IEnumerator FadeOutPlayCoroutine()
	{
		_fadeOut.ToggleFade(false);
		_music.ToggleFade(false);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene("LoadingScreen");
	}

	public void SetCameraTypeToSkyBox()
    {
		_camBackgroundColor.clearFlags = CameraClearFlags.Skybox;
    }
}
