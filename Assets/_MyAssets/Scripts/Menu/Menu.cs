using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using System.IO;
using System.Linq;

public class Menu : MonoBehaviour
{
	public static Menu Instance; // Singleton

	/******** CAMERA ********/
	private const float CAM_SPEED = 5f;
	private bool _isCameraMoving = false;
	private int _pos = 0;
	private Transform _cam;
	private List<Transform> _camList = new List<Transform>();	// doit être Transform pour qu'on puisse aussi prendre sa rotation

	/******** UI ELEMENTS ********/
	private const float BUTTON_SPEED = 6f;
	private List<UIElement> _listUIElement = new List<UIElement>();
	private string[] _arrUIElementNames = { "choix", "bt_retour", "bt_play", "options", "leaderboard", "credits" };

	/******** OPTIONS ********/
	// tracks
	private readonly HashSet<string> AVAILABLE_TRACKS = new HashSet<string> { "test_track_loop 1", "Test_Vincent", "Test_David" };
	private List<string> _listSceneName = new List<string>();
	private int _trackIndex = 0;
	private TextMeshProUGUI _txtTrackName;
	// nb laps
	private int _numLap = 1;
	private TextMeshProUGUI _txtNumLap;

	/******** LEADERBOARD ********/
	private TextMeshProUGUI _txtLeaderboard;

	/******** PLAY BUTTON ********/
	private bool _playCalledOnce = true;
	private Fade _fadeOut;
	private AudioFade _music;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_fadeOut = GameObject.Find("FadeOut").GetComponent<Fade>();
		_music = GameObject.Find("Opening").GetComponent<AudioFade>();

		// camera
		_cam = GameObject.Find("Main Camera").transform;
		foreach (Transform camPos in GameObject.Find("CamPosList").transform) {
			camPos.GetComponent<MeshRenderer>().enabled = false;
			_camList.Add(camPos);
		}

		// UIElement
		foreach (string name in _arrUIElementNames) {
			_listUIElement.Add(UIElementInit(name));
		}

