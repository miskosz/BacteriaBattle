using UnityEngine;
using System.Collections;

public class PauseMenuQuit : MonoBehaviour {

	void OnMouseDown() {
		Application.LoadLevel("Menu");
	}
}
