using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PointCloudCustomVertData : MonoBehaviour
{
    public Mesh Mesh;
    public Material pointCloudMaterial;

    public bool capStars;
    public int maxStarNum;

    public float scaleFactor = 3.28084f; //Default to meters -> feet
    public float parsecPerYearConversion = 0.00000102269f;

    public TextAsset starData;

    public float frameNumber;
    public float isSpect;

    // Start is called before the first frame update
    void Start()
    {
        frameNumber = 0;
        isSpect = 1;

        //Point Cloud 
        Mesh = new Mesh();
        Mesh.name = "StarPointCloud";

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = Mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = pointCloudMaterial;

        VertsFromCSV("athyg_v31_cleaned_exo");

        meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        meshRenderer.material.SetFloat("_isSpect", isSpect);
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if(Input.GetKeyDown(KeyCode.X))
        {
            frameNumber += 1000;
            meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            frameNumber -= 1000;
            meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            scaleFactor -= 0.05f;
            meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            scaleFactor += 0.05f;
            meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            if(isSpect == 0)
            {
                isSpect = 1;
            }
            else if(isSpect == 1)
            {
                isSpect = 0;
            }
            meshRenderer.material.SetFloat("_isSpect", isSpect);
        }
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

        
        int[] indices = new int[starsToAdd-1];

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> uv = new List<Vector4>();

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
        // 11 exoplanet number
        // 12 spectral type
        for(int i = 1; i < starsToAdd; i++)
        {
            string[] data = lines[i].Split(',');

            float x = float.Parse(data[5]);
            float y = float.Parse(data[6]);
            float z = float.Parse(data[7]);

            float vx = float.Parse(data[8]) * parsecPerYearConversion;
            float vy = float.Parse(data[9]) * parsecPerYearConversion;
            float vz = float.Parse(data[10]) * parsecPerYearConversion;

            indices[i-1] = i-1;
            vertices.Add(new Vector3(x,y,z));
            normals.Add(new Vector3(vx,vy,vz)); //Not actually normals, just storing

            // Check the first letter of spectal type 
            // Then color and size by spectral type
            char checkSpect = data[12][1];

            // Handle Irregular cases
            if(checkSpect == '(')
            {
                checkSpect = data[12][2];
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

            Color myColor = new Color(0,0,0);
            float myRadius = 0;

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

            float exoNum;
            float result;

            if (float.TryParse(data[11], out result))
            {
                exoNum = result;
            }
            else
            {
                exoNum = 0f;
            }

            //Debug.Log("My exoplanet number is " + exoNum);

            float r;
            float g;
            float b;

            switch (exoNum)
            {
                case 0:
                    r = 50;
                    g = 50;
                    b = 50;
                    break;
                case 1:
                    r = 37;
                    g = 52;
                    b = 148;
                    break;
                case 2:
                    r = 44;
                    g = 127;
                    b = 184;
                    break;
                case 3:
                    r = 65;
                    g = 182;
                    b = 196;
                    break;
                case 4:
                    r = 127;
                    g = 205;
                    b = 187;
                    break;
                case 5:
                    r = 199;
                    g = 233;
                    b = 180;
                    break;
                case 6:
                    r = 255;
                    g = 255;
                    b = 204;
                    break;
                default:
                    r = 30;
                    g = 30;
                    b = 30;
                    break;
            }

            // Convert to 0-1 RGB representation
            myColor.a = 1f;
            myColor.r = myColor.r / 255;
            myColor.g = myColor.g / 255;
            myColor.b = myColor.b / 255;
    
            colors.Add(myColor);

            r = r/255;
            g = g/255;
            b = b/255;

            //Holds exoplanet color scale, and radius of star. 
            uv.Add(new Vector4(r,g,b,myRadius));

            // Calcualte brightness - TO DO
            rend.material.SetFloat("_Brightness", 1.0f);
        }

        // Create mesh 
        Mesh.SetVertices(vertices);
        Mesh.SetColors(colors);

        Mesh.SetNormals(normals);
        Mesh.SetUVs(0, uv);

        Mesh.indexFormat = IndexFormat.UInt32;
        Mesh.SetIndices(indices, MeshTopology.Points, 0);
    }
}
