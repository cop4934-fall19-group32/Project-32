using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurveDrawer : MonoBehaviour
{

    public enum BezierCurveType { 
        LINEAR,
        QUADRATIC,
        CUBIC
    }

    public BezierCurveType Mode;

    public Vector3 A;
    public Vector3 B;
    public Vector3 C;

    public int NumPoints = 50;
    private Vector3[] Positions;
    private LineRenderer LRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Positions = new Vector3[NumPoints];
        LRenderer = GetComponent<LineRenderer>();
        LRenderer.positionCount = NumPoints;
    }

    // Update is called once per frame
    void Update()
    {
        DrawCurve();
    }

    void DrawCurve() 
    {
        if (Mode == BezierCurveType.LINEAR) {
            GenerateLinearCurve();
        }
        else if (Mode == BezierCurveType.QUADRATIC) {
            GenerateQuadraticCurve();
        }
        else if (Mode == BezierCurveType.CUBIC) {
            GenerateCubicCurve();
        }

        LRenderer.SetPositions(Positions);
    }

    private void GenerateLinearCurve() {
        for (int i = 0; i < NumPoints; i++) {
            float t = i / (float)NumPoints;
            Positions[i] = A + t * (B - A);
        }
    }

    private void GenerateQuadraticCurve() {
        for (int i = 0; i < NumPoints; i++) {
            float t = i / (float)NumPoints;
            float tRemain = 1 - t;

            var AComponent = (tRemain * tRemain) * A;
            var BComponent = 2 * tRemain * t * B;
            var CComponent = t * t * C;

            Positions[i] = AComponent + BComponent + CComponent;
        }
    }
    private void GenerateCubicCurve() {
        throw new NotImplementedException();
    }

}
