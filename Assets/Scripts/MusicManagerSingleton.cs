using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	private static MusicManagerSingleton instance = null;
	private bool isMusicOn;
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

		if (audio.clip != clip && isMusicOn==false) {
			audio.Stop();
			audio.clip = clip;
			audio.loop = loop;
			audio.Play();
		}
	}
	
	//OptionScene is using this via MenuManager. It toggles the music on/off
	public void toggleMusic(){
		isMusicOn = !isMusicOn;

		// Play music when Toggle button is ON and gives false boolean
		if (isMusicOn == false) {
			audio.Play();	
		}
		if (isMusicOn == true) {
			audio.Stop();
		}

	}
			

	public static void soundsOff(AudioSource music){

	}

	public bool musicOn(){

		return this.isMusicOn;
	}
	
}
