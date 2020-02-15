using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierCurveGenerator))]
[RequireComponent(typeof(LineRenderer))]
/**
 * Class to be used by JumpDragNDropBehavior to draw jump lines.
 */
public class JumpLineDrawer : MonoBehaviour
{
    public RectTransform instructionTransform { get; set; }

    public RectTransform anchorTransform { get; set; }

    private BezierCurveGenerator curveGenerator;
    private LineRenderer lineRenderer;
    private static Vector3 controlpointOffset = new Vector3(10, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        curveGenerator = GetComponent<BezierCurveGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (instructionTransform == null || anchorTransform == null) {
            return;
        }

        UpdateJumpLineControlPoints();
    }

    private void UpdateJumpLineControlPoints() {
        //static const Vector3 offset = new
        //Set jump line source
        Vector3[] target = new Vector3[4];
        instructionTransform.GetWorldCorners(target);
        curveGenerator.Source = target[2];
        curveGenerator.ControlPoint1 = curveGenerator.Source + controlpointOffset;

        anchorTransform.GetWorldCorners(target);
        curveGenerator.Target = target[3];
        curveGenerator.ControlPoint2 = curveGenerator.Target + controlpointOffset;
    }

    public IEnumerator DrawJumpLine() {
        //Coroutine runs at 30 frames per second
        while (true) {
            if (anchorTransform == null) { 
                yield return new WaitForSeconds(.033f);
            }

            var curve = curveGenerator.GenerateCurve(BezierCurveGenerator.CurveMode.CUBIC);
            lineRenderer.positionCount = curve.Length;
            lineRenderer.SetPositions(curve);
            yield return new WaitForSeconds(.033f);
        }
    }
}
