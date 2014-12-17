using UnityEngine;
using System.Collections;

public class PlayButton : MonoBehaviour {

	void OnMouseDown() {
		Debug.Log("PlayButton was pressed");

		MusicManagerSingleton.switchMusicToGameMusic();
		Application.LoadLevel("GameScene");
		Debug.Log("GameScene");
		
	}

}




