using UnityEngine;
using System.Collections;

public class SoundsToggle : MonoBehaviour {

	[SerializeField]
	public Sprite[] offOnSprites = new Sprite[2];
	
	SpriteRenderer spriteRenderer;
	
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = offOnSprites[MusicManagerSingleton.Instance.soundsOn() ? 1 : 0];
	}
	
	void OnMouseDown() {
		MusicManagerSingleton.Instance.toggleSound();
		spriteRenderer.sprite = offOnSprites[MusicManagerSingleton.Instance.soundsOn() ? 1 : 0];
	}
}
