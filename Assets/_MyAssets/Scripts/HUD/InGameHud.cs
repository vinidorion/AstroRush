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
	private string _lapTimes_string = "";
	private string last_lap_time = "";

	// progression bar
	private Image _progBar;

	//HealthBar
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
		
		foreach(Transform child in transform) {
			if(child.name == "Speed_Bar") {
				foreach(Transform grandchild in child) {
					if(grandchild.name == "Speed_Bar_Img") {
						_speedBar = grandchild.GetComponent<Image>();
					} else if(grandchild.name == "Speed_Text") {
						_speedText = grandchild.GetComponent<TMP_Text>();
					}
				}
			} else if(child.name == "Laps_Text") {
				_lapsText = child.GetComponent<TMP_Text>();
			} else if(child.name == "Pos_Text") {
				_posText = child.GetComponent<TMP_Text>();
			} else if(child.name == "LapTimes_Text") {
				_lapTimeText = child.GetComponent<TMP_Text>();
			} else if(child.name == "Item_Image") {
				_itemImage = child.GetComponent<Image>();
			} else if(child.name == "ProgressionBar") {
				foreach(Transform grandchild in child) {
					if(grandchild.name == "ProgressionBarFill") {
						_progBar = grandchild.GetComponent<Image>();
					}
				}
            }
            else if (child.name == "HealthBar") {
                foreach (Transform grandchild in child) {
                    if (grandchild.name == "HealthBarFill") {
                        _healthBar = grandchild.GetComponent<Image>();
                    }
                }
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
		Speed();
		Pos();
		LapTimer();
		ProgressionBar();
	}

	// les fonctions dessous s'occupent de ceci: https://github.com/vinidorion/AstroRush/blob/main/Autre/ingame_hud.PNG

	// draw la vie du joueur
	private void HP()
	{
		int hp = _plyShip.GetHP();
		_healthBar.fillAmount = hp / maxHP;
	}

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
		float lapTime = _plyShip.GetTimeSinceLastLap();
		int minutes = Mathf.FloorToInt(lapTime / 60f);
		int seconds = Mathf.FloorToInt(lapTime % 60f);
		int milliseconds = Mathf.FloorToInt((lapTime * 1000f) % 1000f);
		_lapTimes_string = "";
        foreach (string time in _lapTimes_list) 
		{ 
			_lapTimes_string = _lapTimes_string + "\n" + time;
        }
		_lapTimeText.text = _lapTimes_string + "\n" + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);
		last_lap_time = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);

    }

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

	// draw l'icone de l'item du joueur
	public void Item(int pu)
	{
		if (pu >= 0 && pu < _arrPUs.Length)
		{
			_itemImage.sprite = _arrPUs[pu];
		}
		else
		{
			_itemImage.sprite = null;
		}
	}

	// TODO: caller à la fin de LapCompleted() pour être sûr d'obtenir le bon GetTimeSinceLastLap()
	// draw time comparison, your current lap time vs your best lap time
	// s'affiche pendant 2s
	// rouge si plus lent (ex: +00:01:00 si une seconde plus long que le meilleur score)
	// vert si plus rapide (ex: -00:01:00 si une seconde plus court que le meilleur score)
	public void TimeComp()
	{
		//LapTimes(timeDiff);

		// faire GetLastLapTime() - son best lap time pour trouver la comparaison
		// si > 0, color = red, si < 0, color = green, etc
	}

	// méthode publique qui permet de turn on/off le hud
	// (c'est juste l'alpha du canvasgroup qui et set à 0 ou 1)
	// true :	afficher
	// false :	enlever
	public void ToggleDrawHUD(bool drawHUD)
	{
		_canvasGroup.alpha = drawHUD ? 1f : 0f;
	}
}
