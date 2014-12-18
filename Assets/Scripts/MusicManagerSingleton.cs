using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	private static MusicManagerSingleton instance = null;
	private bool isMusicOn=true;
	AudioClip clip;
	public static MusicManagerSingleton Instance {
		get { 
			return instance;
		}
	}
		
	void Awake() {
		if (instance != null && instance != this) {
			Debug.Log ("Game object already created");
			Destroy(this.gameObject);
			return;
		} else {
			Debug.Log ("MusicManagerSingleton null");
			instance = this;
		}
		//This prevents that this game object is not destroyed when scene changes
		DontDestroyOnLoad(this.gameObject);
	}
		
	public void play(AudioClip clip, bool loop = false){
		Debug.Log ("MusicManagerSingleton.play()");

		if (audio.clip != clip && isMusicOn==true) {
			audio.Stop();
			audio.clip = clip;
			audio.loop = loop;
			audio.Play();
		}
	}

	// Options page is buggy. In some cases the music button is just the opposite than the real state is
	// I think that scene jumping causes this because Unity reads scene again and the default state in Toggle button in music on.
	public void toggleMusic(){
		Debug.Log ("toggleMusic: "+isMusicOn);
		isMusicOn = !isMusicOn;
		if (isMusicOn == true) {
			audio.Play();	
		}
		if (isMusicOn == false) {
			audio.Stop();
		}

	}
			

	public static void soundsOff(AudioSource music){

	}
	
}
