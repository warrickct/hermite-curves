using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject[] points = GameObject.FindGameObjectsWithTag("point");
            foreach (GameObject point in points)
            {
                gameObject.transform.position = point.transform.position;
            }
        }
    }
}
