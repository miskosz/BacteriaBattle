using UnityEngine;
using System.Collections;

public class PauseMenuResume : MonoBehaviour {

	public PauseMenu pauseMenu;

	void OnMouseDown() {
		pauseMenu.Toggle();
	}
}
