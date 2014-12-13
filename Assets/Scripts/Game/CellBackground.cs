using UnityEngine;
using System.Collections;

public class CellBackground : MonoBehaviour {

//	void Start() {
//		transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)Random.Range(0,6)*60.0f);
//	}

	public void Init(int i, int j) {
		// this generates a pseudorandom number from the position (i,j)
		// by hashing "salt i j"
		int deterministicRandom = Animator.StringToHash("salt " + i + " " + j);
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, (deterministicRandom % 6)*60.0f);
	}	
}
