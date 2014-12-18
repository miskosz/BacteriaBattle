using UnityEngine;
using System.Collections;

public class ModeChooser : MonoBehaviour {

	[SerializeField]
	public Sprite[] modeSprites = new Sprite[2];

	SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = modeSprites[GlobalState.gameMode];
	}
	
	void OnMouseDown() {
		GlobalState.gameMode = (GlobalState.gameMode + 1) % 2;
		spriteRenderer.sprite = modeSprites[GlobalState.gameMode];
	}
}
