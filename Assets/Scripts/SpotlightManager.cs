/*******************************************
Written by Hal Brynteson for CS 528 
*******************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightManager : MonoBehaviour
{
    public GameObject[] myTextPanels;
    public TextUpdateSpotlight myTitle;

    //Change the text box on the Big Dipper spotlight panel
    //To match the skyculture that is shown
    public void UpdateText(int setID)
    {
        Debug.Log(setID);
        for(int i = 0; i < 6; i++)
        {
            myTextPanels[i].SetActive(false);
        }

        myTextPanels[setID].SetActive(true);
        myTitle.UpdateText();
    }
}
