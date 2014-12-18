using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour {

	public AudioClip musicClip;
	
	void Start () {
		MusicManagerSingleton.Instance.playMusic(musicClip);
	}
	

}
