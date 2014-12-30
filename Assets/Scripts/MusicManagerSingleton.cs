using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	private static MusicManagerSingleton instance = null;
	private bool isMusicOn = false /* TODO */, isSoundsOn = true;

	AudioClip clip;
	public static MusicManagerSingleton Instance {
		get { 
			return instance;
		}
	}
		
	void Awake() {
		if (instance != null && instance != this) {
			Debug.Log ("MusicManagerSingleton already created");
			Destroy(this.gameObject);
			return;
		} else {
			Debug.Log ("Creating MusicManagerSingleton");
			instance = this;
		}
		//This prevents that this game object is not destroyed when scene changes
		DontDestroyOnLoad(this.gameObject);
	}

	public void playMusic(AudioClip clip){
		if (audio.clip != clip) {
			audio.clip = clip;
			if (isMusicOn) {
				audio.Stop();
				audio.loop = true;
				audio.Play();
			}
		}
	}

	public void playSound(AudioClip clip, AudioSource audioSrc){
		if (isSoundsOn) {
			audioSrc.clip = clip;
			audioSrc.Play();
		}
	}

	//OptionScene is using this via MenuManager. It toggles the music on/off
	public void toggleMusic(){
		isMusicOn = !isMusicOn;

		if (isMusicOn)
			audio.Play();	
		else
			audio.Stop();
	}

	//OptionScene is using this via MenuManager. It toggles the sounds on/off
	public void toggleSound(){
		isSoundsOn = !isSoundsOn;
	}

	public bool musicOn(){
		return isMusicOn;
	}

	// When true, mute is on and there is no sounds
	public bool soundsOn() {
		return isSoundsOn;
	}
}
