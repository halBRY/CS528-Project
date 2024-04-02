using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public GameObject myPlayer;

    public float distance;

    public Material darkSky;
    public Material highlightSky;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            myPlayer.transform.position = new Vector3(0f, 0.52f, -6f);
            myPlayer.transform.rotation = Quaternion.identity;
        }

        distance = Vector3.Distance(myPlayer.transform.position, new Vector3(0f, 1f, 0f)); //Sol location
        Debug.Log("Distance from Sol: " + distance);


        if(distance < 30)
        {
            RenderSettings.skybox = darkSky;
        }
        else
        {
            RenderSettings.skybox = darkSky;
        }

    }
}
