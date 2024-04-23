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
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
	}

	// // méthode publique qui incremente le volume SFX (sound effects)
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

	// // méthode publique qui incremente le volume MUS (musique)
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

	// // méthode publique qui retourne le volume global SFX
	public int GetVolSFX()
	{
		return _GlobalVolSFX;
	}

	// // méthode publique qui retourne le volume global MUS
	public int GetVolMUS()
	{
		return _GlobalVolMUS;
	}

	// méthode publique qui update le volume de tous les AudioSource dans la scene
	// à utiliser chaque fois qu'on load une nouvelle scene
	// ou qu'on change le volume
	public void UpdateVolume()
	{
		foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>()) {
			if(audioSource.tag == "SFX") {
				audioSource.volume = (float)_GlobalVolSFX / 10f;
			} else if(audioSource.tag == "MUS") {
				audioSource.volume = (float)_GlobalVolMUS / 10f;
			} else {
				Debug.Log(audioSource + " HAS NO TAG");
			}
		}
	}
}
