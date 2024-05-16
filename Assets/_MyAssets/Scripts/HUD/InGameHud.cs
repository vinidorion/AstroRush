using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using System;

public class InGameHud : MonoBehaviour
{
	public static InGameHud Instance; // Singleton

	// speedbar
	private float _maxSpeed;
	private Image _speedBar;
	private TMP_Text _speedText;

	// laps
	private TMP_Text _lapsText;

	// position
	private TMP_Text _posText;
	[SerializeField] private TMP_ColorGradient[] color_list = default;
	private readonly string[] ORDINAL_INDICATORS = { "st", "nd", "rd" };

	// lap time
	private TMP_Text _lapTimeText;
	private Image _itemImage;
	private List<string> _lapTimes_list = new List<string>();
	private string last_lap_time = "";
	private float _time_lap_start = 0;
	private bool _hasStarted = false;

	// progression bar
	private Image _progBar;

	// health bar
	private Image _healthBar;
	private int maxHP = 0;


	private SpaceShip _plyShip;
	private WaypointFinder _plyWptFinder;
	private Sprite[] _arrPUs;
	private CanvasGroup _canvasGroup;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		// changer la propriété alpha de cette variable permet de turn on/off (1f / 0f) le hud au complet (tous les child objects de cet objet)
		_canvasGroup = GetComponent<CanvasGroup>(); 

		// va chercher tout les element du HUD pour pouvoir les utiliser individuellement
		foreach (Transform child in transform) {
			switch (child.name)
			{
				case "Speed_Bar":
					foreach (Transform grandchild in child) {
						if(grandchild.name == "Speed_Bar_Img") {
							_speedBar = grandchild.GetComponent<Image>();
						} else if(grandchild.name == "Speed_Text") {
							_speedText = grandchild.GetComponent<TMP_Text>();
						}
					}
					break;
				case "Laps_Text":
					_lapsText = child.GetComponent<TMP_Text>();
					break;
				case "Pos_Text":
					_posText = child.GetComponent<TMP_Text>();
					break;
				case "LapTimes_Text":
					_lapTimeText = child.GetComponent<TMP_Text>();
					break;
				case "Item_Image":
					_itemImage = child.GetComponent<Image>();
					break;
				case "ProgressionBar":
					foreach(Transform grandchild in child) {
						if(grandchild.name == "ProgressionBarFill") {
							_progBar = grandchild.GetComponent<Image>();
						}
					}
					break;
				case "HealthBar":
					foreach (Transform grandchild in child) {
						if (grandchild.name == "Image") {
							_healthBar = grandchild.GetComponent<Image>();
						}
					}
					break;
			}
		}

