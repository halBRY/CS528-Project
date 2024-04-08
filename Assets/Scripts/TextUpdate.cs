/*******************************************
Written by Hal Brynteson for CS 528 
*******************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUpdate : MonoBehaviour
{
    public int myID;
    public AppManager appManager;
    public DrawLinksAsMesh myConstellations;

    void Awake()
    {
        //Constellation Name
        if(myID == 0)
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNames[appManager.highlightID];
        }
        // Constellation Number
        else if(myID == 1)
        {
            gameObject.GetComponent<TextMesh>().text = appManager.highlightID.ToString();
        }
        //Constellation name in other language
        else
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNamesOther[appManager.highlightID];
        }
    }

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
