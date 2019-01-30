using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Procedural
{
	public class Delaunay2 : MonoBehaviour
	{

		public MeshFilter mFilter;
		public MeshRenderer mRenderer;
		public Material material;
		public GameObject[] sites;
		public List<Vertex> vertCloud = new List<Vertex>();
		public List<Vertex> hullVerts = new List<Vertex>();
		public List<Triangle> hullTriangles = new List<Triangle>();
		public bool drawVertices;
		public bool drawSites;
		public bool drawHullTris;
		public bool drawHullVerts;

		void Start() {
			CloudEdge();
		}

		private List<Vertex> UpdateSiteVertices(){
			vertCloud.Clear();
			sites = GameObject.FindGameObjectsWithTag("vert");
			foreach (var site in sites)
			{
				vertCloud.Add(new Vertex(site.transform.position));
			}
			return vertCloud;
		}

		void FixedUpdate()
		{
			hullTriangles = TriangulatePoints.TriangulateConvexPolygon(vertCloud);
			Debug.Log(hullTriangles.Count);
			hullVerts = JarvisMarch.GetConvexHull(vertCloud);
			Debug.Log(hullVerts.Count);
		}

		private void OnDrawGizmos()
		{
			if (drawSites)
			{
				// sites 
				Gizmos.color = Color.red;
				foreach (var site in sites)
				{
					Gizmos.DrawSphere(site.transform.position, 0.1f);
				}
			}

			if (drawVertices)
			{
				// polygonal hull
				Gizmos.color = Color.cyan;
				foreach (var vert in vertCloud)
				{
					Gizmos.DrawSphere(vert.position, 0.2f);
				}
			}

			if (drawHullVerts)
			{
				Gizmos.color = Color.black;
				foreach (var vert in hullVerts)
				{
					Gizmos.DrawSphere(vert.position, 0.2f);
				}
			}

			if (drawHullTris)
			{
				Gizmos.color = Color.magenta;
				foreach (var tri in hullTriangles)
				{
					// Gizmos.DrawSphere(point.position, 0.2f);
					Gizmos.DrawSphere(tri.v1.position, 0.2f);
					Gizmos.DrawSphere(tri.v2.position, 0.2f);
					Gizmos.DrawSphere(tri.v3.position, 0.2f);
				}
			}
		}

		private void FormMesh(List<Vector3> verts, List<int> tris)
		{
			Mesh mesh = new Mesh();
			mesh.name = "delaunayMesh";
			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.RecalculateTangents();
			mesh.RecalculateNormals();

			mFilter.mesh = mesh;
			mRenderer.material = material;
		}

		private List<Vertex> CloudEdge() {
			UpdateSiteVertices();
			var vrts = vertCloud.Select(v => v.position).ToArray();

			// finding start P
			int minIndx = 0;
			Vector3 minV3 = vrts[0];
			for (int i = 0; i < vrts.Length; i++)
			{
				Vector3 curV3 = vrts[i];
				if (minV3 != Vector3.Min(minV3, curV3))
				{
					minV3 = curV3;
					minIndx = i;
				}
			}
			Debug.Log(minIndx);
			Debug.Log(minV3);
			
			// convex hull (C) = points[h];

			
			return null;
		}
	}
}