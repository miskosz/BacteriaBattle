using UnityEngine;
using System.Collections;

public class WinnerLoser : MonoBehaviour {

	Vector3 winnerPosition;
	Vector3 loserPosition;

	Transform winner, loser;

	bool spinned;
	// Use this for initialization
	void Start () {

		spinned = false;

		winner = transform.FindChild("WinnerAnimation");
		loser = transform.FindChild("LoserAnimation");

		Vector3 tmp;

		tmp = winner.renderer.bounds.center;
		winnerPosition = new Vector3 (tmp.x, tmp.y, 10);

		tmp = loser.renderer.bounds.center;
		loserPosition = new Vector3 (tmp.x, tmp.y, 10);

		setHidden();
		//spin ();

	}

	public void setVisible() {

		foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
			r.enabled = true;

		foreach (Animator a in gameObject.GetComponentsInChildren<Animator>())
			a.enabled = true;


	}

	public void setHidden() {
		
		foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
			r.enabled = false;

		foreach (Animator a in gameObject.GetComponentsInChildren<Animator>())
			a.enabled = false;
	}
	

	public void spin() {


			if (!spinned) {
				winner.Rotate(new Vector3(0,0,1), 180);
				loser.Rotate(new Vector3(0,0,1), 180);
				
				winner.position = loserPosition;
				loser.position = winnerPosition;
				

				spinned = !spinned;
			}



	}
}
