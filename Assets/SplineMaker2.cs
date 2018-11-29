using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SplineMaker2 : MonoBehaviour
{
    //Use the transforms of GameObjects in 3d space as your points or define array with desired points
    public List<Transform> controlPoints = new List<Transform>();

    //Store points on the Catmull curve so we can visualize them
    List<Vector3> newPoints = new List<Vector3>();

    //How many points you want on the curve
    float amountOfPoints = 10.0f;

    //set from 0-1
    public float alpha = 0.5f;

    /////////////////////////////

    public LineRenderer line;

    public int numberOfPoints;

    private void Start()
    {
        line.positionCount = controlPoints.Count * numberOfPoints - (numberOfPoints);
    }

    void Update()
    {

        DynamicSpline(controlPoints);

        GizmoSpline(controlPoints);
    }

    void GizmoSpline(List<Transform> controlPoints)
    {
        debugPositions.Clear();

        // position
        Vector3 p0, p1, m0, m1;

        for (int j = 0; j < controlPoints.Count - 1; j++)
        {
            // check control points
            if (controlPoints[j] == null || controlPoints[j + 1] == null ||
                (j > 0 && controlPoints[j - 1] == null) ||
                (j < controlPoints.Count - 2 && controlPoints[j + 2] == null))
            {
                return;
            }
            // determine control points of segment
            p0 = controlPoints[j].transform.position;
            p1 = controlPoints[j + 1].transform.position;

            if (j > 0)
            {
                m0 = 0.5f * (controlPoints[j + 1].transform.position - controlPoints[j - 1].transform.position);
            }
            else
            {
                m0 = controlPoints[j + 1].transform.position - controlPoints[j].transform.position;
            }
            if (j < controlPoints.Count - 2)
            {
                m1 = 0.5f * (controlPoints[j + 2].transform.position - controlPoints[j].transform.position);
            }
            else
            {
                m1 = controlPoints[j + 1].transform.position - controlPoints[j].transform.position;
            }

            // set points of Hermite curve
            Vector3 position;
            float t;
            float pointStep = 1.0f / numberOfPoints;

            if (j == controlPoints.Count - 2)
            {
                pointStep = 1.0f / (numberOfPoints - 1.0f);
                // last point of last segment should reach p1
            }
            for (int i = 0; i < numberOfPoints; i++)
            {
                t = i * pointStep;
                position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                    + (t * t * t - 2.0f * t * t + t) * m0
                    + (-2.0f * t * t * t + 3.0f * t * t) * p1
                    + (t * t * t - t * t) * m1;

                // line.SetPosition(i + j * numberOfPoints,
                // position);

                // test
                Vector3 testPos = position + Vector3.Scale(position, m0);

                // backbone
                debugPositions.Add(position);

                //x & y offsets
                var right = position + Vector3.right;
                var left = position - Vector3.right;
                var up = position + Vector3.up;
                var down = position - Vector3.up;

                debugPositions.Add(right);
                debugPositions.Add(position - Vector3.right);
                debugPositions.Add(position + Vector3.up);
                debugPositions.Add(position - Vector3.up);

                // diagonals
                debugPositions.Add(position + Vector3.Normalize(Vector3.right + Vector3.up));
                debugPositions.Add(position - Vector3.Normalize(Vector3.right + Vector3.up));
                debugPositions.Add(position + Vector3.Normalize(Vector3.right - Vector3.up));
                debugPositions.Add(position - Vector3.Normalize(Vector3.right - Vector3.up));
            }
        }
    }


    void DynamicSpline(List<Transform> controlPoints)
    {
        Vector3 p0, p1, m0, m1;

        for (int j = 0; j < controlPoints.Count - 1; j++)
        {
            // check control points
            if (controlPoints[j] == null || controlPoints[j + 1] == null ||
                (j > 0 && controlPoints[j - 1] == null) ||
                (j < controlPoints.Count - 2 && controlPoints[j + 2] == null))
            {
                return;
            }
            // determine control points of segment
            p0 = controlPoints[j].transform.position;
            p1 = controlPoints[j + 1].transform.position;

            if (j > 0)
            {
                m0 = 0.5f * (controlPoints[j + 1].transform.position - controlPoints[j - 1].transform.position);
            }
            else
            {
                m0 = controlPoints[j + 1].transform.position - controlPoints[j].transform.position;
            }
            if (j < controlPoints.Count - 2)
            {
                m1 = 0.5f * (controlPoints[j + 2].transform.position - controlPoints[j].transform.position);
            }
            else
            {
                m1 = controlPoints[j + 1].transform.position - controlPoints[j].transform.position;
            }

            // set points of Hermite curve
            Vector3 position;
            float t;
            float pointStep = 1.0f / numberOfPoints;

            if (j == controlPoints.Count - 2)
            {
                pointStep = 1.0f / (numberOfPoints - 1.0f);
                // last point of last segment should reach p1
            }
            for (int i = 0; i < numberOfPoints; i++)
            {
                t = i * pointStep;
                position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                    + (t * t * t - 2.0f * t * t + t) * m0
                    + (-2.0f * t * t * t + 3.0f * t * t) * p1
                    + (t * t * t - t * t) * m1;
                line.SetPosition(i + j * numberOfPoints,
                    position);
            }
        }
    }

    private List<Vector3> debugPositions = new List<Vector3>();

    private void OnDrawGizmos()
    {
        foreach (var position in debugPositions)
        {
            Gizmos.DrawSphere(position, 0.1f);
        }
    }
}
