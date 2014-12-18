using UnityEngine;
using System.Collections;

public class BoardChooser : MonoBehaviour {

	[SerializeField]
	public Sprite[] boardSprites = new Sprite[3];
	
	SpriteRenderer renderer;
	
	void Start () {
		renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.sprite = boardSprites[GlobalState.board];
	}
	
	void OnMouseDown() {
		GlobalState.board = (GlobalState.board + 1) % 3;
		renderer.sprite = boardSprites[GlobalState.board];
	}
}
