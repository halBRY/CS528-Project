﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawLinks : MonoBehaviour
{
    public Mesh Mesh;
    public Material LinesMaterial;
    public GameObject linePrefab;

    public TextAsset starData;

    // Start is called before the first frame update
    void Start()
    {
        DrawConstellations("Assets/Data/constellations.txt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void DrawConstellations(string fileName)
    {
        Renderer rend = GetComponent<Renderer>();

        // Read in CSV
        if(File.Exists(fileName))
        {
            // Each line is a constellation
            //string[] lines = File.ReadAllLines(fileName);

            starData = Resources.Load<TextAsset>("constellations");
            string[] lines = starData.text.Split('\n');

            // For each constellations, extract pairs of verts for lines
            for(int i = 0; i < lines.Length; i++)
            {
                // Individual star coords are separated by a space
                string[] data = lines[i].Split(' ');

                // Count pairs for this constellation
                int numLines = (data.Length / 2) - 1;

                // For each line pair...
                for(int k = 1; k < numLines+1; k++)
                {
                    GameObject myLine = Instantiate (linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    Mesh Mesh;

                    Mesh = new Mesh();
                    Mesh.name = "LineMesh";

                    MeshFilter meshFilter = myLine.GetComponent<MeshFilter>();
                    meshFilter.mesh = Mesh;
                    MeshRenderer meshRenderer = myLine.GetComponent<MeshRenderer>();
                    meshRenderer.material = LinesMaterial;

                    int[] indices = new int[2];
                    Vector3[] vertices = new Vector3[2];
                    Color[] colors = new Color[2];
                    
                    string[] coords_1 = data[k + (k-1)].Split(',');
                    string[] coords_2 = data[k + k].Split(',');

                    //tring printout = string.Format("Constellation {0} has {1} lines, with points X{2} Y{3} Z{4}, X{5}, Y{6}, Z{7}", i, numLines, coords_1[0], coords_1[1], coords_1[2], coords_2[0], coords_2[1], coords_2[2]);
                    //Debug.Log(printout);

                    // Default to very far away, in case the parse fails
                    // There's some null data for HIP values, so there are stars that's coords couldn't be matched
                    float x = 10000f;
                    float y = 10000f;
                    float z = 10000f;

                    float result = 0f;

                    // Extract verts for star 1
                    if (float.TryParse(coords_1[0], out result))
                    {
                        x = result;
                    }
                    if (float.TryParse(coords_1[1], out result))
                    {
                        y = result;
                    }
                    if (float.TryParse(coords_1[2], out result))
                    {
                        z = result;
                    }

                    vertices[0] = new Vector3(x, y, z);
                    indices[0] = 0;
                    colors[0] = new Color(1,1,1,1);
                                
                    // Extract verts for star 2
                    if (float.TryParse(coords_2[0], out result))
                    {
                        x = result;
                    }
                    if (float.TryParse(coords_2[1], out result))
                    {
                        y = result;
                    }
                    if (float.TryParse(coords_2[2], out result))
                    {
                        z = result;
                    }

                    vertices[1] = new Vector3(x, y, z);
                    indices[1] = 1;
                    colors[1] = new Color(1,1,1,1);
                    
                    Mesh.vertices = vertices;
                    Mesh.colors = colors;

                    Mesh.indexFormat = IndexFormat.UInt32;
                    Mesh.SetIndices(indices, MeshTopology.Lines, 0);
                }
            }
        }
        else
        {
            Debug.Log("File not found.");
        }
    }
}
