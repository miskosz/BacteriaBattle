using UnityEngine;
using System.Collections;

public class BoardChooser : MonoBehaviour {

	[SerializeField]
	public Sprite[] boardSprites = new Sprite[3];
	
	SpriteRenderer spriteRenderer;
	
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = boardSprites[GlobalState.board];
	}
	
	void OnMouseDown() {
		GlobalState.board = (GlobalState.board + 1) % 3;
		spriteRenderer.sprite = boardSprites[GlobalState.board];
	}
}
