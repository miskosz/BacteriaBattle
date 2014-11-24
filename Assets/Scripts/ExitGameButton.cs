using UnityEngine;
using System.Collections;

public class ExitGameButton : MonoBehaviour {

	void OnMouseDown() {
		Application.LoadLevel("Menu");
	}
}
