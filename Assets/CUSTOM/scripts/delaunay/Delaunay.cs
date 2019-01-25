﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Procedural
{
	public partial class Delaunay : MonoBehaviour
	{

		public MeshFilter mFilter;
		public MeshRenderer mRenderer;

		public List<Vertex> convexHullPoints;
		public List<Vertex> vertices;

		private void Start()
		{
			var vertPointsGos = GameObject.FindGameObjectsWithTag("vert");
			vertices = new List<Vertex>();
			foreach (var vertGo in vertPointsGos)
			{
				vertices.Add(new Vertex(vertGo.transform.position));
			}

			convexHullPoints = JarvisMarch.GetConvexHull(vertices);
			// Debug.Log(convexHullPoints.Count);

			var triangles = TriangulatePoints.TriangulateConvexPolygon(convexHullPoints);
			// Debug.Log(triangles.Count);
		}

		public Mesh debugMesh;
		private void FixedUpdate()
		{
			// var vertPointsGos = GameObject.FindGameObjectsWithTag("vert");
			// vertices = new List<Vertex>();
			// foreach (var vertGo in vertPointsGos)
			// {
			// 	vertices.Add(new Vertex(vertGo.transform.position));
			// }

			// convexHullPoints = JarvisMarch.GetConvexHull(vertices);
			// // Debug.Log(convexHullPoints.Count);

			// List<Triangle> triangles = TriangulatePoints.TriangulateConvexPolygon(convexHullPoints);

			var sites = GameObject.FindGameObjectsWithTag("vert").Select(go => go.transform.position).ToList();
			var triangles = TriangulateByFlippingEdges(sites);

			if (mFilter.mesh == null)
			{
				mFilter.mesh = new Mesh();
			}
			// mFilter.mesh.vertices = sites.ToArray();
			// mFilter.mesh.triangles = triangles.ToArray();

			// only use relevant vertices and map em
			var verts = new Vector3[triangles.Count * 3];
			var triangleInts = new int[triangles.Count];
			for (int i = 0; i < triangles.Count; i++)
			{
				
			}
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < convexHullPoints.Count - 1; i++)
			{
				var v1 = convexHullPoints[i];
				var v2 = convexHullPoints[i + 1];
				Gizmos.DrawLine(v1.position, v2.position);
			}
			Gizmos.DrawLine(convexHullPoints[convexHullPoints.Count - 1].position, convexHullPoints[0].position);

			Gizmos.DrawMesh(debugMesh);
		}

		private static void FlipEdge(HalfEdge one)
		{
			//The data we need
			//This edge's triangle
			HalfEdge two = one.nextEdge;
			HalfEdge three = one.prevEdge;
			//The opposite edge's triangle
			HalfEdge four = one.oppositeEdge;
			HalfEdge five = one.oppositeEdge.nextEdge;
			HalfEdge six = one.oppositeEdge.prevEdge;
			//The vertices
			Vertex a = one.v;
			Vertex b = one.nextEdge.v;
			Vertex c = one.prevEdge.v;
			Vertex d = one.oppositeEdge.nextEdge.v;
			//Flip

			//Change vertex
			a.halfEdge = one.nextEdge;
			c.halfEdge = one.oppositeEdge.nextEdge;

			//Change half-edge
			//Half-edge - half-edge connections
			one.nextEdge = three;
			one.prevEdge = five;

			two.nextEdge = four;
			two.prevEdge = six;

			three.nextEdge = five;
			three.prevEdge = one;

			four.nextEdge = six;
			four.prevEdge = two;

			five.nextEdge = one;
			five.prevEdge = three;

			six.nextEdge = two;
			six.prevEdge = four;

			//Half-edge - vertex connection
			one.v = b;
			two.v = b;
			three.v = c;
			four.v = d;
			five.v = d;
			six.v = a;

			//Half-edge - triangle connection
			Triangle t1 = one.t;
			Triangle t2 = four.t;

			one.t = t1;
			three.t = t1;
			five.t = t1;

			two.t = t2;
			four.t = t2;
			six.t = t2;

			//Opposite-edges are not changing!

			//Triangle connection
			t1.v1 = b;
			t1.v2 = c;
			t1.v3 = d;

			t2.v1 = b;
			t2.v2 = d;
			t2.v3 = a;

			t1.halfEdge = three;
			t2.halfEdge = four;
		}


		public static List<Triangle> TriangulateByFlippingEdges(List<Vector3> sites)
		{
			//Step 1. Triangulate the points with some algorithm
			//Vector3 to vertex
			List<Vertex> vertices = new List<Vertex>();

			for (int i = 0; i < sites.Count; i++)
			{
				vertices.Add(new Vertex(sites[i]));
			}

			//Triangulate the convex hull of the sites
			List<Triangle> triangles = TriangulatePoints.IncrementalTriangulation(vertices);
			//List triangles = TriangulatePoints.TriangleSplitting(vertices);

			//Step 2. Change the structure from triangle to half-edge to make it faster to flip edges
			List<HalfEdge> halfEdges = TransformRepresentation.TransformFromTriangleToHalfEdge(triangles);

			//Step 3. Flip edges until we have a delaunay triangulation
			int safety = 0;

			int flippedEdges = 0;

			while (true)
			{
				safety += 1;

				if (safety > 100000)
				{
					Debug.Log("Stuck in endless loop");

					break;
				}

				bool hasFlippedEdge = false;

				//Search through all edges to see if we can flip an edge
				for (int i = 0; i < halfEdges.Count; i++)
				{
					HalfEdge thisEdge = halfEdges[i];

					//Is this edge sharing an edge, otherwise its a border, and then we cant flip the edge
					if (thisEdge.oppositeEdge == null)
					{
						continue;
					}

					//The vertices belonging to the two triangles, c-a are the edge vertices, b belongs to this triangle
					Vertex a = thisEdge.v;
					Vertex b = thisEdge.nextEdge.v;
					Vertex c = thisEdge.prevEdge.v;
					Vertex d = thisEdge.oppositeEdge.nextEdge.v;

					Vector2 aPos = a.GetPos2D_XZ();
					Vector2 bPos = b.GetPos2D_XZ();
					Vector2 cPos = c.GetPos2D_XZ();
					Vector2 dPos = d.GetPos2D_XZ();

					//Use the circle test to test if we need to flip this edge
					if (Geometry.IsPointInsideOutsideOrOnCircle(aPos, bPos, cPos, dPos) < 0f)
					{
						//Are these the two triangles that share this edge forming a convex quadrilateral?
						//Otherwise the edge cant be flipped
						if (Geometry.IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
						{
							//If the new triangle after a flip is not better, then dont flip
							//This will also stop the algoritm from ending up in an endless loop
							if (Geometry.IsPointInsideOutsideOrOnCircle(bPos, cPos, dPos, aPos) < 0f)
							{
								continue;
							}

							//Flip the edge
							flippedEdges += 1;

							hasFlippedEdge = true;

							FlipEdge(thisEdge);
						}
					}
				}

				//We have searched through all edges and havent found an edge to flip, so we have a Delaunay triangulation!
				if (!hasFlippedEdge)
				{
					//Debug.Log("Found a delaunay triangulation");

					break;
				}
			}

			//Debug.Log("Flipped edges: " + flippedEdges);

			//Dont have to convert from half edge to triangle because the algorithm will modify the objects, which belongs to the 
			//original triangles, so the triangles have the data we need

			return triangles;
		}
	}
}
