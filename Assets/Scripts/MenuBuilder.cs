using UnityEngine;
using System.Collections;

public class MenuBuilder : MonoBehaviour {

	/*
	 * I've made the button as a sprite with collider for now.
	 * Maybe we'll have to use GUI when we have more on the main screen,
	 * so I'm keeping this here for now.
	 * 
	void OnGUI() {

		// temporary button size & placement
		float buttonWidth = Screen.width / 2.0f;
		float buttonHeight = 50;
		Rect buttonRect = new Rect (
			Screen.width / 2.0f - buttonWidth / 2.0f,
			Screen.height / 2.0f - buttonHeight / 2.0f,
			buttonWidth,
			buttonHeight);

		// button Start Game
		if (GUI.Button(buttonRect, "Start Game")) {
			Application.LoadLevel("GameScene");
		}
	}
	*/
}
