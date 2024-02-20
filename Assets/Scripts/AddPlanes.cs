using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class AddPlanes : MonoBehaviour
{
    public Mesh Mesh;
    public Material pointCloudMaterial;

    public bool capStars;
    public int maxStarNum;

    public TextAsset starData;

    public GameObject planeBoard;

    // Start is called before the first frame update
    void Start()
    {
        VertsFromCSV("Assets/Data/athyg_v31-1_cleaned.csv");
        //VertsFromCSV("athyg_v31-1_cleaned.csv");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void VertsFromCSV(string fileName)
    {
        // Read in CSV
        if(File.Exists(fileName))
        {
            // Each line is a star 
            //string[] lines = File.ReadAllLines(fileName);

            starData = Resources.Load<TextAsset>("athyg_v31-1_cleaned");
            //Debug.Log(starData);

            //var splitFile = new string[] { "\r\n", "\r", "\n" };
            string[] lines = starData.text.Split('\n');
            
            int size = lines.Length;
            
            int starsToAdd = size - 1;

            if(capStars)
            {
                starsToAdd = maxStarNum;
            }

            // For each star, save the x, y, z and spectral type
            // These values are saved to columns 1, 2, 3, and 6, respectively
            // 4 is mag and 5 is lum
            for(int i = 1; i < starsToAdd ; i++)
            {
                string[] data = lines[i].Split(',');

                float x = float.Parse(data[1]);
                float y = float.Parse(data[2]);
                float z = float.Parse(data[3]);

                GameObject myPlane = Instantiate (planeBoard, new Vector3(x,y,z),  Quaternion.Euler(-90f, 0f, 0f));
                Renderer rend = myPlane.GetComponent<Renderer>();

                // Really awful way of doing it but this is vertex color
                Color myColor = new Color(0,0,0);
                float myRadius = 0;

                switch (data[6][1])
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
                    default:
                        myColor = new Color(0, 255, 255);
                        break;
                }

                myPlane.transform.localScale = new Vector3(myRadius*.25f, myRadius*.25f, myRadius*.25f);
                rend.material.SetColor("_Color", myColor);

                //float H, S, V;
                //Color.RGBToHSV(myColor, out H, out S, out V);
                //V = float.Parse(data[5]);
                //myColor = Color.HSVToRGB(H, S, V);

                myColor.a = 1f;
                myColor.r = myColor.r / 255;
                myColor.g = myColor.g / 255;
                myColor.b = myColor.b / 255;
                //colors[i-1] = myColor;
            
                //rend.material.SetFloat("_Radius", myRadius);
                //rend.material.SetFloat("_Brightness", float.Parse(data[5]));

                //Draw a plane at the given transform
            }

            //Debug.Log("There are " + size + " stars");
        }
        else
        {
            Debug.Log("File not found.");
        }

    }


}
