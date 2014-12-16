using UnityEngine;
using System.Collections;

public class WinnerLoser : MonoBehaviour {

	Vector3 winnerPosition;
	Vector3 loserPosition;


	// Use this for initialization
	void Start () {

		//winnerPosition = new Vector3 (30, Screen.width / 2, 10);
		//loserPosition = new Vector3 (Screen.height-30, Screen.width / 2, 10);
		setHidden();
		//spin ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setVisible() {

		foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
			r.enabled = true;
	}

	public void setHidden() {
		
		foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
			r.enabled = false;
	}
	

	public void spin() {
		foreach (Transform child in transform) {

			child.transform.Rotate(new Vector3(0,0,1), 180);


		}

	}
}
