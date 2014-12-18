using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	private static MusicManagerSingleton instance = null;
	private bool isMusicOff,isSoundsOff;

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
		
	public void playMusic(AudioClip clip){
		Debug.Log ("MusicManagerSingleton.play()");

		if (audio.clip != clip && isMusicOff==false) {
			audio.Stop();
			audio.clip = clip;
			audio.loop = true;
			audio.Play();
		}
	}
	
	//OptionScene is using this via MenuManager. It toggles the music on/off
	public void toggleMusic(){
		isMusicOff = !isMusicOff;

		// Play music when Toggle button is ON and gives false boolean
		if (isMusicOff == false) {
			audio.Play();	
		}
		if (isMusicOff == true) {
			audio.Stop();
		}

	}

	//OptionScene is using this via MenuManager. It toggles the sounds on/off
	public void toggleSound(){
		isSoundsOff = !isSoundsOff;
	}

	public bool musicOn(){
		return !this.isMusicOff;
	}

	// When true, mute is on and there is no sounds
	public bool soundsOn() {
		return !this.isSoundsOff;
	}
}
