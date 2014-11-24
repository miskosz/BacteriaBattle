using UnityEngine;
using System.Collections;

public class OptionsButton : MonoBehaviour {

	void OnMouseDown() {
		Debug.Log("PlayButton was pressed");
		
		Application.LoadLevel("Options");
		Debug.Log("Loading OptionsScene...");
		
	}
}
