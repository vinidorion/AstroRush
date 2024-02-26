using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
	public static VolumeManager Instance; // Singleton

	// constantes
	private const int MAX_VOL = 10;
	private const int MIN_VOL = 0;
	
	// variables
	private int _GlobalVolSFX = 10;
	private int _GlobalVolMUS = 10;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	// incremente le volume SFX (sound effects)
	// isIncrease:
	//    true	- augmenter de 1
	//    false	- baisser de 1
	public void IncrementVolSFX(bool isIncrease)
	{
		if(isIncrease) {
			_GlobalVolSFX = Mathf.Clamp(_GlobalVolSFX + 1, MIN_VOL, MAX_VOL);
		} else {
			_GlobalVolSFX = Mathf.Clamp(_GlobalVolSFX - 1, MIN_VOL, MAX_VOL);
		}
		UpdateVolume();
	}

	// incremente le volume MUS (musique)
	// isIncrease:
	//    true	- augmenter de 1
	//    false	- baisser de 1
	public void IncrementVolMUS(bool isIncrease)
	{
		if(isIncrease) {
			_GlobalVolMUS = Mathf.Clamp(_GlobalVolMUS + 1, MIN_VOL, MAX_VOL);
		} else {
			_GlobalVolMUS = Mathf.Clamp(_GlobalVolMUS - 1, MIN_VOL, MAX_VOL);
		}
		UpdateVolume();
	}

	// retourne le volume global SFX
	public int GetVolSFX()
	{
		return _GlobalVolSFX;
	}

	// retourne le volume global MUS
	public int GetVolMUS()
	{
		return _GlobalVolMUS;
	}

	// à utiliser chaque fois qu'on load scene
	// ou qu'on change le volume
	public void UpdateVolume()
	{
		// loop through all audio sources,
		// if tag is SFX: set .volume to _GlobalVolSFX
		// if tag is MUS: set .volume to _GlobalVolMUS
		// else: Debug.Log(audiosource + " n'a pas de tag SFX/MUS")
	}
}
