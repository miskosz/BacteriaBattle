using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {

	void OnMouseDown() {
		Application.LoadLevel("GameScene");
	}
}
