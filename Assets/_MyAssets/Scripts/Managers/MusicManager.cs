using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MusicManager : MonoBehaviour
{
	private AudioClip[] _listMusic;
	private AudioSource _audio;
	private int _musicIndex = 0;

	void Awake()
	{
		_listMusic = Resources.LoadAll("Music/", typeof(AudioClip)).Cast<AudioClip>().ToArray();

		/*foreach(AudioClip music in _listMusic) {
			Debug.Log(music);
		}*/

		ShuffleMusicList();

		_audio = GameObject.Find("MusicAudioSource").GetComponent<AudioSource>(); // ne pas oublier de mettre le tag MUS sur cet objet dans l'inspecteur
		
	}

	void Start()
	{

		// start music here
		// on race start or on scene start???
	}

	private void ShuffleMusicList()
	{
		System.Random rnd = new System.Random();

		for (int i = 0; i < _listMusic.Length; i++) {
			int randomIndex = rnd.Next(i + 1);

			AudioClip temp = _listMusic[randomIndex];
			_listMusic[randomIndex] = _listMusic[i];
			_listMusic[i] = temp;
		}
	}

	public void PauseMusic()
	{
		if(_audio.isPlaying) {
			_audio.Pause();
		} else {
			_audio.Play();
		}
	}

	// PauseMusic() overloaded
	// true:	pause
	// false:	unpause
	public void PauseMusic(bool pause)
	{
		if(pause) {
			_audio.Pause();
		} else {
			_audio.Play();
		}
	}

	IEnumerator StartMusicCoroutine()
	{
		_audio.clip = _listMusic[_musicIndex];
		_audio.Play();

		_musicIndex++;
		if(_musicIndex >= _listMusic.Length) {
			_musicIndex = 0;
			ShuffleMusicList();
		}

		yield return new WaitForSeconds(_audio.clip.length);
		StartCoroutine(StartMusicCoroutine());
	}

	// pour l'affichage de "currently playing" dans le menu pause
	public string GetCurrentMusicName()
	{
		return _audio.clip.name;
	}

}
