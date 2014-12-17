using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour {

	public AudioSource music;


	void Awake () {


		music = gameObject.GetComponent<AudioSource>();
		MusicManagerSingleton.play(music);
		//MusicManagerSingleton.Instance.playMenuMusic();


	}
	

}
