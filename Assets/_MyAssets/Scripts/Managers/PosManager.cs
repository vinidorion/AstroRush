using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosManager : MonoBehaviour
{
	public static PosManager Instance; // Singleton
	
	private float _nextUpdate = 0f;
	private const float COOLDOWN = 1f;
	
	private List<SpaceShip> _listSpaceship = new List<SpaceShip>(); 

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
		// get la liste de spaceship dans le start
		// sort la même liste à chaque fois
		foreach (SpaceShip spaceship in FindObjectsOfType<SpaceShip>()) {
			_listSpaceship.Add(spaceship);
		}
	}

	void Update()
	{
		if(_nextUpdate < Time.time) { // expensive, mettre un cooldown sur l'opération
			SortSpaceshipList();
			_nextUpdate = Time.time + COOLDOWN;
		}
	}

	// méthode privée qui met en ordre décroissant les spaceship
	// en fonction de leurs "valeur" de position (GetPosValue())
	private void SortSpaceshipList()
	{
		_listSpaceship.Sort((a, b) => {
			return b.GetPosValue().CompareTo(a.GetPosValue());
		});
		for(int i = 0; i < _listSpaceship.Count; i++) {
			_listSpaceship[i].SetPosition(i);
		}
		//PrintList();
	}

	// pour debug
	// print la liste _listSpaceship en un string
	private void PrintList()
	{
		string str = string.Empty;
		foreach (SpaceShip spaceship in _listSpaceship) {
			str += spaceship.GetPosition() + " : " + spaceship + "\n";
		}
		Debug.Log(str);
	}

	// méthode publique qui obtient le spaceship associé à la position
	public SpaceShip GetShipFromPos(int pos)
	{
		return _listSpaceship[pos];
	}

	// méthode publique qui obtient le dernier spaceship
	public SpaceShip GetLastShip()
	{
		return _listSpaceship[_listSpaceship.Count - 1];
	}
}
