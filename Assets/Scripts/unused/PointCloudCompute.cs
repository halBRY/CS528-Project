using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PointCloudCompute : MonoBehaviour
{
    public Mesh Mesh;
    public Material pointCloudMaterial;

    public ComputeBuffer computeBuffer;

    public bool capStars;
    public int maxStarNum;

    public float scaleFactor = 3.28084f; //Default to meters -> feet

    public TextAsset starData;

    public Bounds bounds;
      
    // struct for computeBuffer
    public struct bufferElement
    {
        public Vector4 position;
        public Vector4 VaC;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Point Cloud 
        /*
        Mesh = new Mesh();
        Mesh.name = "StarPointCloud";*/
        
        //set _scaleFactor and _frameNumber uniforms
        ReadCSV("athyg_v31_cleaned");

        /*MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = Mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = pointCloudMaterial;*/

        pointCloudMaterial.SetFloat("_scaleFactor", scaleFactor);
        pointCloudMaterial.SetFloat("_frameNumber", 0.0f);

        // This will probably need to be changed
        bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        /*
        System.Int32[] cbCheck; 
        
        cbCheck = new int[114889];

        computeBuffer.GetData(cbCheck);

        Debug.Log(cbCheck);*/

        var cbCheck = new bufferElement[114889];
        computeBuffer.GetData(cbCheck); 

        for(int i = 0; i < 114889; i++)
        {
            if(i < 3)
            {
                Debug.Log("Buffer: " + cbCheck[i].position + " " + cbCheck[i].VaC);
            }
        }

        //Graphics.DrawProceduralNow(MeshTopology.Points, computeBuffer.count, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //Graphics.DrawProcedural(pointCloudMaterial, bounds, MeshTopology.Points, computeBuffer.count, 1);
       //update _scaleFactor and _frameNumber uniforms 
    }

    void OnDestroy()
    {
        computeBuffer.Release();
    }

    private void ReadCSV(string fileName)
    {
        Renderer rend = GetComponent<Renderer>();

        // Read in CSV
        // Each line is a star 
        starData = Resources.Load<TextAsset>(fileName);
        //Debug.Log(starData);

        string[] lines = starData.text.Split('\n');
        
        int size = lines.Length;

        //remove header
        int starsToAdd = size - 2;

        //int[] indices = new int[size];
        //Vector3[] vertices = new Vector3[size];

        bufferElement[] bufferElements = new bufferElement[starsToAdd];

        // Show limited number of stars
        if(capStars)
        {
            starsToAdd = maxStarNum;
        }

        //Test that all lines are read in correctly
        Debug.Log("Star count: " + starsToAdd);

        // For each star...
        // Index locations
        // 0 ID
        // 1 hip
        // 2 distance 
        // 3 absmag
        // 4 mag
        // 5 6 7 x, y, z
        // 8 9 10 vx, vy, vz
        // 11 spectral type
        for(int i = 1; i < starsToAdd; i++)
        {
            //indices[i-1] = i-1;

            string[] data = lines[i].Split(',');
            
            float x = float.Parse(data[5]);
            float y = float.Parse(data[6]);
            float z = float.Parse(data[7]);

            float vx = float.Parse(data[8]);
            float vy = float.Parse(data[9]);
            float vz = float.Parse(data[10]);

            bufferElements[i-1].position = new Vector4(x, y, z, 0f);

            //vertices[i-1] = new Vector3(x,y,z);

            // Check the first letter of spectal type 
            // Then color and size by spectral type
            char checkSpect = data[11][1];

            // Handle Irregular cases
            if(checkSpect == '(')
            {
                checkSpect = data[11][2];
            }
            else if(checkSpect == 's') //subdwarf
            {
                checkSpect = 'G'; 
            }
            else if(checkSpect == 'D') //white dwarf
            {
                checkSpect = 'F'; 
            }
            else if(checkSpect == 'R' || checkSpect == 'N' || checkSpect == 'S') //cool giant
            {
                checkSpect = 'M';
            }
            else if(checkSpect == 'W') //Wolf-Rayet
            {
                checkSpect = 'O';
            }
            else if(checkSpect == '6') //Carbon star/redgiant
            {
                checkSpect = 'C';
            }
            else if(checkSpect == 'p') //Peculiar star
            {
                checkSpect = 'P';
            }

            float myColor = 0f;

            switch (checkSpect)
            {
                case 'O':
                    myColor = 0f;
                    break;
                case 'B':
                    myColor = 1f;
                    break;
                case 'A':
                    myColor = 2f;
                    break;
                case 'F':
                    myColor = 3f;
                    break;
                case 'G':
                    myColor = 4f;
                    break;
                case 'K':
                    myColor = 5f;
                    break;
                case 'M':
                    myColor = 6f;
                    break;
                case 'C':
                    myColor = 7f;
                    break;
                case 'P':
                    myColor = 8f;
                    break;
                default:
                    myColor = 9f;
                    //Debug.Log("Star " + i + ", " + data[1] + " has a spect of " + data[11]);
                    break;
            }

            bufferElements[i-1].VaC = new Vector4(vx, vy, vz, myColor);
        }
        // Buffer will be 8 floats
        // [x, y, z, color, vx, vy, vz, flag]
        // https://forum.unity.com/threads/sending-data-to-gpu-computeshader-buffer-materialproperty-and-hlsl-script.1205299/
        computeBuffer = new ComputeBuffer(starsToAdd, sizeof(float) * 8); // 32 bytes
        computeBuffer.SetData(bufferElements);

        // Create mesh 
        /*
        Mesh.vertices = vertices;

        Mesh.indexFormat = IndexFormat.UInt32;
        Mesh.SetIndices(indices, MeshTopology.Points, 0);*/
    }

}
