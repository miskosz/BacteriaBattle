using UnityEngine;
using System.Collections;


public class PauseMenu : MonoBehaviour {

	public PauseButton pauseButton;

	private SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();

		//hide pause menu
		spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		//hide or show pause menu if button pushed
		if (pauseButton.getMenuVisible()) {
			spriteRenderer.enabled = true;
		}
		else 
			spriteRenderer.enabled = false;

	}
}
