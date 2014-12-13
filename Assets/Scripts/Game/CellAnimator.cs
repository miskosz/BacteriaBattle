using UnityEngine;
using System.Collections;

public class CellAnimator : MonoBehaviour {

	void Start() {
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)Random.Range(0,6)*60.0f);
	}
}
