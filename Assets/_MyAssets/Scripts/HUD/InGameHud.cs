using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHud : MonoBehaviour
{
	public static InGameHud Instance; // Singleton

	private SpaceShip _ship;
	private float _maxSpeed;
	[SerializeField] private Image _speedBar = default;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

    private void Start()
    {
		_ship = Player.Instance.GetSpaceShip();
		_maxSpeed = _ship.GetMaxSpeed();
    }

    void Update()
	{
		Speed();
	}


	// les fonctions dessous s'occupent de ceci: https://github.com/vinidorion/AstroRush/blob/main/Autre/ingame_hud.PNG

	// draw la vie du joueur
	private void HP()
	{

	}

	private void Speed()
	{
		_ship.GetSpeed();

		_speedBar.fillAmount = _ship.GetSpeed() / _maxSpeed;
		Color color = _speedBar.color;
        color.b = 1 - _ship.GetSpeed() / _maxSpeed;
		color.r = _ship.GetSpeed() / _maxSpeed;
        _speedBar.color = color;

		Debug.Log(_ship.GetSpeed() / _maxSpeed);
    }

	// draw la position (premier, deuxième, etc)
	private void Pos()
	{

	}

	// lap time et total time
	private void Time()
	{
		
	}

	// draw l'icone de l'item du joueur
	private void Item()
	{

	}

	// draw la map en 2D (vue de haut)
	private void Map()
	{

	}

	// draw la progression sur la track (utiliser le current checkpoint du joueur sur le nb total de checkpoint)
	private void Prog()
	{

	}

	// draw time comparison, your current lap time vs your best lap time
	// s'affiche pendant quelques secondes
	// rouge si plus lent (ex: +00:01:00 si une seconde plus long que le meilleur score)
	// vert si plus rapide (ex: -00:01:00 si une seconde plus court que le meilleur score)
	public void TimeComp(float timeDiff)
	{
		Debug.Log("Lap time: " + timeDiff);
		// faire timeDiff - son best lap time pour trouver la comparaison
		// si > 0, color = red, si < 0, color = green, etc
	}

}
