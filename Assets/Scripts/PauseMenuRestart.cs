using UnityEngine;
using System.Collections;

public class PauseMenuRestart : MonoBehaviour {

	public PauseButton pauseButton;
	
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		
		//hide pause menu
		spriteRenderer.enabled = false;
	}
	
	
	void Update () {
		//hide or show pause menu if button pushed
		if (pauseButton.getMenuVisible ()) {
			spriteRenderer.enabled = true;	//enable sprite and collider
			boxCollider.enabled = true;
		} else {
			spriteRenderer.enabled = false;	//disable
			boxCollider.enabled = false;
		}
	}

	void OnMouseDown() {
		
		Application.LoadLevel("GameScene");
		
	}
}
