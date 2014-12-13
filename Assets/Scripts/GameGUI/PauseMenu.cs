using UnityEngine;
using System.Collections;


public class PauseMenu : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	public BoardBuilder boardBuilder;

	// Use this for initialization
	void Start () {
		Toggle();
	}

	public void Toggle() {
		renderer.enabled = !renderer.enabled;
		foreach(Renderer r in gameObject.GetComponentsInChildren<Renderer>())
			r.enabled=renderer.enabled;
		foreach(BoxCollider2D b in gameObject.GetComponentsInChildren<BoxCollider2D>())
			b.enabled=renderer.enabled;

		if (!renderer.enabled) {
			boardBuilder.enableInput();
		}
		else {
			boardBuilder.disableInput();
		}
	}
}
