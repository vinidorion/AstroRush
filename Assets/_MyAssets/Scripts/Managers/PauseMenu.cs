using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance; // Singleton

	private bool _isPaused = false;
	private bool _canPause = true; // DOIT ÊTRE FALSE PAR DÉFAUT, (GameManager le set false)

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	void Update()
	{
		if(Input.GetKeyDown("escape")) {
			TogglePause();
		}
	}

	public void TogglePause()
	{
		if(_canPause) { // va probablement être utilisé en dehors de cette classe, donc check _canPause ici
			if(_isPaused) {
				Time.timeScale = 1f;
				_isPaused = false;
			} else {
				Time.timeScale = 0f;
				_isPaused = true;
			}
		}
	}

	// TogglePause() overloaded
	// true:	pause
	// false:	unpause
	public void TogglePause(bool pause)
	{
		if(_canPause) {
			_isPaused = pause;

			if(pause) {
				Time.timeScale = 0f;
			} else {
				Time.timeScale = 1f;
			}
		}
	}

	public void SetCanPause(bool canPause)
	{
		_canPause = canPause;
	}

	public bool GetIsPaused()
	{
		return _isPaused;
	}
}
