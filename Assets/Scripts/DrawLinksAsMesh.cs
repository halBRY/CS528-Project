using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawLinksAsMesh : MonoBehaviour
{
    public Mesh Mesh;
    public Material LinesMaterial;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public TextAsset starData;

    public float scaleFactor = 1f; //Default to meters -> feet
    public float parsecPerYearConversion = 0.00000102269f;

    public float frameNumber;

    public Color LineColor;
    public Color HighlightColor;
    public Color DimColor;

    public GameObject myHighlightPlane;

    public int testNum;
    public int highlightID;

    public int[] constellationSubMeshIds;

    public AppManager appManager;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();

        DrawConstellations("constellations_2");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 0 = forward in time
    // 1 = backward in time
    public void updateFrameNumber(int direction)
    {
        if(direction == 0)
        {
            frameNumber += 500;
            meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        }

        if(direction == 1)
        {
            frameNumber -= 500;
            meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        }
    }

    // 0 = parsec maps to a larger unit
    // 1 = parsec maps to a smaller unit
    public void updateScaleFactor(int direction)
    {
        if(direction == 0)
        {
            scaleFactor -= 0.05f;
            meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        }

        if(direction == 1)
        {
            scaleFactor += 0.05f;
            meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        }
    }

    public void hideLines()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
    }

    public void clearMeshData()
    {
        Destroy(Mesh);
    }

    public void showHighlight(int ID)
    {
        //Debug.Log("Highlight On");
        myHighlightPlane.SetActive(true);

        List<Color> colors = new List<Color>();

        for(int i = 0; i < Mesh.vertices.Length; i++)
        {
            // Not selected constellation
            if(i < constellationSubMeshIds[ID] || i >= constellationSubMeshIds[ID + 1])
            {
                colors.Add(DimColor);
            }
            else
            {
                colors.Add(HighlightColor);
            }
        }
        
        //This is annoying but I think it must be done...
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        Mesh.GetVertices(vertices);
        Mesh.GetNormals(normals);
        Mesh.GetIndices(indices, 0);

        string meshname = Mesh.name;

        Destroy(Mesh);

        Mesh = new Mesh();
        Mesh.name = meshname;

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = Mesh;

        Mesh.SetVertices(vertices);
        Mesh.SetNormals(normals);
        Mesh.SetColors(colors);

        int[] indicesArr = indices.ToArray();
        Mesh.indexFormat = IndexFormat.UInt32;

        Mesh.SetIndices(indicesArr, MeshTopology.Lines, 0);
    }

    public void hideHighlight()
    {
        //Debug.Log("Highlight Off");
        myHighlightPlane.SetActive(false);

        List<Color> colors = new List<Color>();

        for(int i = 0; i < Mesh.vertices.Length; i++)
        {
            colors.Add(LineColor);
        }

        //This is annoying but I think it must be done...
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        Mesh.GetVertices(vertices);
        Mesh.GetNormals(normals);
        Mesh.GetIndices(indices, 0);

        string meshname = Mesh.name;

        Destroy(Mesh);

        Mesh = new Mesh();
        Mesh.name = meshname;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = Mesh;

        Mesh.SetVertices(vertices);
        Mesh.SetNormals(normals);
        Mesh.SetColors(colors);

        int[] indicesArr = indices.ToArray();
        Mesh.indexFormat = IndexFormat.UInt32;

        Mesh.SetIndices(indicesArr, MeshTopology.Lines, 0);
    }

    public void DrawConstellations(string fileName)
    {
        //Lines mesh
        Mesh = new Mesh();
        Mesh.name = fileName;

		meshFilter.mesh = Mesh;
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.material = LinesMaterial;

        Renderer rend = GetComponent<Renderer>();

        // Read in CSV
        // Each line is a constellation
        starData = Resources.Load<TextAsset>(fileName);

        string[] lines = starData.text.Split('\n');

        List<int> indices = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();

        //Keep track of when stars are missing
        bool DropThisLine = false;
        int linesAdded = 0;

        constellationSubMeshIds = new int[lines.Length];

        // For each constellations, extract pairs of verts for lines
        for(int i = 0; i < lines.Length; i++)
        //for(int i = testNum; i < testNum+1; i++)
        {
            //track submesh id
            constellationSubMeshIds[i] = linesAdded * 2;

            // Individual star coords are separated by a space
            string[] myConstellationStars = lines[i].Split(' ');

            // Count pairs for this constellation
            int numPairs = (myConstellationStars.Length / 2);

            int myLinesAdded = 0;

            // For each line pair...
            for(int k = 1; k < numPairs + 1; k++)
            {   
                //Reset missing star flag
                DropThisLine = false;

                Vector3[] verticesTemp = new Vector3[2];
                Vector3[] normalsTemp = new Vector3[2];

                //Get two stars forming a line pair
                string[] star1 = myConstellationStars[k + (k-1)].Split(',');
                string[] star2 = myConstellationStars[k + k].Split(',');

                float x = 1f;
                float y = 1f;
                float z = 1f;

                float vx = 1f;
                float vy = 1f;
                float vz = 1f;

                float result = 0f;

                // Extract verts for star 1
                // If value isn't a number, there is a missing star for this line. 
                if (float.TryParse(star1[0], out result))
                {
                    x = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star1[1], out result))
                {
                    y = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star1[2], out result))
                {
                    z = result;
                }
                else
                {
                    DropThisLine = true;
                }

                // Extract velocity for star 1
                if (float.TryParse(star1[3], out result))
                {
                    vx = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star1[4], out result))
                {
                    vy = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star1[5], out result))
                {
                    vz = result;
                }
                else
                {
                    DropThisLine = true;
                }

                vx *= parsecPerYearConversion;
                vy *= parsecPerYearConversion;
                vz *= parsecPerYearConversion;

                verticesTemp[0] = new Vector3(x, y, z);
                normalsTemp[0] = new Vector3(vx, vy, vz);
                            
                // Extract verts for star 2
                // If value isn't a number, there is a missing star for this line. 
                if (float.TryParse(star2[0], out result))
                {
                    x = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star2[1], out result))
                {
                    y = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star2[2], out result))
                {
                    z = result;
                }
                else
                {
                    DropThisLine = true;
                }

                // Extract velocity for star 1
                if (float.TryParse(star2[3], out result))
                {
                    vx = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star2[4], out result))
                {
                    vy = result;
                }
                else
                {
                    DropThisLine = true;
                }

                if (float.TryParse(star2[5], out result))
                {
                    vz = result;
                }
                else
                {
                    DropThisLine = true;
                }

                vx *= parsecPerYearConversion;
                vy *= parsecPerYearConversion;
                vz *= parsecPerYearConversion;

                verticesTemp[1] = new Vector3(x, y, z);
                normalsTemp[1] = new Vector3(vx, vy, vz);

                if(!DropThisLine)
                {
                    vertices.Add(verticesTemp[0]);
                    normals.Add(normalsTemp[0]);

                    vertices.Add(verticesTemp[1]);
                    normals.Add(normalsTemp[1]);

                    // Line vertex colors
                    colors.Add(LineColor);
                    colors.Add(LineColor);

                    linesAdded++;
                    myLinesAdded++;
                }
                
            }
 
            //Debug.Log("Constellation " + i + " has added " + myLinesAdded + " out of" + numPairs);
        }

        for(int i = 0; i < linesAdded*2; i++)
        {
            indices.Add(i);
        }

        // Create mesh 
        Mesh.SetVertices(vertices);
        Mesh.SetNormals(normals);
        Mesh.SetColors(colors);

        int[] indicesArr = indices.ToArray();
        Mesh.indexFormat = IndexFormat.UInt32;

        Mesh.SetIndices(indicesArr, MeshTopology.Lines, 0);

        meshRenderer.material.SetFloat("_ParsecScaleFactor", scaleFactor);
        meshRenderer.material.SetFloat("_frameNumber", frameNumber);
        meshRenderer.material.SetColor("_LineColor", LineColor);
    }
}
