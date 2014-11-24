using UnityEngine;
using System.Collections;

public class CreditsButton : MonoBehaviour {

	void OnMouseDown() {
		Debug.Log("CreditsButton was pressed");
		
		Application.LoadLevel("Credits");
		Debug.Log("Credits");
		
	}
}
