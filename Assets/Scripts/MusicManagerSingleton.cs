using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	private static MusicManagerSingleton instance = null;
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
		if (audio.clip != clip) {
			audio.Stop();
			audio.clip = clip;
			audio.loop = loop;
			audio.Play();
		}
	}
			
	public static void musicOff(AudioSource music){

	}

	public static void soundsOff(AudioSource music){

	}
	
}
