using UnityEngine;
using System.Collections;

public class NewGameButton : MonoBehaviour {

	void OnMouseDown() {

		Application.LoadLevel("GameScene");
	}
}
