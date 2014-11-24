using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
	void OnMouseDown() {
		Debug.Log("BackButton was pressed");
		
		Application.LoadLevel("Menu");
		Debug.Log("Menu");
		
	}
}
