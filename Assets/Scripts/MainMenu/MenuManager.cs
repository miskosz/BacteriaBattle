using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public bool musicOn = true;


	public void ToggleMusic() {
				musicOn = !musicOn;
		Debug.Log ("Music boolean: "+musicOn);
				if (musicOn == false) {
						//MusicManagerSingleton.musicOff ();
				} else {
						//MusicManagerSingleton.musicOn ();
				}
		}


}


