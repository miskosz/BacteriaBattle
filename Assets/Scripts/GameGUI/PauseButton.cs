using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {

	public PauseMenu pauseMenu;

	void OnMouseDown() {
		pauseMenu.Toggle();
	}
}
