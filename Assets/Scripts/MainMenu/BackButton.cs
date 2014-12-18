using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {

	public float xOffset = 0.2f, yOffset = 0.4f;

	void Start() {
		transform.position = new Vector3 (
			- Camera.main.orthographicSize / Screen.height * Screen.width + xOffset,
			Camera.main.orthographicSize - yOffset,
			0);
	}

	void OnMouseDown() {
		Debug.Log("BackButton was pressed");
		
		Application.LoadLevel("MainMenu");
		Debug.Log("MainMenu");
		
	}
}
