using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
