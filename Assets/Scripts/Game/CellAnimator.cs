using UnityEngine;
using System.Collections;

public class CellAnimator : MonoBehaviour {

	public float blinkAverageFreq = 20f;
	public float blinkDeviation = 7f;

	Animator animator;
		
	void Start() {
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)Random.Range(0,6)*60.0f);
		animator = gameObject.GetComponent<Animator>();
		StartCoroutine(StartBlinking());
	}

	IEnumerator StartBlinking() {
		// wait in the beginning so that the animations are more spread
		yield return new WaitForSeconds(Random.Range(0,blinkAverageFreq));
		yield return StartCoroutine(Blink());
	}

	IEnumerator Blink() {
		while (true) {
			// blink only if idle
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player1_Idle") ||
			    animator.GetCurrentAnimatorStateInfo(0).IsName("Player2_Idle") ) {
				animator.SetTrigger("Blink");
			}

			float waitTime = blinkAverageFreq + Random.Range(-blinkDeviation, blinkDeviation);
			yield return new WaitForSeconds(waitTime);
		}
	}
}
