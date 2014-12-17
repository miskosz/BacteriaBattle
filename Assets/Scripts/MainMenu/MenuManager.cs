using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public bool musicOn;
	public AudioClip musicClip;

	public void ToggleMusic() {
				
		Debug.Log ("Music boolean: "+musicOn);
		if (musicOn == true) {
			MusicManagerSingleton.Instance.musicOff ();
			Debug.Log ("Music boolean: "+musicOn);
			musicOn =!musicOn;
		} else {
			Debug.Log ("Music boolean in ELSE: "+musicOn);
			musicOn =!musicOn;	
			MusicManagerSingleton.Instance.play(musicClip, true);
		}
		}


}


