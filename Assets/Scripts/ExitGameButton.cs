using UnityEngine;
using System.Collections;

public class ExitGameButton : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	public Sprite exitDownSprite;

	void Start () {

		spriteRenderer = GetComponent<SpriteRenderer>();

	}

	void OnMouseDown() {
<<<<<<< HEAD
		Application.LoadLevel("Menu");
=======

		spriteRenderer.sprite = exitDownSprite;

		Application.LoadLevel("MainMenu");
>>>>>>> FETCH_HEAD
	}
}
