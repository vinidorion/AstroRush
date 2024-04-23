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

	private SpaceShip _ship;
	private float _maxSpeed;
	[SerializeField] private Image _speedBar = default;
	[SerializeField] private TMP_Text _speedText = default;
	[SerializeField] private TMP_Text _LapsText = default;
	[SerializeField] private TMP_Text _PosText = default;
	[SerializeField] private TMP_ColorGradient[] color_list = default;
	[SerializeField] private TMP_Text _LapTimeText = default;
	[SerializeField] private Image _itemImage = default;
	private float[] _lapTimes_list = default;
	private string _lapTimes_string = "";
	private float _time_lap_start = 0;

	private Sprite[] _arrPUs;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
		_arrPUs = Resources.LoadAll("PUs/Images/", typeof(Sprite)).Cast<Sprite>().ToArray();
	}

	void Start()
	{
		_ship = Player.Instance.GetSpaceShip();
		_maxSpeed = _ship.GetMaxSpeed();
	}

	void FixedUpdate()
	{
		CameraMode camMode = CameraController.Instance.GetCameraMode();
		if(camMode == CameraMode.Intro || camMode == CameraMode.Spectate) {
			return; // ne pas draw le hud dans l'intro ou spectate
		}
		Speed();
		Laps();
		Pos();
		LapTimer();
	}

	// les fonctions dessous s'occupent de ceci: https://github.com/vinidorion/AstroRush/blob/main/Autre/ingame_hud.PNG

	// draw la vie du joueur
	private void HP()
	{

	}

	private void Speed()
	{
		float speed = _ship.GetForwardSpeed();


		_speedBar.fillAmount = speed * .8f / _maxSpeed;
		Color color = _speedBar.color;
		color.b = 1 - speed / _maxSpeed;
		color.r = speed / _maxSpeed;
		_speedBar.color = color;
		if (speed < _maxSpeed) _speedText.text = ((int)(speed / _maxSpeed * 100)).ToString();
		else _speedText.text = "99";
	}

	private void Laps()
	{
		_LapsText.text = "Lap " + (_ship.GetLap() + 1).ToString();
	}

	// draw la position (premier, deuxième, etc)
	private void Pos()
	{
		int pos = _ship.GetPosition() + 1;
		if (pos == 1) { _PosText.text = "1st"; _PosText.colorGradientPreset = color_list[0]; }
		else if (pos == 2) { _PosText.text = "2nd"; _PosText.colorGradientPreset = color_list[1]; }
		else if (pos == 3) { _PosText.text = "3rd"; _PosText.colorGradientPreset = color_list[2]; }
		else { _PosText.text = pos + "th"; _PosText.colorGradientPreset = color_list[3]; }
	}

	// lap time
	private void LapTimes(float time)
	{
		_lapTimes_list[_lapTimes_list.Length] = time;

		string text = "";
		foreach(float t in _lapTimes_list)
		{
			text = text + "\n" + t;
		}
		_lapTimes_string = text;
		_time_lap_start = Time.time;
	}

	private void LapTimer()
	{
		_LapTimeText.text = _lapTimes_string + "\n" + (Time.time - _time_lap_start);
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

	// draw la map en 2D (vue de haut)
	private void Map()
	{

	}

	// draw la progression sur la track (utiliser le current waypoint du joueur sur le nb total de waypoint)
	private void Prog()
	{

	}

	// draw time comparison, your current lap time vs your best lap time
	// s'affiche pendant quelques secondes
	// rouge si plus lent (ex: +00:01:00 si une seconde plus long que le meilleur score)
	// vert si plus rapide (ex: -00:01:00 si une seconde plus court que le meilleur score)
	public void TimeComp(float timeDiff)
	{
		LapTimes(timeDiff);
		Debug.Log($"Lap time: {timeDiff}");
		// faire timeDiff - son best lap time pour trouver la comparaison
		// si > 0, color = red, si < 0, color = green, etc
	}

}
