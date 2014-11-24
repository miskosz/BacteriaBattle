using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour {

	void OnMouseDown() {
		Debug.Log("QuickButton was pressed");
		
		Application.Quit();
		Debug.Log("Quitting game...This is working only in built app, not in editor");
		
	}
}


