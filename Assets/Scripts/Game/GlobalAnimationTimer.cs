using UnityEngine;
using System.Collections;

static class GlobalAnimationTimer {

	static float animationEnd;

	static GlobalAnimationTimer() {
		animationEnd = Time.time;
	}

	public static void AnimationTriggered(AnimationClip clip) {
		animationEnd = Mathf.Max(animationEnd, Time.time + clip.length);
	}

	public static IEnumerator WaitForAnimationEnd() {
		while (Time.time < animationEnd) {
			yield return new WaitForSeconds(animationEnd - Time.time);
		}
	}

}
