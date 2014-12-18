using UnityEngine;
using System.Collections;


public class MenuManager : MonoBehaviour {

	public bool musicOn;
	public AudioClip musicClip;
	public UnityEngine.UI.Toggle toggle,soundsToggle;


	public void Start ()
	{
		toggle = GameObject.Find("MusicToggle").GetComponent<UnityEngine.UI.Toggle>();
		soundsToggle =  GameObject.Find("SoundsToggle").GetComponent<UnityEngine.UI.Toggle>();
		toggle.isOn  = MusicManagerSingleton.Instance.musicOn ();
		soundsToggle.isOn  = MusicManagerSingleton.Instance.IsSoundsOn();
		Debug.Log ("0. START() MenuManager musicOn in ToggleMusic()--->" +toggle.isOn);
	}

	public void ToggleMusic() {

		if (toggle.isOn != MusicManagerSingleton.Instance.musicOn()) {
			MusicManagerSingleton.Instance.toggleMusic ();

		} 

	}

	public void ToggleSound() {
		
		if (soundsToggle.isOn != MusicManagerSingleton.Instance.IsSoundsOn()) {
			MusicManagerSingleton.Instance.toggleSound();

		} 
		
	}

}


