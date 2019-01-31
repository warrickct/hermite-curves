using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TestTriangle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// var points = GameObject.FindGameObjectsWithTag("vert");
		// var mesh = new Mesh();
		// mesh.vertices = points.Select(p => p.transform.position).ToArray();
		// mesh.triangles = new int[] { 0, 1, 2 };
		// var mFilter = GetComponent<MeshFilter>();
		// mFilter.sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
		SingleCircleTest();
	}

	private void SingleCircleTest()
	{
		Vector3[] verts = GameObject.FindGameObjectsWithTag("vert").OrderBy(b => b.transform.parent.name).Select(v => v.transform.position).ToArray();
		List<int> tris = new List<int>();
		for (int i = 1; i < verts.Length - 1; i++)
		{
			tris.Add(0);
			tris.Add(i);
			tris.Add(i + 1);
		}
		var mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris.ToArray();
		mesh.RecalculateNormals();
		var mFilter = GetComponent<MeshFilter>();
		mFilter.sharedMesh = mesh;
		// material
		var mat = GameObject.FindGameObjectWithTag("vert").GetComponent<MeshRenderer>().material;
		var mRend = GetComponent<MeshRenderer>();
		mRend.material = mat;
	}

	void SingleTriangleTest(){
		// mesh
		var points = GameObject.FindGameObjectsWithTag("vert");
		var mesh = new Mesh();
		mesh.vertices = points.Select(p => p.transform.position).ToArray();
		mesh.triangles = new int[] { 1, 0, 2 };
		// mesh.triangles = new int[] { 0, 1, 2 };
		mesh.RecalculateNormals();
		var mFilter = GetComponent<MeshFilter>();
		mFilter.sharedMesh = mesh;

		// material
		var mat = points[0].GetComponent<MeshRenderer>().material;
		var mRend = GetComponent<MeshRenderer>();
		mRend.material = mat;
	}
}
