using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPointerController : MonoBehaviour
{
	public float SlideTime;
	public float SlideDistance;
	public Vector3 StartScale = new Vector3(1, 1, 1);
	public Vector3 EndScale = new Vector3(1, 1, 1);
	public float LoopDelay;

	public AnimationCurve ScaleCurve;
	public AnimationCurve PositionCurve;
	public AnimationCurve AlphaCurve;

	public float SlideVelocity {
		get { return SlideDistance / SlideTime; }
	}

	private Vector3 TargetPosition;
	private Vector3 TranslationVector;

	protected void OnEnable() {
		var currNode = GameObject.Find(FindObjectOfType<PlayerState>().LastAttemptedLevel).GetComponent<MapNode>();
		var nextNode = currNode.Next;
		if (nextNode == null || nextNode.ScoreRequired > FindObjectOfType<PlayerState>().GetScore()) {
			return;
		}
		else {
			TranslationVector = (nextNode.transform.position - currNode.transform.position).normalized;
			TargetPosition = transform.localPosition + (SlideDistance * TranslationVector);
			transform.rotation = 
				Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * TranslationVector);
			StartCoroutine(Animate());
		}
	}

	IEnumerator Animate() {
		while (true) {
			transform.localPosition = new Vector3();
			transform.localScale = StartScale;
			var startPosition = transform.localPosition;
			float time = 0;
			var spriteRenderer = GetComponent<SpriteRenderer>();
			var startColor = spriteRenderer.color;
			var endColor = startColor;
			endColor.a = 0;

			while (time < SlideTime) {
				var positionPercentage = PositionCurve.Evaluate(time / SlideTime);
				transform.localPosition =
					Vector3.Lerp(startPosition, TargetPosition, positionPercentage);

				var sizePercentage = ScaleCurve.Evaluate(time / SlideTime);
				transform.localScale =
					Vector3.Lerp(StartScale, EndScale, sizePercentage);

				var alphaPercentage = AlphaCurve.Evaluate(time / SlideTime);
				spriteRenderer.color =
					Color.Lerp(startColor, endColor, alphaPercentage);

				time += Time.deltaTime;

				yield return null;
			}

			spriteRenderer.color = startColor;

			yield return new WaitForSeconds(LoopDelay);
		}
	}
}
