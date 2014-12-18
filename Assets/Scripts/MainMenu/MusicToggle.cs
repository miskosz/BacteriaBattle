using UnityEngine;
using System.Collections;

public class MusicToggle : MonoBehaviour {

	[SerializeField]
	public Sprite[] offOnSprites = new Sprite[2];
	
	SpriteRenderer spriteRenderer;
	
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = offOnSprites[MusicManagerSingleton.Instance.musicOn() ? 1 : 0];
	}
	
	void OnMouseDown() {
		MusicManagerSingleton.Instance.toggleMusic();
		spriteRenderer.sprite = offOnSprites[MusicManagerSingleton.Instance.musicOn() ? 1 : 0];
	}
}