		_arrPUs = Resources.LoadAll("PUs/Images/", typeof(Sprite)).Cast<Sprite>().ToArray();
	}

	void Start()
	{
		_plyShip = Player.Instance.GetComponent<SpaceShip>();
		_plyWptFinder = _plyShip.GetComponent<WaypointFinder>();
		_maxSpeed = _plyShip.GetMaxSpeed();
		maxHP = _plyShip.GetMaxHP();
	}

	void FixedUpdate()
	{
		CameraMode camMode = CameraController.Instance.GetCameraMode();
		if(camMode == CameraMode.Intro || camMode == CameraMode.Spectate) {
			return; // ne pas draw le hud dans l'intro ou spectate
		}
		HP();
		Speed();
		Pos();
		if(_hasStarted) {
			LapTimer();
		}
		ProgressionBar();
	}

	// les fonctions dessous s'occupent de ceci: https://github.com/vinidorion/AstroRush/blob/main/Autre/ingame_hud.PNG

	// draw la vie du joueur
	private void HP()
	{
		int hp = _plyShip.GetHP();
		_healthBar.fillAmount = hp / (float)maxHP;
	}

	// affiche la vitesse du joueur avec un nombre et avec la barre de progression
	private void Speed()
	{
		float speed = _plyShip.GetForwardSpeed();


		_speedBar.fillAmount = speed * .8f / _maxSpeed;
		Color color = _speedBar.color;
		color.b = 1 - speed / _maxSpeed;
		color.r = speed / _maxSpeed;
		_speedBar.color = color;
		if (speed < _maxSpeed) _speedText.text = ((int)(speed / _maxSpeed * 100)).ToString();
		else _speedText.text = "99";
	}

	// draw la position (premier, deuxième, etc)
	private void Pos()
	{
		int pos = _plyShip.GetPosition();

		_posText.text = (pos + 1).ToString();

		if(pos < 3) {
			_posText.colorGradientPreset = color_list[pos];
			_posText.text += ORDINAL_INDICATORS[pos];
		} else {
			_posText.colorGradientPreset = color_list[3];
			_posText.text += "th";
		}
	}

	// lap time
	private void LapTimer()
	{
		string strLapTime = "";
		foreach (string time in _lapTimes_list) { 
			strLapTime += "\n" + time;
		}

		string formatedStr = FormatTime(_plyShip.GetTimeSinceLastLap());

		_lapTimeText.text = strLapTime + "\n" + formatedStr;
		last_lap_time = formatedStr;
	}

	// TODO: delete si on s'en sert pas
	// draw la map en 2D (vue de haut)
	private void Map()
	{

	}

	// draw la progression sur la track (utiliser le current waypoint du joueur sur le nb total de waypoint)
	private void ProgressionBar()
	{
		float value = (float)_plyWptFinder.GetWaypoint() / WaypointManager.Instance.GetNbWpt();
		float diff = value - _progBar.fillAmount;
		_progBar.fillAmount += diff >= 0f ? diff * Time.fixedDeltaTime : Time.fixedDeltaTime;
	}

	// méthode publique pour reset la barre de progression
	public void ResetProgBar()
	{
		_progBar.fillAmount = 0f;
	}

	// méthode publique pour informer cette classe que la course a commencé
	public void StartTimer()
	{
		_hasStarted = true;
	}

	// méthode publique pour set le numéro du lap
	// called dans LapCompleted()
	// pour éviter de set le lap 50x par seconde
	// on passe pas le _lap par argument pour être
	// capable de le caller de n'importe où
	public void UpdateLap()
	{
		_lapsText.text = "Lap " + (_plyShip.GetLap() + 1).ToString();

		float lapTime = _plyShip.GetTimeSinceLastLap();
		int minutes = Mathf.FloorToInt(lapTime / 60f);
		int seconds = Mathf.FloorToInt(lapTime % 60f);
		int milliseconds = Mathf.FloorToInt((lapTime * 1000f) % 1000f);
		_lapTimes_list.Add(last_lap_time);
	}

	// draw l'icone de l'item du joueur
	// (check si c'est within range, otherwise return null)
	public void Item(int pu)
	{
		bool isWithinRange = pu >= 0 && pu < _arrPUs.Length;

		_itemImage.sprite = isWithinRange ? _arrPUs[pu] : null;
		_itemImage.color = isWithinRange ? Color.white : Color.clear;
	}

	// draw time comparison, your current lap time vs your best lap time
	// s'affiche pendant 2s (voir la classe TimeComp)
	// rouge si plus lent (ex: +00:01:00 si une seconde plus long que le meilleur score)
	// vert si plus rapide (ex: -00:01:00 si une seconde plus court que le meilleur score)
	public void DrawTimeComp()
	{
		//LapTimes(timeDiff);
		Debug.Log($"GetLastLapTime() : {_plyShip.GetLastLapTime()}");

		float myLap = _plyShip.GetLastLapTime();
		float bestLap = 0f;		// TODO: trouver best lap

		bool boolColor = myLap < bestLap;

		string timeComp = (boolColor ? "-" : "+") + FormatTime(Mathf.Abs(myLap - bestLap));

		TimeComp.Instance.DrawTimeComp(timeComp, boolColor);
	}

	// méthode publique qui permet de turn on/off le hud
	// (c'est juste l'alpha du canvasgroup qui est set à 0 ou 1)
	// true :	afficher
	// false :	enlever
	public void ToggleDrawHUD(bool drawHUD)
	{
		_canvasGroup.alpha = drawHUD ? 1f : 0f;
	}

	// format time 00:00:00
	// mettre publique si on veut format le temps comme ça en dehors de cette classe
	private string FormatTime(float time)
	{
		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);
		int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);
	}

	public string GetPosOrdinal() { return _posText.text; }
}
