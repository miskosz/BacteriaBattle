using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {

	//There is two sounds at the moment, just for testing
	//0=startup_sound
	//1=fanfare
	//Not working
	public AudioClip[] audioClip;

	void OnMouseDown() {
		Debug.Log("StartGameButton was pressed");
		PlaySound (0);
		Application.LoadLevel("GameScene");
		Debug.Log("Audio play");

	}


	void PlaySound(int clip){
		Debug.Log("Any sounds?");
		audio.clip = audioClip [clip];
		audio.Play();
	
	}
}
