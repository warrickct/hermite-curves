using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Material))]
public class ProceduralMesh : MonoBehaviour
{

	public int CrossSegments = 8;
	public int lastCrossSegments;
	public float Length = 1;
	public float radius = 1;
	public Vector3[] gizmoMeshPoints;
	public Vector3[] gizmoControlPoints;
	public Material material;


	void Start()
	{
		GetComponent<MeshRenderer>().material = material;
		Vector3[] controlPoints = CreateControlPoints();
		Vector3[] crossPoints = CreateCrossPoints(controlPoints);
		CreateMeshVertices(controlPoints, crossPoints);
	}

	private void CreateMeshVertices(Vector3[] controlPoints, Vector3[] crossPoints)
	{
		// Quaternion rotation = transform.rotation;
		// for (int i = 0; i < controlPoints.Length; i++)
		// {
		// 	for (int j = 0; j < crossPoints.Length; j++)
		// 	{
		// 		int vertexIndex = (i * crossPoints.Length) + j;
		// 		meshVertices[vertexIndex] = controlPoints[i] + rotation * crossPoints[j] * radius;
		// 	}
		// }

		Vector3[] meshVertices = new Vector3[controlPoints.Length * crossPoints.Length];
		Vector2[] uvs = new Vector2[controlPoints.Length * CrossSegments];
		Color[] colors = new Color[controlPoints.Length * CrossSegments];
		int[] tris = new int[controlPoints.Length * CrossSegments * 6];
		int[] lastVertices = new int[CrossSegments];
		int[] theseVertices = new int[CrossSegments];
		Quaternion rotation = Quaternion.identity;
		for (int p = 0; p < controlPoints.Length; p++)
		{
			rotation = transform.rotation;
			// if (p < controlPoints.Length - 1)
			// 	rotation = Quaternion.FromToRotation(Vector3.forward, controlPoints[p + 1]- controlPoints[p]);

			for (int c = 0; c < CrossSegments; c++)
			{
				int vertexIndex = p * CrossSegments + c;
				meshVertices[vertexIndex] = controlPoints[p] + rotation * crossPoints[c] * radius;
				uvs[vertexIndex] = new Vector2((0.0f + c) / CrossSegments, (0.0f + p) / controlPoints.Length);
				colors[vertexIndex] = Color.yellow;
				// colors[vertexIndex] = tubeVertices[p].color;

				lastVertices[c] = theseVertices[c];
				theseVertices[c] = p * CrossSegments + c;
			}
			//make triangles
			if (p > 0)
			{
				for (int c = 0; c < CrossSegments; c++)
				{
					int start = (p * CrossSegments + c) * 6;
					tris[start] = lastVertices[c];
					tris[start + 1] = lastVertices[(c + 1) % CrossSegments];
					tris[start + 2] = theseVertices[c];
					tris[start + 3] = tris[start + 2];
					tris[start + 4] = tris[start + 1];
					tris[start + 5] = theseVertices[(c + 1) % CrossSegments];
				}
			}

			// code for triangle caps
			if (p == 0 || p == controlPoints.Length - 1)
			{
				for (int c = 0; c < CrossSegments; c++)
				{
					continue;
					// Debug.Log("asdkjfh");
					
				}
			}
		}
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		if (!mesh)
		{
			mesh = new Mesh();
		}
		mesh.vertices = meshVertices;
		mesh.triangles = tris;
		mesh.RecalculateNormals();
		mesh.uv = uvs;
		mesh.colors = colors;
		GetComponent<MeshFilter>().sharedMesh = mesh;
		gizmoMeshPoints = meshVertices;
	}

	private Vector3[] CreateCrossPoints(Vector3[] crossSectionOrigins)
	{
		Vector3[] crossPoints = new Vector3[CrossSegments];
		if (CrossSegments != lastCrossSegments)
		{
			float theta = 2.0f * Mathf.PI / CrossSegments;
			for (int c = 0; c < CrossSegments; c++)
			{
				crossPoints[c] = new Vector3(Mathf.Cos(theta * c) / 1.0f, Mathf.Sin(theta * c), 0);
			}
			lastCrossSegments = CrossSegments;
		}
		return crossPoints;
	}

	private Vector3[] CreateControlPoints()
	{
		Vector3[] controlPoints = new Vector3[2];
		controlPoints[0] = transform.position - (Vector3.up * (Length / 2));
		controlPoints[1] = transform.position + (Vector3.up * (Length / 2));
		gizmoControlPoints = controlPoints;
		return controlPoints;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (gizmoMeshPoints != null)
		{
			foreach (var point in gizmoMeshPoints)
			{
				Gizmos.DrawSphere(point, 0.1f);
			}
		}

		Gizmos.color = Color.green;
		if (gizmoControlPoints != null)
		{
			foreach (var point in gizmoControlPoints)
			{
				Gizmos.DrawSphere(point, 0.1f);
			}
		}
	}
}