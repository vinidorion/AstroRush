using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoadingScreen : MonoBehaviour
{
	public static AsyncLoadingScreen Instance;

	private AsyncOperation _scene;
	private GameData _gameData;
	
	private LoadingScreenLogo _logo;
	private bool _canSkip = false;
	
	private Fade _fadeOut;
	private AudioFade _audio;

	private ShineText _txt;

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		_gameData = GameData.Instance;
		_logo = GameObject.Find("logo").GetComponent<LoadingScreenLogo>();
		_txt = GameObject.Find("TxtSkip").GetComponent<ShineText>();

		_fadeOut = GameObject.Find("FadeOut").GetComponent<Fade>();
		AudioSource audio = GameObject.Find("Ambient").GetComponent<AudioSource>();
		audio.time = 176f;
		audio.volume = 0f;
		_audio = audio.GetComponent<AudioFade>();
		_audio.ToggleFade(true);
	}

	void Start()
	{
		if(!_gameData) {
			Debug.Log("NO GAMEDATA OBJECT, NO SCENE TO LOAD");
			Destroy(this.gameObject);
			return;
		}

		_scene = SceneManager.LoadSceneAsync(/*"test_track_loop 1"*/_gameData.GetTrackName()); // TODO: ALLER CHERCHER LE NOM DE LA SCENE DANS GAMEDATA
		_scene.allowSceneActivation = false;
		_logo.StartLogoAnim();
	}

	void Update()
	{
		if(Input.anyKeyDown && _canSkip) {
			StartCoroutine(LoadSceneCoroutine());
			_canSkip = false;
		}
	}

	// méthode publique qui permet de rendre le loading screen skipable
	public void CanSkip()
	{
		_canSkip = true;
		_txt.Shine();
	}

	// coroutine qui fade to black et fade out l'audio, puis load la scene
	IEnumerator LoadSceneCoroutine()
	{
		_fadeOut.ToggleFade(false);
		_audio.ToggleFade(false);
		yield return new WaitForSeconds(1f);
		_scene.allowSceneActivation = true;
	}
}
