using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Utility class to draw line between two points
 */
public class BezierCurveGenerator : MonoBehaviour
{

    public enum CurveMode { 
        LINEAR,
        QUADRATIC,
        CUBIC
    }

    [Header("RenderingSettings")]
    public Vector3 Source;
    public Vector3 Target;
    public Vector3 ControlPoint1;
    public Vector3 ControlPoint2;
    public int NumPoints = 50;

    private Vector3[] Positions;
    // Start is called before the first frame update
    void Start()
    {
        Positions = new Vector3[NumPoints];
    }

    public Vector3[] GenerateCurve(CurveMode mode) {
        switch (mode) {
            case CurveMode.LINEAR:
                return GenerateLinearCurve();
            case CurveMode.QUADRATIC:
                return GenerateQuadraticCurve();
            case CurveMode.CUBIC:
                return GenerateCubicCurve();
        }

        return null;
    }

    private Vector3[] GenerateLinearCurve() {
        return new Vector3[] { Source, Target };
    }

    private Vector3[] GenerateQuadraticCurve() {
        Positions[0] = Source;
        for (int i = 1; i < NumPoints - 1; i++) {
            float t = i / (float)NumPoints;
            float tRemain = 1 - t;

            var AComponent = (tRemain * tRemain) * Source;
            var BComponent = 2 * tRemain * t * ControlPoint1;
            var CComponent = t * t * Target;

            Positions[i] = AComponent + BComponent + CComponent;
        }
        Positions[49] = Target;
        return Positions;
    }
    private Vector3[] GenerateCubicCurve() {
        Positions[0] = Source;
        for (int i = 1; i < NumPoints - 1; i++) {
            float t = i / (float)NumPoints;
            float tRemain = 1 - t;

            var AComponent = (tRemain * tRemain * tRemain) * Source;
            var BComponent = 3 * (tRemain * tRemain) * t * ControlPoint1;
            var CComponent = 3 * tRemain * (t * t) * ControlPoint2;
            var DComponent = t * t * t * Target;

            Positions[i] = AComponent + BComponent + CComponent + DComponent;
        }
        Positions[49] = Target;
        return Positions;
    }
}
