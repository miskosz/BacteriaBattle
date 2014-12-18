using UnityEngine;
using System.Collections;

public class ModeChooser : MonoBehaviour {

	[SerializeField]
	public Sprite[] modeSprites = new Sprite[2];

	SpriteRenderer renderer;

	void Start () {
		renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.sprite = modeSprites[GlobalState.gameMode];
	}
	
	void OnMouseDown() {
		GlobalState.gameMode = (GlobalState.gameMode + 1) % 2;
		renderer.sprite = modeSprites[GlobalState.gameMode];
	}
}