		// options
		_txtNumLap = GameObject.Find("txt_nbLap").GetComponent<TextMeshProUGUI>();
		_txtTrackName = GameObject.Find("txt_trackName").GetComponent<TextMeshProUGUI>();

		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);			
			if (AVAILABLE_TRACKS.Contains(sceneName)) {
				_listSceneName.Add(sceneName);
				//Debug.Log($"found track: {name}");
			}
		}

		// leaderboard
		_txtLeaderboard = GameObject.Find("txt_leaderboard").GetComponent<TextMeshProUGUI>();
		_txtLeaderboard.text = "";
	}

	void Start()
	{
		SetTrack(0);	// default track
		GameData.Instance.SetNumLap(_numLap);
		InitLeaderboard();
	}

	void Update()
	{
		if(_isCameraMoving) {
			_cam.position += (_camList[_pos].position - _cam.position) * Time.deltaTime * CAM_SPEED;
			_cam.rotation = Quaternion.Slerp(_cam.rotation, _camList[_pos].rotation, CAM_SPEED * Time.deltaTime);
		}

		UpdateUIElementPos();
	}

	// UI elements only, NOT THE CAMERA
	private void UpdateUIElementPos()
	{
		foreach (UIElement uiElement in _listUIElement) {
			uiElement.tf.localPosition += (uiElement.obTargetPos - uiElement.tf.localPosition) * Time.deltaTime * BUTTON_SPEED;
		}
	}

	private UIElement UIElementInit(string obName)
	{
		Transform tf = GameObject.Find(obName).transform;
		UIElement uiElement = new UIElement(obName, tf, tf.localPosition, Vector3.zero);
		uiElement.obTargetPos = obName != "bt_play" ? new Vector3(-1400f, uiElement.obPos.y, uiElement.obPos.z) : new Vector3(uiElement.obPos.x, -700f, uiElement.obPos.z);  
		uiElement.tf.localPosition = uiElement.obTargetPos;
		return uiElement;
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

	public void Quitter()
	{
		Debug.Log("QUITTER() CALLED");
		Application.Quit();
	}

	// on peut pas mettre plus de deux arguments dans l'inspecteur sur OnClick()
	// donc on a deux méthodes, Show et Hide
	public void ShowUIElement(string name)
	{
		MoveUIElement(name, true);
	}

	public void HideUIElement(string name)
	{
		MoveUIElement(name, false);
	}

	private void MoveUIElement(string name, bool boolShow)
	{
		for (int i = 0; i < _listUIElement.Count; i++) {	// cant pass value by reference dans un foreach
			UIElement uiElement = _listUIElement[i];
			if(uiElement.obName == name) {
				uiElement.obTargetPos = boolShow ? uiElement.obPos : new Vector3(-1400f, uiElement.obPos.y, uiElement.obPos.z);
				_listUIElement[i] = uiElement;
				return;
			}
		}
	}

	public void NextMapButton()
	{
		_trackIndex++;
		if(_trackIndex >= _listSceneName.Count) {
			_trackIndex = 0;
		}
		SetTrack(_trackIndex);
	}

	private void SetTrack(int trackIndex)
	{
		string trackName = _listSceneName[_trackIndex];//"test_track_loop 1";
		GameData.Instance.SetTrackName(trackName);
		_txtTrackName.text = trackName;
		//Debug.Log($"selected map: {trackName}");
	}

	public void IncrementLapNum()
	{
		_numLap = _numLap > 5 ? 1 : _numLap + 1;
		string text = _txtNumLap.text;
		text = text.Substring(0, 7) + _numLap.ToString() + text.Substring(8);
		_txtNumLap.text = text;
		GameData.Instance.SetNumLap(_numLap);
	}

	public void PlayButton()
	{
		if(!_playCalledOnce) {
			return;
		}
		_playCalledOnce = false;

		StartCoroutine(FadeOutPlayCoroutine());
	}

	private void InitLeaderboard()
	{
		if(!GameData.Instance) {
			return;
		}

		string strSaveFilePath = GameData.Instance.GetSavePath();

		if(!File.Exists(strSaveFilePath)) {
			_txtLeaderboard.text = "AUCUN SCORE";
			return;
		}

		List<SaveData> saveList = JsonUtility.FromJson<SaveDataList>(File.ReadAllText(strSaveFilePath)).saves;

		// formated leaderboard
		Dictionary<string, SortedDictionary<int, string>> fmtLB = new Dictionary<string, SortedDictionary<int, string>>();

		foreach(SaveData saveData in saveList) {
			string trackName = saveData.trackName;
			string formatedTotalTime = FormatTime(saveData.lapTimes.Sum());
			string formatedPlyName = saveData.plyName.ToUpper().PadRight(12, '_');		// l'inputfield utilisé pour enregistrer son nom a 8 char max
			string formatedScore = formatedPlyName + formatedTotalTime;
			int nbLap = saveData.lapTimes.Count;

			if(!fmtLB.ContainsKey(trackName)) {
				fmtLB.Add(trackName, new SortedDictionary<int, string> { { nbLap, formatedScore } });
			} else {
				fmtLB[trackName][nbLap] = formatedScore;
			}
		}

		/*foreach (var track in fmtLB) {
			Debug.Log("track: " + track.Key);
			foreach (var nbLaps in track.Value) {
				Debug.Log("nbLaps: " + nbLaps.Key + ", ply and his score: " + nbLaps.Value);
			}
		}*/

		foreach (var track in fmtLB) {
			_txtLeaderboard.text += $"{track.Key}<br>";
			foreach (var nbLaps in track.Value) {
				_txtLeaderboard.text += $"        {nbLaps.Key} tours:        {nbLaps.Value}<br>";
			}
			_txtLeaderboard.text += "<br>";
		}
	}

	// format time 00:00:00
	private string FormatTime(float time)
	{
		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);
		int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);
	}

	IEnumerator FadeOutPlayCoroutine()
	{
		_fadeOut.ToggleFade(false);
		_music.ToggleFade(false);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene("LoadingScreen");
	}
}