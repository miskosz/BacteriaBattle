using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {

	private bool menuVisible = false;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnMouseDown() {
		//controls whether the pause menu is visible or not
		toggleMenuVisible();
		
	}

	
	public bool getMenuVisible() {
		return menuVisible;
	}
	
	public void toggleMenuVisible() {

		menuVisible = ! menuVisible;
		
	}

}
