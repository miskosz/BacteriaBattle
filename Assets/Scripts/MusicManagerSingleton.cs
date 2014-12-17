using UnityEngine;
using System.Collections;

public class MusicManagerSingleton:MonoBehaviour {

	// This should be static! If it is static, this never appears to Inspector	
	public AudioClip menuMusic;
	// Because this is static, this never appears to Inspector	
	public static AudioClip gameMusic;

	public static AudioSource playingMusic=null;
	public static AudioSource music = null;

	private static MusicManagerSingleton instance = null;
		public static MusicManagerSingleton Instance {
			get { 
				if (instance == null)
				{
					//THIS IS NOT CORRECT
					instance = new MusicManagerSingleton();
				}
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
			//This prevent that this game object is not destroyed when scene changes
			DontDestroyOnLoad(this.gameObject);
			}
		
	public static void play(AudioSource music){
		Debug.Log ("MusicManagerSingleton.play()");
		music.Play();

		}


	/**
	 * I couldn't get this working @SN
	 * I got the runtime exception:
	 * NullReferenceException
	 * MusicManagerSingleton.playMenuMusic () (at Assets/Scripts/MusicManagerSingleton.cs:55)
	 * PlayMusic.Awake () (at Assets/Scripts/MainMenu/PlayMusic.cs:14)
	 * 
	 */
	public void playMenuMusic(){
		Debug.Log ("MusicManagerSingleton.playMenuMusic()--->" +menuMusic);
		MusicManagerSingleton.Instance.audio.clip = menuMusic;
		MusicManagerSingleton.Instance.audio.Play();
		//audio.clip = menuMusic;
		//audio.Play ();

	}
		
	public static void musicOff(AudioSource music){
		

	}

	public static void soundsOff(AudioSource music){
		

	}

	public static void switchMusicToGameMusic(){
		Debug.Log ("MusicManagerSingleton.play()");
		MusicManagerSingleton.Instance.audio.Stop();

		//Shoud get AudioClips working somehow or destroy the old AudioSource first
		//MusicManagerSingleton.Instance.audio.Play();
		}

	public static AudioSource getAudioSource(){

		return MusicManagerSingleton.Instance.gameObject.audio;  

	}

}
