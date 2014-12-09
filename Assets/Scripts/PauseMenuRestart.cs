using UnityEngine;
using System.Collections;

public class PauseMenuRestart : MonoBehaviour {

	void OnMouseDown() {
		Application.LoadLevel("GameScene");
	}
}
