using UnityEngine;
using System.Collections;

public class PlayButton : MonoBehaviour {

	void OnMouseDown() {
		Debug.Log("PlayButton was pressed");

		Application.LoadLevel("GameScene");
		Debug.Log("GameScene");
		
	}
}




