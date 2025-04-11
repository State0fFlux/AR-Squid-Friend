/****************************************************************************
 * Copyright ©2021 Khoa Nguyen and Quan Dang. Adapted from CSE 457 Modeler by
 * Brian Curless. All rights reserved. Permission is hereby granted to
 * students registered for University of Washington CSE 457.
 * No other use, copying, distribution, or modification is permitted without
 * prior written consent. Copyrights for third-party components of this work
 * must be honored.  Instructors interested in reusing these course materials
 * should contact the authors below.
 * Khoa Nguyen: https://github.com/akkaneror
 * Quan Dang: https://github.com/QuanGary
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

/// <summary>
/// SurfaceOfRevolution is responsible for generating a mesh given curve points.
/// </summary>

#if (UNITY_EDITOR)
public class SurfaceOfRevolution : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector2> curvePoints;
    private int _mode;
    private int _numCtrlPts;
    private readonly string _curvePointsFile = "curvePoints.txt";

    private Vector3[] normals;
    private Vector2[] UVs;
    private Vector3[] vertices;

    private int[] triangles;

    private int subdivisions;
    public TextMeshProUGUI subdivisionText;

    private void Start()
    {
        subdivisions = 16;
        subdivisionText.text = "Subdivision: " + subdivisions.ToString();
    }

    private void Update()
    {
    }

    public void Initialize()
    {
        // Create an empty mesh
        mesh = new Mesh();
        mesh.indexFormat =
            UnityEngine.Rendering.IndexFormat.UInt32; // Set Unity's max number of vertices for a mesh to be ~4 billion
        GetComponent<MeshFilter>().mesh = mesh;

        // Load curve points
        ReadCurveFile(_curvePointsFile);

        // Invalid number of control points
        if (_mode == 0 && _numCtrlPts < 4 || _mode == 1 && _numCtrlPts < 2) return;

        // Calculate and draw mesh
        ComputeMeshData();
        UpdateMeshData();
    }


    /// <summary>
    /// Computes the surface revolution mesh given the curve points and the number of radial subdivisions.
    /// 
    /// Inputs:
    /// curvePoints : the list of sampled points on the curve.
    /// subdivisions: the number of radial subdivisions
    /// 
    /// Outputs:
    /// vertices : a list of `Vector3` containing the vertex positions
    /// normals  : a list of `Vector3` containing the vertex normals. The normal should be pointing out of
    ///            the mesh.
    /// UVs      : a list of `Vector2` containing the texture coordinates of each vertex
    /// triangles: an integer array containing vertex indices (of the `vertices` list). The first three
    ///            elements describe the first triangle, the fourth to sixth elements describe the second
    ///            triangle, and so on. The vertex must be oriented counterclockwise when viewed from the 
    ///            outside.
    /// </summary>
    private void ComputeMeshData()
    {
        // TODO: Compute and set vertex positions, normals, UVs, and triangle faces
        // You will want to use curvePoints and subdivisions variables, and you will
        // want to change the size of these arrays
        int numSlices = curvePoints.Count - 1;
        int numPoints = curvePoints.Count * subdivisions;

        // Initialize stuff
        vertices = new Vector3[numPoints];
        normals = new Vector3[numPoints];
        UVs = new Vector2[numPoints];

        // Logic: there are (numPoints - subdivisions) quads, and each quad has 2 triangles,
        // and each triangle has 3 points
        triangles = new int[(numPoints - subdivisions) * 6];

        // set up the vertices accordingly
        for (int i = 0; i < subdivisions; i++) // for each rotated plane
        {
            float currRotation = (2 * PI) * i / subdivisions;
            for (int j = 0; j < curvePoints.Count; j++) // for each point on this particular plane
            {
                int vertexIndex = i * curvePoints.Count + j;
                Vector2 curvePoint = curvePoints.ElementAt(j);
                vertices[vertexIndex] = new Vector3(curvePoint.x * Sin(currRotation), curvePoint.y, curvePoint.x * Cos(currRotation));
            }
        }

        // set up the triangles accordingly
        for (int i = 0; i < subdivisions; i++) // for each rotated plane
        {
            for (int j = 0; j < numSlices; j++) // for each quad on this plane (1 per horizontal slice)
            {
                // define the current quad's vertices
                int bottomLeftIndex = i * curvePoints.Count + j;
                int bottomRightIndex = (i + 1) % subdivisions * curvePoints.Count + j;
                int topLeftIndex = bottomLeftIndex + 1;
                int topRightIndex = bottomRightIndex + 1;

                // create 2 triangles from quad
                int triangleIndex = (i * numSlices + j) * 6;
                triangles[triangleIndex] = bottomLeftIndex;
                triangles[triangleIndex + 1] = topLeftIndex;
                triangles[triangleIndex + 2] = bottomRightIndex;

                triangles[triangleIndex + 3] = bottomRightIndex;
                triangles[triangleIndex + 4] = topLeftIndex;
                triangles[triangleIndex + 5] = topRightIndex;
            }
        }

        // calculate normal vectors
        for (int i = 0; i < triangles.Length; i += 3) { // for each triangle
            // calculate triangle's normal vector
            Vector3 A = vertices[triangles[i]];
            Vector3 B = vertices[triangles[i+1]];
            Vector3 C = vertices[triangles[i+2]];
            Vector3 normal = Vector3.Cross((A - B), (A - C));

            // add vector to the included vertices' normal vectors
            normals[triangles[i]] += normal;
            normals[triangles[i+1]] += normal;
            normals[triangles[i+2]] += normal;
        }
        // normalize each vertices' normal vector
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = normals[i].normalized;
        }

        // calculate UV vectors
        float curveLength = 0;
        for (int j = 1; j < curvePoints.Count; j++) {
            curveLength += (curvePoints.ElementAt(j) - curvePoints.ElementAt(j - 1)).magnitude;
        }

        for (int i = 0; i < subdivisions; i++)
        { // for each rotated plane
            float u = (float)i / (subdivisions - 1); // 0 to 1 around the full circle
            float vNum = 0;
            for (int j = 0; j < curvePoints.Count; j++)
            { // for each horizontal slice
                if (j != 0) {
                    vNum += (curvePoints.ElementAt(j) - curvePoints.ElementAt(j - 1)).magnitude;
                }
                float v = vNum / curveLength;
                int vertexIndex = i * curvePoints.Count + j;
                UVs[vertexIndex] = new Vector2(u, v);
            }
        }
    }

    private void UpdateMeshData()
    {
        // Assign data to mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = UVs;
    }

    // Export mesh as an asset
    public void ExportMesh()
    {
        string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/ExportedMesh/", mesh.name, "asset");
        if (string.IsNullOrEmpty(path)) return;
        path = FileUtil.GetProjectRelativePath(path);
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }

    public void SubdivisionValueChanged(Slider slider)
    {
        subdivisions = (int)slider.value;
        subdivisionText.text = "Subdivision: " + subdivisions.ToString();
    }
    
    private void ReadCurveFile(string file)
    {
        curvePoints = new List<Vector2>();
        string line;

        var f =
            new StreamReader(file);
        if ((line = f.ReadLine()) != null)
        {
            var curveData = line.Split(' ');
            _mode = Convert.ToInt32(curveData[0]);
            _numCtrlPts = Convert.ToInt32(curveData[1]);
        }

        while ((line = f.ReadLine()) != null)
        {
            var curvePoint = line.Split(' ');
            var x = float.Parse(curvePoint[0]);
            var y = float.Parse(curvePoint[1]);
            curvePoints.Add(new Vector2(x, y));
        }

        f.Close();
    }
}
#endif
