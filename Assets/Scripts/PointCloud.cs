using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PointCloud : MonoBehaviour
{
    public Mesh Mesh;
    public Material pointCloudMaterial;

    public bool capStars;
    public int maxStarNum;

    public float scaleFactor = 3.28084f; //Default to meters -> feet

    public TextAsset starData;

    // Start is called before the first frame update
    void Start()
    {
        
        //Point Cloud 
        Mesh = new Mesh();
        Mesh.name = "StarPointCloud";

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = Mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = pointCloudMaterial;

        VertsFromCSV("athyg_v31_cleaned");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void VertsFromCSV(string fileName)
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

        int[] indices = new int[size];
        Vector3[] vertices = new Vector3[size];
        Color[] colors = new Color[size];

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
            string[] data = lines[i].Split(',');

            /*
            if(i < 4)
            {
                for(int j = 0; j < data.Length; j++)
                {
                    Debug.Log("data" + j + " " + data[j]);
                }
            }*/

            float x = float.Parse(data[5]);
            float y = float.Parse(data[6]);
            float z = float.Parse(data[7]);

            //If parsec = meter, convert to foot
            x = x * scaleFactor;
            y = y * scaleFactor;
            z = z * scaleFactor;

            vertices[i-1] = new Vector3(x, y, z);
            indices[i-1] = i-1;

            Color myColor = new Color(0,0,0);
            float myRadius = 0;

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

            switch (checkSpect)
            {
                case 'O':
                    myColor = new Color(146, 181, 255);
                    myRadius = 6.6f;
                    break;
                case 'B':
                    myColor = new Color(191, 213, 255);
                    myRadius = 3.5f;
                    break;
                case 'A':
                    myColor = new Color(213, 224, 255);
                    myRadius = 1.5f;
                    break;
                case 'F':
                    myColor = new Color(249, 245, 255);
                    myRadius = 1.15f;
                    break;
                case 'G':
                    myColor = new Color(255, 237, 227);
                    myRadius = 0.96f;
                    break;
                case 'K':
                    myColor = new Color(255, 218, 181);
                    myRadius = 0.7f;
                    break;
                case 'M':
                    myColor = new Color(255, 181, 108);
                    myRadius = 0.5f;
                    break;
                case 'C':
                    myColor = new Color(255, 124, 91);
                    myRadius = 3.5f;
                    break;
                case 'P':
                    myColor = new Color(231, 210, 255);
                    myRadius = 1.0f;
                    break;
                default:
                    myColor = new Color(255, 0, 0);
                    //Debug.Log("Star " + i + ", " + data[1] + " has a spect of " + data[11]);
                    break;
            }

            // Unused conversion to HSV color 
            //float H, S, V;
            //Color.RGBToHSV(myColor, out H, out S, out V);
            //V = float.Parse(data[5]);
            //myColor = Color.HSVToRGB(H, S, V);

            // Convert to 0-1 RGB representation
            myColor.a = 1f;
            myColor.r = myColor.r / 255;
            myColor.g = myColor.g / 255;
            myColor.b = myColor.b / 255;
            colors[i-1] = myColor;
        
            // Set radius
            rend.material.SetFloat("_Radius", myRadius);

            // Calcualte brightness - TO DO
            rend.material.SetFloat("_Brightness", 1.0f);
        }

        // Create mesh 
        Mesh.vertices = vertices;
        Mesh.colors = colors;

        Mesh.indexFormat = IndexFormat.UInt32;
        Mesh.SetIndices(indices, MeshTopology.Points, 0);
    }


}
