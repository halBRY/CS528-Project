using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUpdate : MonoBehaviour
{
    public int myID;
    public AppManager appManager;
    public DrawLinksAsMesh myConstellations;

    // Start is called before the first frame update
    void Awake()
    {
        if(myID == 0)
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNames[appManager.highlightID];
        }
        else if(myID == 1)
        {
            gameObject.GetComponent<TextMesh>().text = appManager.highlightID.ToString();
        }
        else
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNamesOther[appManager.highlightID];
        }
    }

    // Update is called once per frame
    public void UpdateText()
    {
        if(myID == 0)
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNames[appManager.highlightID];
        }
        else if(myID == 1)
        {
            gameObject.GetComponent<TextMesh>().text = appManager.highlightID.ToString();
        }
        else
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNamesOther[appManager.highlightID];
        }
    }
}
