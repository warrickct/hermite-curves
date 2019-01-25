using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Procedural
{
	public class Delaunay : MonoBehaviour
	{

		public MeshFilter mFilter;
		public MeshRenderer mRenderer;

		public List<Vertex> convexHullpoints;

		private void Start()
		{
			// var vertPoints = GameObject.FindGameObjectsWithTag("vert");
			// List<Vertex> convexHullpoints = new List<Vertex>();
			// foreach (var vertPoint in vertPoints)
			// {
			// 	convexHullpoints.Add(new Vertex(vertPoint.transform.position));
			// }

			var vertPointsGos = GameObject.FindGameObjectsWithTag("vert");
			List<Vertex> vertices = new List<Vertex>();
			foreach (var vertGo in vertPointsGos)
			{
				vertices.Add(new Vertex(vertGo.transform.position));
			}

			convexHullpoints = JarvisMarch.GetConvexHull(vertices);
			Debug.Log(convexHullpoints.Count);

			var triangles = TriangulateConvexPolygon(convexHullpoints);
			Debug.Log(triangles.Count);
			
			Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
			if (!mesh)
			{
				mesh = new Mesh();
			}
			// mesh.vertices = convexHullpoints;
			// mesh.triangles = tris;
			// mesh.RecalculateNormals();
			// mesh.uv = uvs;
			// mesh.colors = colors;
			// GetComponent<MeshFilter>().sharedMesh = mesh;
		}

		private void FixedUpdate() {
			var vertPointsGos = GameObject.FindGameObjectsWithTag("vert");
			List<Vertex> vertices = new List<Vertex>();
			foreach (var vertGo in vertPointsGos)
			{
				vertices.Add(new Vertex(vertGo.transform.position));
			}

			convexHullpoints = JarvisMarch.GetConvexHull(vertices);
			Debug.Log(convexHullpoints.Count);

			var triangles = TriangulateConvexPolygon(convexHullpoints);
			Debug.Log(triangles.Count);
			
			Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
			if (!mesh)
			{
				mesh = new Mesh();
			}
		}

		private void OnDrawGizmos() {
			for (int i = 0; i < convexHullpoints.Count; i++)
			{
				var v1 = convexHullpoints[i];
				if (convexHullpoints[i + 1] != null)
				{
					var v2 = convexHullpoints[i + 1];
					Gizmos.DrawLine(v1.position, v2.position);

				}
			}
		}

		public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
		{
			List<Triangle> triangles = new List<Triangle>();

			for (int i = 2; i < convexHullpoints.Count; i++)
			{
				Vertex a = convexHullpoints[0];
				Vertex b = convexHullpoints[i - 1];
				Vertex c = convexHullpoints[i];

				triangles.Add(new Triangle(a, b, c));
			}
			return triangles;
		}
	}
}
