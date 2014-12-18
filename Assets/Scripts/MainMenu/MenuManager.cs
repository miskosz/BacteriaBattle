using UnityEngine;
using System.Collections;


public class MenuManager : MonoBehaviour {

	public bool musicOn;
	public AudioClip musicClip;
	public UnityEngine.UI.Toggle toggle;


	public void Start ()
	{
		toggle = GameObject.Find("MusicToggle").GetComponent<UnityEngine.UI.Toggle>();
		toggle.isOn  = MusicManagerSingleton.Instance.musicOn ();
		Debug.Log ("0. START() MenuManager musicOn in ToggleMusic()--->" +toggle.isOn);
	}

	public void ToggleMusic() {

		if (toggle.isOn != MusicManagerSingleton.Instance.musicOn()) {
			MusicManagerSingleton.Instance.toggleMusic ();
			//toggle.isOn = MusicManagerSingleton.Instance.musicOn ();
			Debug.Log ("1. AFTER TOGGLING MenuManager musicOn in ToggleMusic()--->" +toggle.isOn);
			Debug.Log ("2. AFTER TOGGLING MenuManager musicOn in ToggleMusic()--->" +MusicManagerSingleton.Instance.musicOn ());
		} 

	}

}


